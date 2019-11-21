using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using SimpleJSON;
using MageApi.Models;
using MageApi.Models.Request;
using MageApi.Models.Response;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Game;
using Mage.Models.Application;
using Mage.Models;

namespace MageSDK.Client {
	public class MageEngine : MonoBehaviour {
		///<summary>isLocalApplicationData is using to indicate where to get Application Data.</summary>
		// this is used to test application in Editor mode, if this is true then Application Data will be load from local resources
		public bool isLocalApplicationData = true;
		///<summary>resetUserDataOnStart is used during test in Unity editor to reset user data whenever editor is running.</summary>
		// if this variable is false, then data will be save to local storage and server accordingly
		public bool resetUserDataOnStart = true;
		public ClientLoginMethod loginMethod;

		[HideInInspector]
		public static MageEngine instance;

		#region private variables
		private bool _isLogin = false;
		#endregion

		public void Awake() {
			if (instance == null)
				instance = this;
			else
				GameObject.Destroy (this.gameObject);
			
			// get application data from server
			GetApplicationData();

			// at start initiate Default user
			InitDefaultUser();
			
		}
		

		public void Start() {
			// login user during start
			if (loginMethod == ClientLoginMethod.LOGIN_DEVICE_UUID) {
				LoginWithDeviceID();
			}
		}


		#region Login & user handling
		///<summary>Login using device id</summary>
		public void LoginWithDeviceID() {
			LoginRequest r = new LoginRequest (ApiSettings.LOGIN_DEVICE_UUID);

			//call to login api
			ApiHandler.instance.SendApi<LoginResponse>(
				ApiSettings.API_LOGIN, 
				r, 
				(result) => {
					//do some other processing here
					OnCompleteLogin (result);
					//GetApplicationData ();
				},
				(errorStatus) => {
					Debug.Log("Error: " + errorStatus);
					//do some other processing here
				}, 
				() => {
					TimeoutHandler();
				}
			);
		}

