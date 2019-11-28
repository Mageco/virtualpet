// Change this if needs to define platform test. Remember to change this to production release
// UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBGL
// Production build
/*#if UNITY_EDITOR 
#define PLATFORM_TEST
#endif*/

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBGL
#define PLATFORM_TEST
#endif

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

		public bool isWorkingOnline = false;
		public ClientLoginMethod loginMethod;

		public List<ActionData> actions = new List<ActionData>();

		#region private variables
		private bool _isLogin = false;
		private static bool _isLoaded = false;

		private Hashtable variables;
		#endregion

		void Awake() {
			if (!_isLoaded) {
				_isLoaded = true;
				Load();

				// get application data from server
				GetApplicationData();

				// at start initiate Default user
				InitDefaultUser();
				
				#if PLATFORM_TEST
					if (this.resetUserDataOnStart || !this.isWorkingOnline) {
						// in unity and test mode don't reuse user saved previously, always initate new user
						_isLogin = true;
						//test callback
						OnLoginCompleteCallback();
					} else {
						// login user during start
						if (loginMethod == ClientLoginMethod.LOGIN_DEVICE_UUID) {
							LoginWithDeviceID();
						}
					}
				#else
					// Always connect to server login user during start
					if (loginMethod == ClientLoginMethod.LOGIN_DEVICE_UUID) {
						LoginWithDeviceID();
					}
				#endif
			}
			
		}

		///<summary>Other implementation will override this function</summary>
		protected virtual void Load() {

		}

		// Test callback first, needs to implement event handler
		protected virtual void OnLoginCompleteCallback() {

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
					Debug.Log("Login: " + result.ToJson());
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

			// update user to game engine
			#if PLATFORM_TEST
				if (resetUserDataOnStart) {
					// in unity and test mode don't reuse user saved previously, always initate new user
					SetUser(defaultUser);
				} else {
					// if not in test mode, check the user saved in local and use the saved one
					// if there is no user saved locally, then initate a default user
					if (ES2.Exists (MageEngineSettings.GAME_ENGINE_USER)) {
						SetUser(ES2.Load<User> (MageEngineSettings.GAME_ENGINE_USER));
					} else {
						SetUser(defaultUser);
					}
				}
			#else
				// if not in Editor mode, check the user saved in local and use the saved one
				// if there is no user saved locally, then initate a default user
				if (ES2.Exists (MageEngineSettings.GAME_ENGINE_USER)) {
					SetUser(ES2.Load<User> (MageEngineSettings.GAME_ENGINE_USER));
				} else {
					SetUser(defaultUser);
				}
			#endif
		}

		///<summary>On completed logged in</summary>
		private void OnCompleteLogin(LoginResponse result) {
			//update information after login
			_isLogin = true;
			RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
			RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);

			#if PLATFORM_TEST
				if (resetUserDataOnStart) {
					// in unity and test mode don't reuse user saved previously, always initate new user
					CreateNewUser (result.User);
				} else {
					// handle first time login to system
					if (result.User.status == UserStatus.FIRST_LOGIN) {
						CreateNewUser (result.User);
					} else {
						CreateExistingUser (result.User);
					}
				}
			#else
				// handle first time login to system
				if (result.User.status == UserStatus.FIRST_LOGIN) {
					CreateNewUser (result.User);
				} else {
					CreateExistingUser (result.User);
				}
			#endif

			//test callback
			OnLoginCompleteCallback();
		}

		///<summary>If this is new user then it will need to create a default profile</summary>
		private void CreateNewUser(User u) {
			//Debug.Log("Create new user");
			//this.SetCharacter(this.GetApplicationDataItem<Character>(MageEngineSettings.GAME_ENGINE_DEFAULT_CHARACTER_DATA));
			this.UpdateUserData(this.GetApplicationDataItem<User>(MageEngineSettings.GAME_ENGINE_DEFAULT_USER_DATA).user_datas);
			SetUser(u);
		}

		void CreateExistingUser(User u) {
			/*if (u.characters.Count == 0 || u.characters[0] == null) {
				//update character to store
				this.SetCharacter(this.GetApplicationDataItem<Character>(MageEngineSettings.GAME_ENGINE_DEFAULT_CHARACTER_DATA));
			}*/

			List<UserData> defaultUserData = GetApplicationDataItem<User>(MageEngineSettings.GAME_ENGINE_DEFAULT_USER_DATA).user_datas;

			if (defaultUserData != null && defaultUserData.Count > 0) {
				foreach (UserData d in defaultUserData) {
					string val = u.GetUserData(d.attr_name);
					if ("" == val || val != d.attr_value ) {
						u.SetUserData(d);
					}
				}
				// update data to store
				UpdateUserData(u.user_datas);
			}
		}


		///<summary>Add new character to current user. Once complete, save data to cache</summary>
		public Character SetCharacter(Character newCharacter) {
			// if newCharacter is null then exist
			if (null == newCharacter) {
				return null;
			}
			// Update user data in game engine first
			User u = GetUser();
			// Assign random id first
			if (newCharacter.id == "") {
				newCharacter.id = DateTime.Now.Ticks.ToString();
			}
			
			// user must logged in
			if (!this.IsLogin()) {
				return null;
			}

			// update user to game engine
			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					u.SetCharacter(newCharacter);
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
					return newCharacter;	
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

			return newCharacter;

		}

		public Character GetCharacter(string id) {
			return GetUser().GetCharacter(id);
		}

		///<summary>Update character data to a character. Once complete, save data to cache</summary>
		public void UpdateGameCharacterData(Character character, List<CharacterData> characterDatas) {
			User u = GetUser();
			foreach (CharacterData d in characterDatas) {
				character.SetCharacterData(d);
			}

			// user must logged in
			if (!this.IsLogin()) {
				return;
			}

			// update user to game engine
			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
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

		///<summary>Update character data to a character. Once complete, save data to cache</summary>
		public void UpdateGameCharacterData(Character character, CharacterData characterData) {
			UpdateGameCharacterData(character, new List<CharacterData>() {characterData});
		}

		///<summary>Update character data to a character. Once complete, save data to cache</summary>
		public void UpdateGameCharacterData(string characterId, CharacterData characterData) {
			Character c = GetCharacter(characterId);
			UpdateGameCharacterData(c, characterData);
		}

		///<summary>Update character data to a character. Once complete, save data to cache</summary>
		public void UpdateGameCharacterData<T>(string characterId, T obj) where T:BaseModel{
			Character c = GetCharacter(characterId);
			string className = typeof(T).Name;
			CharacterData d = new CharacterData() {
				attr_name = ApiHandler.GetInstance().ApplicationKey + "_" + className,
				attr_value = obj.ToJson(),
				attr_type = className
			};
			UpdateGameCharacterData(c, d);
		}

		///<summary>Get character data of a character</summary>
		public T GetGameCharacterData<T>(string characterId) where T:BaseModel{
			Character c = GetCharacter(characterId);
			if (null == c) {
				return default(T);
			}
			return BaseModel.CreateFromJSON<T>(c.GetCharacterData(ApiHandler.GetInstance().ApplicationKey + "_" +  typeof(T).Name));
		}

		///<summary>Get character data of a character</summary>
		public string GetGameCharacterData(string characterId, string key) {
			Character c = GetCharacter(characterId);
			if (null == c) {
				return "";
			}
			return c.GetCharacterData(key);
		}

		
		///<summary>Get character data of a character</summary>
		public int GetGameCharacterDataInteger(string characterId, string key) {
			Character c = GetCharacter(characterId);
			if (null == c) {
				return 0;
			}

			if ("" != c.GetCharacterData(key)) {
				return int.Parse(c.GetCharacterData(key));
			} else {
				return 0;
			}


		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData(UserData data) {
			UpdateUserData(new List<UserData>() {data});
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData<T>(T obj) where T:BaseModel {
			string className = typeof(T).Name;
			UserData d = new UserData() {
				attr_name = ApiHandler.GetInstance().ApplicationKey + "_" + className,
				attr_value = obj.ToJson(),
				attr_type = className
			};

			UpdateUserData(d);
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
			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
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
		public string GetUserData(string key) {
			return GetUser().GetUserData(key);
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public int GetUserDataInteger(string key) {
			if (GetUserData (key) != null)
				return int.Parse (GetUserData (key));
			else
				return 0;
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public T GetUserData<T>() where T:BaseModel {
			string key = ApiHandler.GetInstance().ApplicationKey + "_" + typeof(T).Name;
			return BaseModel.CreateFromJSON<T>(GetUser().GetUserData(key));
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
			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
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

		private void SetUser(User u) {
			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER, u);
		}

		#endregion

		#region Application Data manipulation
		///<summary>Get Application Data configured in server to start the application</summary>
		public void GetApplicationData() {

			// Load application data
			#if PLATFORM_TEST
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
					Debug.Log("Load data: " + data + jsonTextFile.text);
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
			#if PLATFORM_TEST
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

			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					return;
				}
			#endif

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

		#region Actions
		public void LogAction(ActionType t){
			ActionData a = new ActionData();
			a.actionType = t;
			a.startTime = System.DateTime.Now;
			actions.Add(a);
	//		Debug.Log(a.actionType + "  " + a.startTime.ToShortTimeString());
			SaveAction();
		}

		public List<ActionData> GetActionLogs(System.DateTime t){
			List<ActionData> temp = new List<ActionData>();
			for(int i=0;i<actions.Count;i++){
				if(actions[i].startTime > t){
					temp.Add(actions[i]);
				}
			}
			return temp;
		}

		void SaveAction(){
			ES2.Save(actions,"ActionLog");
		}

		void LoadAction(){
			if(ES2.Exists("ActionLog")){
				ES2.LoadList<ActionData>("ActionLog");
			}
		}

		#endregion

		#region  Variable
		public void SaveVariables()
		{
			List <string> keys = new List<string> ();
			List <string> values = new List<string> ();
			foreach (DictionaryEntry entry in variables) {
				keys.Add ((string)entry.Key);
				values.Add ((string)entry.Value);
			}
			ES2.Save(keys, "variablesKey");
			ES2.Save(values, "variablesValue");

		}

		public void LoadVariables()
		{
			List <string> keys = new List<string> ();
			List <string> values = new List<string> ();

			if(ES2.Exists("variablesKey"))
				keys = ES2.LoadList<string>("variablesKey");
			if(ES2.Exists("variablesValue"))
				values = ES2.LoadList<string>("variablesValue");

			for (int i = 0; i < keys.Count;i++) {
				if(i < values.Count)
					variables.Add (keys [i], values [i]);

				Debug.Log ("Load Variables " + keys [i] + " " + values [i]);
			}
		}


		#endregion
	}
}