		///<summary>Init default user, based on input data from Unity</summary>
		private void InitDefaultUser() {
			string randomId = DateTime.Now.Ticks.ToString();
			// initiate default user
			User defaultUser = GetApplicationDataItem<User>(MageEngineSettings.GAME_ENGINE_DEFAULT_USER_DATA);
			defaultUser.id = randomId;
			Character defaultCharacter = GetApplicationDataItem<Character>(MageEngineSettings.GAME_ENGINE_DEFAULT_CHARACTER_DATA);
			defaultCharacter.id = randomId;
			defaultUser.SetCharacter(defaultCharacter);
			Debug.Log("Default User: " + defaultUser.ToJson());

			// update user to game engine
			#if UNITY_EDITOR
				if (resetUserDataOnStart) {
					// in unity and test mode don't reuse user saved previously, always initate new user
					RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_USER, defaultUser);
				} else {
					// if not in test mode, check the user saved in local and use the saved one
					// if there is no user saved locally, then initate a default user
					if (ES2.Exists (MageEngineSettings.GAME_ENGINE_USER)) {
						RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_USER, ES2.Load<User> (MageEngineSettings.GAME_ENGINE_USER));
					} else {
						RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_USER, defaultUser);
					}
				}
			#else
				// if not in Editor mode, check the user saved in local and use the saved one
				// if there is no user saved locally, then initate a default user
				if (ES2.Exists (MageEngineSettings.GAME_ENGINE_USER)) {
					RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_USER, ES2.Load<User> (MageEngineSettings.GAME_ENGINE_USER));
				} else {
					RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_USER, defaultUser);
				}
			#endif
		}

		///<summary>On completed logged in</summary>
		private void OnCompleteLogin(LoginResponse result) {
			//update information after login
			_isLogin = true;
			RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
			RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);
			// Mage game engine will mainly operate on this user
			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER, result.User);

			Debug.Log(result.User.ToJson());

			// handle first time login to system
			if (result.User.status == UserStatus.FIRST_LOGIN) {
				CreateNewUser (result.User);
			} else {
				CreateExistingUser (result.User);
			}
		}

		///<summary>If this is new user then it will need to create a default profile</summary>
		private void CreateNewUser(User u) {
			Debug.Log("Create new user");
			this.AddNewCharacter(this.GetApplicationDataItem<Character>(MageEngineSettings.GAME_ENGINE_DEFAULT_CHARACTER_DATA));
			this.UpdateUserData(this.GetApplicationDataItem<User>(MageEngineSettings.GAME_ENGINE_DEFAULT_USER_DATA).user_datas);
		}

		void CreateExistingUser(User u)
		{
			/* user = u;
			bool isSave = false;
				
			if (user.GetUserData ("Coin") == null) {
				isSave = true;
				user.user_datas.Add (new UserData ("Coin", "100000", ""));
			}
			if (user.GetUserData ("Diamond") == null) {
				isSave = true;
				user.user_datas.Add (new UserData ("Diamond", "50000", ""));
			}

			if (isSave) {
				UpdateUserData ();
				UpdateUserProfile ();
			}

			SaveUserData (); */
		}


		///<summary>Add new character to current user. Once complete, save data to cache</summary>
		public void AddNewCharacter(Character newCharacter) {
			// if newCharacter is null then exist
			if (null == newCharacter) {
				return;
			}
			// Update user data in game engine first
			User u = GetUser();
			// Assign random id first
			newCharacter.id = DateTime.Now.Ticks.ToString();
			
			// user must logged in
			if (!this.IsLogin()) {
				return;
			}

			// switch active character to new character
			SetActiveCharacter(newCharacter);

			// update user to game engine
			#if UNITY_EDITOR
				if (this.resetUserDataOnStart) {
					u.SetCharacter(newCharacter);
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
					return;	
				}
			#endif


			//Update user online and save cache data
			AddGameCharacterRequest r = new AddGameCharacterRequest (newCharacter.character_name, newCharacter.character_type);
			//call to login api
			ApiHandler.instance.SendApi<AddGameCharacterResponse>(
				ApiSettings.API_ADD_GAME_CHARACTER,
				r, 
				(result) => {
					newCharacter.id = result.Character.id;
					newCharacter.status = result.Character.status;
					u.SetCharacter(newCharacter);

					//save to cache
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
				},
				(errorStatus) => {
					Debug.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);

		}

		///<summary>Update character data to current user. Once complete, save data to cache</summary>
		public void UpdateGameCharacterData(List<CharacterData> characterDatas) {
			Character character = GetActiveCharacter();
			User u = GetUser();
			foreach (CharacterData d in characterDatas) {
				character.SetCharacterData(d);
			}

			// user must logged in
			if (!this.IsLogin()) {
				return;
			}

			// update user to game engine
			#if UNITY_EDITOR
				if (this.resetUserDataOnStart) {
					u.SetCharacter(character);
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
					return;	
				}
			#endif

			UpdateGameCharacterDataRequest r = new UpdateGameCharacterDataRequest (character.id);
			r.CharacterDatas = character.character_datas;

			//call to update game character data
			ApiHandler.instance.SendApi<UpdateGameCharacterDataResponse>(
				ApiSettings.API_UPDATE_GAME_CHARACTER_DATAS,
				r, 
				(result) => {
					// once update Character data completed, update local data
					u.SetCharacter(character);
					//save to cache
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
				},
				(errorStatus) => {
					Debug.Log("[UpdateGameCharacterData]: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}

		public void UpdateGameCharacterData(CharacterData characterData) {
			UpdateGameCharacterData(new List<CharacterData>() {characterData});
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData(List<UserData> userDatas) {
			// check if userDatas is valid
			if (null == userDatas || userDatas.Count == 0) {
				return;
			}

			User u = GetUser();
			foreach(UserData d in userDatas) {
				u.SetUserData(d);
			}

			// user must logged in
			if (!this.IsLogin()) {
				return;
			}

			// update user to game engine
			#if UNITY_EDITOR
				if (this.resetUserDataOnStart) {
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
					return;	
				}
			#endif

			// save data to server
			UpdateUserDataRequest r = new UpdateUserDataRequest ();
			r.UserDatas = u.user_datas;
			//call to update user data api
			ApiHandler.instance.SendApi<UpdateUserDataResponse>(
				ApiSettings.API_UPDATE_USER_DATA, 
				r, 
				(result) => {
					Debug.Log("Success: Update user data");
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
				},
				(errorStatus) => {
					Debug.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}
		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData(UserData data) {
			UpdateUserData(new List<UserData>() {data});
		}

		///<summary>Update user profile</summary>
		public void UpdateUserProfile(User user) {
			// check if userDatas is valid
			if (null == user) {
				return;
			}

			User u = GetUser();

			// to be updated
			if ("" != user.fullname && user.fullname != u.fullname) {
				u.fullname = user.fullname;
			}

			if ("" != user.phone && user.phone != u.phone) {
				u.phone = user.phone;
			}

			if ("" != user.email && user.email != u.email) {
				u.email = user.email;
			}

			if ("" != user.avatar && user.avatar != u.avatar) {
				u.avatar = user.avatar;
			}

			if ("" != user.notification_token && user.notification_token != u.notification_token) {
				u.notification_token = user.notification_token;
			}

			// user must logged in
			if (!this.IsLogin()) {
				return;
			}

			// update user to game engine
			#if UNITY_EDITOR
				if (this.resetUserDataOnStart) {
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
					return;	
				}
			#endif

			// save data to server
			UpdateProfileRequest r = new UpdateProfileRequest (u.fullname, u.phone, u.email, u.avatar, u.notification_token);
			
			//call to update user data api
			ApiHandler.instance.SendApi<UpdateProfileResponse>(
				ApiSettings.API_UPDATE_USER_DATA, 
				r, 
				(result) => {
					Debug.Log("Success: Update user profile");
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
				},
				(errorStatus) => {
					Debug.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}

		// 
		public void SetActiveCharacter(Character character) {
			if (null != character) {
				return;
			}
			// user must logged in
			if (!this.IsLogin()) {
				return;
			}

			// update user to game engine
			#if UNITY_EDITOR
				if (this.resetUserDataOnStart) {
					this.SaveCacheData<Character>(character, MageEngineSettings.GAME_ENGINE_ACTIVE_CHARACTER);
					return;	
				}
			#endif
		}

		public Character GetActiveCharacter() {
			return RuntimeParameters.GetInstance().GetParam<Character>(MageEngineSettings.GAME_ENGINE_ACTIVE_CHARACTER);
		}


		#endregion 



		#region API Error & Exception handler
		///<summary>Default timeout handler declare here, client implemnetation can overwrite this</summary>
		public void TimeoutHandler() {
			//timeout handler here
			Debug.Log("Api call is timeout");
		}

		public void ApiErrorHandler() {
			//timeout handler here
			Debug.Log("Api error handler");
		}
		#endregion

		#region Get & Set information
		public bool IsLogin() {
			return _isLogin;
		}

		public User GetUser() {
			return RuntimeParameters.GetInstance().GetParam<User>(MageEngineSettings.GAME_ENGINE_USER);
		}

		#endregion

		#region Application Data manipulation
		///<summary>Get Application Data configured in server to start the application</summary>
		public void GetApplicationData() {

			// Load application data
			#if UNITY_EDITOR
				if (isLocalApplicationData) {
					// in unity and test mode application data can be retrieved from local Resources
					LoadApplicationDataFromResources();
				} else {
					// load application data from server
					LoadApplicationDataFromServer();
				}
			#else
				// load application data from server
				LoadApplicationDataFromServer();
			#endif

			
		}
		///<summary>Get Application Data configured in Resources/Data</summary>
		private void LoadApplicationDataFromResources() {
			List<ApplicationData> localResources = new List<ApplicationData>();
			foreach (string data in MageEngineSettings.GAME_ENGINE_APPLICATION_DATA_ITEM) {
				try {
					var jsonTextFile = Resources.Load<TextAsset>("Data/" + data);
					if(jsonTextFile != null) {

						localResources.Add (new ApplicationData() {
							attr_name = data,
							attr_value = jsonTextFile.text
						});
					} else {
						Debug.Log("Failed to load resource file: " + data);
					}
				} catch (Exception e) {
					Debug.Log("Failed to load resource file: " + data);
				}
				
			}
			
			RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, localResources);
		}

		///<summary>Get Application Data configured in Server</summary>
		private void LoadApplicationDataFromServer() {
			GetApplicationDataRequest r = new GetApplicationDataRequest ();
			//call to login api
			ApiHandler.instance.SendApi<GetApplicationDataResponse>(
				ApiSettings.API_GET_APPLICATION_DATA,
				r, 
				(result) => {
					// store application data
					RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, result.ApplicationDatas);
					ES2.Save<List<ApplicationData>>( result.ApplicationDatas, MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
					
				},
				(errorStatus) => {
					Debug.Log("Error: " + errorStatus);
					//do some other processing here
					if (ES2.Exists (MageEngineSettings.GAME_ENGINE_APPLICATION_DATA)) {
						RuntimeParameters.GetInstance().SetParam (MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, ES2.Load<List<ApplicationData>> (MageEngineSettings.GAME_ENGINE_APPLICATION_DATA));
					}
				},
				() => {
					TimeoutHandler();
				}
			);
		}

		///<summary>Get Application Data item store locally - value as string</summary>
		public string GetApplicationDataItem(string attributeName, string attributeType = "") {
			List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
			if (null != applicationDatas) {
				foreach(ApplicationData data in applicationDatas)
				{
					if(data.attr_name == attributeName && (attributeType == "" || data.attr_type == attributeType)) {
						return data.attr_value;
					}
				}
			}
			return "";		
		}

		///<summary>Get Application Data item store locally - value as object</summary>
		public T GetApplicationDataItem<T>(string attributeName, string attributeType = "") where T: BaseModel {
			List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
			if (null != applicationDatas) {
				foreach(ApplicationData data in applicationDatas)
				{
					if(data.attr_name == attributeName && (attributeType == "" || data.attr_type == attributeType)) {
						return BaseModel.CreateFromJSON<T>(data.attr_value);
					}
				}
			}
			return default(T);		
		}

		///<summary>Get Application Data item store locally - value as list object</summary>
		public List<T> GetApplicationDataByType<T>(string attributeType) where T: BaseModel{
			List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
			List<T> result = new List<T> ();
			if (null != applicationDatas) {
				foreach(ApplicationData data in applicationDatas)
				{
					if(data.attr_type == attributeType) {
						T i = BaseModel.CreateFromJSON<T>(data.attr_value);
						if (null != i) {
							result.Add(i);
						}
					}
				}
			}
			return result;
		}

		///<summary>Get Application Data item store locally - value as list object</summary>
		public void AdminSetApplicationData(List<ApplicationData> applicationDatas) {

		}

		///<summary>Use this function to save data to both Engine / Local file</summary>
		public void SaveCacheData<T>(T data, string cacheName) {
			#if UNITY_EDITOR
				if (!this.resetUserDataOnStart) {
					ES2.Save<T>(data, cacheName);
				}
			#else
				ES2.Save<T>(data, cacheName);
			#endif

			RuntimeParameters.GetInstance().SetParam(cacheName, data);
		}
		#endregion

		#region Event 
		public void SendAppEvent(string eventName) {
			SendUserEventRequest r = new SendUserEventRequest (eventName);

			//call to login api
			ApiHandler.instance.SendApi<SendUserEventResponse>(
				ApiSettings.API_SEND_USER_EVENT,
				r, 
				(result) => {
					//Debug.Log("Success: send event successfully");
				},
				(errorStatus) => {
					//Debug.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}
		#endregion
	}
}

