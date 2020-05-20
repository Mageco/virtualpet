﻿// Change this if needs to define platform test. Remember to change this to production release
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
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MageSDK.Client.Helper;
using System.Security.Cryptography;
using System.Text;
#if BUNNY_CDN
using BunnyCDN.Net.Storage;
#endif

namespace MageSDK.Client {
	public class MageEngine : MonoBehaviour {

        public static MageEngine instance;

        ///<summary>isLocalApplicationData is using to indicate where to get Application Data.</summary>
        // this is used to test application in Editor mode, if this is true then Application Data will be load from local resources
        public bool isLocalApplicationData = true;
		///<summary>resetUserDataOnStart is used during test in Unity editor to reset user data whenever editor is running.</summary>
		// if this variable is false, then data will be save to local storage and server accordingly
		public bool resetUserDataOnStart = true;

		public bool isWorkingOnline = false;
		public ClientLoginMethod loginMethod;

		#region private variables
		private bool _isLogin = false;

		private bool _isLoginRequestSent = false;

		private bool _isReloadRequired = false;
		private bool _isApplicationDataLoaded = false;
		private static bool _isLoaded = false;

		[HideInInspector]
		public Hashtable apiCounter = new Hashtable();

		[HideInInspector]
		public bool isAppActive = true;

		private DateTime lastUserDataUpdate = DateTime.Now;

		public string signatureHashAndroid = "";

		private bool _completedSignatureCheckForAndroid = false;
		#if BUNNY_CDN
		private BunnyCDNStorage bunnyCDNStorage = new BunnyCDNStorage("virtupet", "877b185e-3517-42e0-a21b-04dc7feb9ea24feacd2d-4ac1-4bc6-993e-da1838d60ed5");
		#endif

		private bool _hasDataEncrypted = false;

		private string _encryptKey = "wgIl3ZjLdwYviMeTPo90QhVk1BHLA4YgWt5ES1avh8Lace8wqp5SfetYqRFJvg2xh6Kn1pIrHRyVBGCtgCSn3V8FbsVROZSosJEN5CcHQpX6Roj89TDk0sRYzxPKzbqjzPbjk7PxZhVYAO8vg6kFPF7px8Hwl5yDxElhwxRdlpvmU9La96qelkXyAuTK55JuYOvohN0zdEJp5MSlkfpYxAMcudeB7dxL973Y6B835RIKB8Yq7Usr0IaxvF8QostF";

		#endregion

		void Awake() {

            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);

            if (!_isLoaded) {
				_isLoaded = true;

				Load();

				// get application data from server
				GetApplicationData();

				// at start initiate Default user
				InitDefaultUser();

				// init api cache
				InitApiCache();

				LoadEngineCache();

			}
			
		}

		void Update() {
			//ApiUtils.Log("Mage Engine update...");
			//SceneTrackerHelper.GetInstance().AddScreenTime(SceneManager.GetActiveScene().name);
		}

		void OnApplicationFocus(bool hasFocus) {
			if (hasFocus) {
				SceneTrackerHelper.GetInstance().ResetScreenTime();
			} else {
				isAppActive = false;
			}
		}

		public void DoLogin() {
			// if there is one login request sent, then wait for that request completes
			if (this._isLoginRequestSent) {
				return;
			}
			this._isLoginRequestSent = true;
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

		///<summary>Other implementation will override this function</summary>
		protected virtual void Load() {

		}

		// Test callback first, needs to implement event handler
		protected virtual void OnLoginCompleteCallback() {

		}

		#region Login & user handling
		///<summary>Login using device id</summary>
		public void LoginWithDeviceID() {
			//text.text += RuntimeParameters.GetInstance().ToString();
			LoginRequest r = new LoginRequest (ApiSettings.LOGIN_DEVICE_UUID);

			//text.text += "-----\r\n" + r.ToJson();

			//call to login api
			ApiHandler.instance.SendApi<LoginResponse>(
				ApiSettings.API_LOGIN, 
				r, 
				(result) => {
					//do some other processing here
					ApiUtils.Log("Login: " + result.ToJson());
					OnCompleteLogin (result);
					//GetApplicationData ();
					OnLoginCompleteCallback();
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				}, 
				() => {
					TimeoutHandler();
				}
			);
		}

		///<summary>Init default user, based on input data from Unity</summary>
		private void InitDefaultUser() {
			// initiate default user
			User defaultUser = GetApplicationDataItem<User>(MageEngineSettings.GAME_ENGINE_DEFAULT_USER_DATA);
			defaultUser.id = "0";

			// update user to game engine
			#if PLATFORM_TEST
				if (resetUserDataOnStart) {
					// in unity and test mode don't reuse user saved previously, always initate new user
					SetUser(defaultUser);
				} else {
					// if not in test mode, check the user saved in local and use the saved one
					// if there is no user saved locally, then initate a default user
					if (ES2.Exists (MageEngineSettings.GAME_ENGINE_USER)) {
						if (!this._hasDataEncrypted) {
							ApiUtils.Log("Data has not encrypted");
							SetUser(ES2.Load<User> (MageEngineSettings.GAME_ENGINE_USER));
						} else {
							ApiUtils.Log("Data has encrypted");
							byte[] rawData = ES2.LoadRaw(MageEngineSettings.GAME_ENGINE_USER);
							string encryptedUser = Encoding.UTF8.GetString(rawData, 0, rawData.Length);
							SetUser(User.CreatFromEncryptJson<User>(encryptedUser, this._encryptKey));
						}
						
					} else {
						SetUser(defaultUser);
					}
				}
			#else
				// if not in Editor mode, check the user saved in local and use the saved one
				// if there is no user saved locally, then initate a default user
				if (ES2.Exists (MageEngineSettings.GAME_ENGINE_USER)) {
					if (!this._hasDataEncrypted) {
						ApiUtils.Log("Data has not encrypted");
						SetUser(ES2.Load<User> (MageEngineSettings.GAME_ENGINE_USER));
					} else {
						ApiUtils.Log("Data has encrypted");
						byte[] rawData = ES2.LoadRaw(MageEngineSettings.GAME_ENGINE_USER);
						string encryptedUser = Encoding.UTF8.GetString(rawData, 0, rawData.Length);
						SetUser(User.CreatFromEncryptJson<User>(encryptedUser, this._encryptKey));
					}
				} else {
					SetUser(defaultUser);
				}
			#endif

			ApiUtils.Log("User data after default load: " + GetUser().ToJson());
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

					//assign new messages
					if (result.UserMessages != null && result.UserMessages.Count > 0) {
						MessageHelper.GetInstance().AddNewMessages(result.UserMessages, this.UpdateUserMessageStatusToServer);
						OnHasNewUserMessagesCallback(result.UserMessages);
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

			#if UNITY_ANDROID && !UNITY_EDITOR
				if (!this._completedSignatureCheckForAndroid) {
					if (!CheckApplicationSignature()) {
						OnEvent(MageEventType.ApplicationSignatureFailed);
						Application.Quit();
					} else {
						OnEvent(MageEventType.ApplicationSignatureSuccess);
						this._completedSignatureCheckForAndroid = true;
					}
				}
				
			#endif
		}

		///<summary>If this is new user then it will need to create a default profile</summary>
		private void CreateNewUser(User u) {
			User tmp = GetUser();
			//in case new user, then update with server information
			tmp.id = u.id;
			if ("" != u.notification_token && tmp.notification_token != u.notification_token) {
				tmp.notification_token = u.notification_token;
			}

			SetUser(tmp);
		}

		void CreateExistingUser(User u) {
			User tmp = GetUser();

			//if (tmp.id == "0") {
			tmp.id = u.id;
			tmp.last_run_app_version = u.last_run_app_version;
			//}

			ApiUtils.Log("Local version: " + tmp.GetUserDataInt(UserBasicData.Version) + " server version; " + u.GetUserDataInt(UserBasicData.Version));

			// check and swap version
			if (tmp.GetUserDataInt(UserBasicData.Version) >= u.GetUserDataInt(UserBasicData.Version)) {
				// in case local is newer, then it requires to update server with local
				SetUser(tmp);
				// save user datas to server
				SaveUserDataToServer(tmp);
			} else {
				// in case data from server is newer, then replace local by copy from server
				SetUser(u);
				_isReloadRequired = true;
			}

			if (u.last_run_app_version != ""  && string.Compare(u.last_run_app_version, "1.08") <= 0) {
				ApiUtils.Log("Old Version detected");
				_isReloadRequired = true;
			}
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData(UserData data, bool forceUpdate = false) {
			User u = GetUser();

			// if user is not setup then don't update anything
			if (null == u) {
				return;
			}
			// check to increase version
			int currentVersion = GetUser().GetUserDataInt(UserBasicData.Version);
			if (data.attr_name != UserBasicData.Version.ToString()) {
				UserData newVersion = new UserData(UserBasicData.Version.ToString(), "" + (currentVersion+1), "MageEngine");
				UpdateUserData(new List<UserData>() {data, newVersion}, forceUpdate);

			} else {
				UpdateUserData(new List<UserData>() {data}, forceUpdate);
			}

			//this.OnEvent(MageEventType.UpdateUserData, "Update user data");
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData<T>(T obj, bool forceUpdate = false) where T:BaseModel {
			string className = typeof(T).Name;
			UserData d = new UserData() {
				attr_name = ApiHandler.instance.ApplicationKey + "_" + className,
				attr_value = obj.ToJson(),
				attr_type = className
			};

			// do background enrich data first
			BackgroundEnrichData(d.ExtractFields<T>(obj));
			UpdateUserData(d, forceUpdate);


		}

		private void BackgroundEnrichData(List<UserData> userDatas) {
			User u = GetUser();
			if (null != u && null != userDatas && userDatas.Count > 0) {
				//ApiUtils.Log("Backround update");
				foreach(UserData d in userDatas) {
					u.SetUserData(d);
				}
			}
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		private void UpdateUserData(List<UserData> userDatas, bool forceUpdate = false) {
			// check if userDatas is valid
			if (null == userDatas || userDatas.Count == 0) {
				return;
			}

			User u = GetUser();
			foreach(UserData d in userDatas) {
				u.SetUserData(d);
			}

			// enrich system auto data
			u.SetUserData(new UserData( ApiHandler.instance.ApplicationKey+"_"+MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE, 
										SceneTrackerHelper.GetInstance().ConvertCacheScreenJson(), "MageEngine")
						);
			
			// enrich user event
			u.SetUserData(new UserData(	ApiHandler.instance.ApplicationKey+"_"+MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, 
										MageEventHelper.GetInstance().ConvertEventCounterListToJson(), 
										"MageEngine")
						);

			this.SaveUserDataToCache(u);

            if (this.isWorkingOnline && IsLogin() && (this.IsSendable("UpdateUserDataRequest") || forceUpdate)) {
				/* decided when to send data */
				DateTime now = DateTime.Now;
				double timeToAdd = now.Subtract(this.lastUserDataUpdate).TotalSeconds;

				/* for this we only send if the last update is more than X seconds ago */
				if (u.GetUserDataInt(UserBasicData.Version) < 500 || timeToAdd > GetApplicationDataItemInt(MageEngineSettings.GAME_ENGINE_MIN_USER_DATA_UPDATE_DURATION) || forceUpdate) {
					SaveUserDataToServer(u);
					this.lastUserDataUpdate = now;
				}
			}
		}
		private void SaveUserDataToServer(User u) {
			
			// save data to server
			UpdateUserDataRequest r = new UpdateUserDataRequest ();
			r.UserDatas = u.user_datas;
			//call to update user data api
			ApiHandler.instance.SendApi<UpdateUserDataResponse>(
				ApiSettings.API_UPDATE_USER_DATA, 
				r, 
				(result) => {
					ApiUtils.Log("Success: Update user data");
					this.SaveUserDataToCache(u);
					//clear counter cache
					ResetSendable("UpdateUserDataRequest");

				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}


		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public T GetUserData<T>() where T:BaseModel {
			string key = ApiHandler.instance.ApplicationKey + "_" + typeof(T).Name;

			if (null != GetUser() && "" != GetUser().GetUserData(key)) {
				return BaseModel.CreateFromJSON<T>(GetUser().GetUserData(key));
			} else {
				return null;
			}
			
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

			UpdateUserProfileToServer(u);

		}

		///<summary>Upload file to server and get back the url</summary>
		private void UpdateUserProfileToServer(User u) {
			this.SaveUserDataToCache(u);

			// user must logged in
			if (this.isWorkingOnline && IsLogin()) {
				// save data to server
				UpdateProfileRequest r = new UpdateProfileRequest (u.fullname, u.phone, u.email, u.avatar, u.notification_token);
				
				//call to update user data api
				ApiHandler.instance.SendApi<UpdateProfileResponse>(
					ApiSettings.API_UPDATE_PROFILE, 
					r, 
					(result) => {
						ApiUtils.Log("Success: Update user profile");
					},
					(errorStatus) => {
						ApiUtils.Log("Error: " + errorStatus);
						//do some other processing here
					},
					() => {
						TimeoutHandler();
					}
				);

			}
		}


		///<summary>Upload file to server and get back the url</summary>
		public string UploadFile(string file) {
			UploadFileRequest r = new UploadFileRequest ();

			string imagePath = Application.dataPath + file;

			r.SetUploadFile (File.ReadAllBytes(imagePath));

			string output = "";

			//call to login api
			ApiHandler.instance.UploadFile<UploadFileResponse>(
				r, 
				(result) => {
					ApiUtils.Log("Success: Upload file successfully");
					ApiUtils.Log("Upload URL: " + result.UploadedURL);
					output = result.UploadedURL;
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					//timeout handler here
					ApiUtils.Log("Api call is timeout");
				}
			);

			return output;
		}

		#endregion 



		#region API Error & Exception handler
		///<summary>Default timeout handler declare here, client implemnetation can overwrite this</summary>
		public void TimeoutHandler() {
			//timeout handler here
			ApiUtils.Log("Api call is timeout");
		}

		public void ApiErrorHandler() {
			//timeout handler here
			ApiUtils.Log("Api error handler");
		}
		#endregion

		#region Get & Set information
		public bool IsLogin() {
			return _isLogin;
		}

		public bool IsReloadRequired() {
			return _isReloadRequired;
		}
		public User GetUser() {
			return RuntimeParameters.GetInstance().GetParam<User>(MageEngineSettings.GAME_ENGINE_USER);
		}

		private void SetUser(User u) {
			this.SaveUserDataToCache(u);
			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER, u);

		}

		private void SaveUserDataToCache(User u) {
			#if PLATFORM_TEST
				if (!this.resetUserDataOnStart) {
					if (this._hasDataEncrypted) {
						ApiUtils.Log("Save encrypted data");
						ES2.SaveRaw(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);
					} else {
						this._hasDataEncrypted = true;
						ES2.SaveRaw("{\"Company\": \"Mage Co. Ltd.,\"}", MageEngineSettings.GAME_ENGINE_OWNER);
						ApiUtils.Log("Convert data to encrypt");
						ApiUtils.Log("Encrypted data: " + u.ToEncryptedJson(this._encryptKey));
						//ES2.Delete(MageEngineSettings.GAME_ENGINE_USER);
						//ES2.Save(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);

						ES2.SaveRaw(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);
					}
					
				}
			#else
				if (this._hasDataEncrypted) {
					ApiUtils.Log("Save encrypted data");
					ES2.SaveRaw(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);
				} else {
					this._hasDataEncrypted = true;
					ES2.SaveRaw("{\"Company\": \"Mage Co. Ltd.,\"}", MageEngineSettings.GAME_ENGINE_OWNER);
					ApiUtils.Log("Convert data to encrypt");
					ApiUtils.Log("Encrypted data: " + u.ToEncryptedJson(this._encryptKey));
					//ES2.Delete(MageEngineSettings.GAME_ENGINE_USER);
					//ES2.Save(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);

					ES2.SaveRaw(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);
				}
			#endif
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
					// load from local first and then overwrite with value from server
					LoadApplicationDataFromResources();
					// load application data from server
					LoadApplicationDataFromServer();
				}
			#else
				// load from local first and then overwrite with value from server
				LoadApplicationDataFromResources();
				// load application data from server
				LoadApplicationDataFromServer();
			#endif

			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_OWNER)) {
				this._hasDataEncrypted = true;
			} 
		}

		///<summary>Get Application Data configured in Resources/Data</summary>
		private void LoadApplicationDataFromResources() {
			List<ApplicationData> localResources = new List<ApplicationData>();
			foreach (string data in MageEngineSettings.GAME_ENGINE_APPLICATION_DATA_ITEM) {
				try {
					
					var jsonTextFile = Resources.Load<TextAsset>("Data/" + data);
					//ApiUtils.Log("Load data: " + data + jsonTextFile.text);
					if(jsonTextFile != null) {

						localResources.Add (new ApplicationData() {
							attr_name = data,
							attr_value = jsonTextFile.text
						});
					} else {
						ApiUtils.Log("Failed to load resource file: " + data);
					}
				} catch (Exception e) {
					ApiUtils.Log("Failed to load resource file: " + data);
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
					MergeApplicationDataFromServer(result.ApplicationDatas);
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}

		private void MergeApplicationDataFromServer(List<ApplicationData> serverList) {
			List<ApplicationData> localList = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);

			if (null == localList) {
				// store application data
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, serverList);
				ES2.Save(serverList, MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
				this._isApplicationDataLoaded = true;
			} else {
				List<ApplicationData> mergedList = serverList;
				foreach (ApplicationData data in localList) {
					bool found = false;
					foreach(ApplicationData check in serverList) {
						if(data.attr_name == check.attr_name && (check.attr_type == "" || data.attr_type == check.attr_type)) {
							found = true;	
						} 
					}

					if (!found) {
						mergedList.Add(data);
					}
				}

				// save merged list
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, mergedList);
				ES2.Save(mergedList, MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
				this._isApplicationDataLoaded = true;

			}

			// refresh Event cache counter
			RefereshEventCacheCounter();
		}

		public bool IsApplicationDataLoaded() {
			return this._isApplicationDataLoaded;
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

		public int GetApplicationDataItemInt(string attributeName, string attributeType = "") {
			List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
			if (null != applicationDatas) {
				foreach(ApplicationData data in applicationDatas)
				{
					if(data.attr_name == attributeName && (attributeType == "" || data.attr_type == attributeType)) {
						return int.Parse(data.attr_value);
					}
				}
			}
			return 0;		
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

		///<summary>Save Application Data to server</summary>
		public void AdminSaveApplicationDataToServer(List<ApplicationData> applicationDatas, string unityAdminToken, Action successCallback = null) {
			UpdateApplicationDataRequest r = new UpdateApplicationDataRequest (applicationDatas, unityAdminToken);

			//call to login api
			ApiHandler.instance.SendApi<UpdateApplicationDataResponse>(
				ApiSettings.API_UPDATE_APPLICATION_DATA,
				r, 
				(result) => {
					if (null != successCallback) {
						successCallback();
					}
				},
				(errorStatus) => {
					//ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}

		#endregion

		#region Event 
		private void SendAppEvents() {
			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					return;
				}
			#endif
			ApiUtils.Log(" Queue size: " + ((OnlineCacheCounter)this.apiCounter["SendUserEventListRequest"]).GetMax());
			if (this.IsSendable("SendUserEventListRequest")) {
				ApiUtils.Log("Event can be sent");
				List<MageEvent> cachedEvent = MageEventHelper.GetInstance().GetMageEventsList();
				ApiUtils.Log("Size of Event queue: " + cachedEvent.Count);
				if (cachedEvent.Count > 1) {
					SendUserEventListRequest r = new SendUserEventListRequest (cachedEvent);

					//call to login api
					ApiHandler.instance.SendApi<SendUserEventListResponse>(
						ApiSettings.API_SEND_USER_EVENT_LIST,
						r, 
						(result) => {
							cachedEvent = new List<MageEvent>();
							MageEventHelper.GetInstance().SaveMageEventsList(cachedEvent);
						},
						(errorStatus) => {
							//ApiUtils.Log("Error: " + errorStatus);
							//do some other processing here
						},
						() => {
							TimeoutHandler();
						}
					);
				} else {
					MageEvent t = cachedEvent[0];
					SendUserEventRequest r = new SendUserEventRequest(t.eventName, t.eventDetail);
					//call to login api
					ApiHandler.instance.SendApi<SendUserEventResponse>(
						ApiSettings.API_SEND_USER_EVENT,
						r, 
						(result) => {
							cachedEvent = new List<MageEvent>();
							MageEventHelper.GetInstance().SaveMageEventsList(cachedEvent);
						},
						(errorStatus) => {
							//ApiUtils.Log("Error: " + errorStatus);
							//do some other processing here
						},
						() => {
							TimeoutHandler();
						}
					);
				}
				
			}
		}

		private void SendAppEvent(MageEvent t) {

			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					return;
				}
			#endif
			
			SendUserEventRequest r = new SendUserEventRequest(t.eventName, t.eventDetail);
			//call to login api
			ApiHandler.instance.SendApi<SendUserEventResponse>(
				ApiSettings.API_SEND_USER_EVENT,
				r, 
				(result) => {
					//this.cachedEvent = new List<MageEvent>();
					//SaveEvents();
				},
				(errorStatus) => {
					//ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					TimeoutHandler();
				}
			);
		}

		public void OnEvent(MageEventType type, string eventDetail = "") {
			/*this.cachedEvent.Add(new MageEvent(type, eventDetail));
			SaveEvents();
			SendAppEvents();*/
			// temporary fix to send single event
			ApiUtils.Log("OnEvent: " + type);
			MageEventHelper.GetInstance().OnEvent(type, this.SendAppEvents, eventDetail);
		}

		public void OnEvent<T>(MageEventType type, T obj) where T:BaseModel {
			/*this.cachedEvent.Add(new MageEvent(type, obj.ToJson()));
			SaveEvents();
			SendAppEvents();*/
			// temporary fix to send single event
			ApiUtils.Log("OnEvent: " + type);
			MageEventHelper.GetInstance().OnEvent(type, obj, this.SendAppEvents);
		}

		

		private void LoadEngineCache(){
			// load screen cache
			SceneTrackerHelper.GetInstance().LoadSceneCacheListData();

			// Load User messages
			MessageHelper.GetInstance().LoadUserMessages();

			// load event counter
			MageEventHelper.GetInstance().LoadMageEventData();
		}


		#endregion

		#region  Action Logs

		public void LogAction<T>(T actionData) where T: BaseModel {
			MageLogHelper.GetInstance().LogAction<T>(actionData);

			SendActionLogs<T>();
		}
		private void SendActionLogs<T>() where T: BaseModel {

			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					return;
				}
			#endif

			if (this.IsSendable("SendGameUserActionLogRequest" + MageLogHelper.GetInstance().LoggerID<T>())) {
				SendGameUserActionLogRequest r = new SendGameUserActionLogRequest (MageLogHelper.GetInstance().GetLogger<T>());
			
				//call to send action log api
				ApiHandler.instance.SendApi<SendGameActionLogResponse>(
					ApiSettings.API_SEND_GAME_USER_ACTION_LOG,
					r, 
					(result) => {
						MageLogHelper.GetInstance().ClearLogger<T>();
					},
					(errorStatus) => {
						//ApiUtils.Log("Error: " + errorStatus);
						//do some other processing here
					},
					() => {
						TimeoutHandler();
					}
				);
			}

		}

		


		#endregion

		#region ApiCache
		private void InitApiCache() {
			this.apiCounter.Add("UpdateUserDataRequest", new OnlineCacheCounter(0, 10));
			this.apiCounter.Add("UpdateGameCharacterDataRequest", new OnlineCacheCounter(0, 10));
			this.apiCounter.Add("SendUserEventListRequest", new OnlineCacheCounter(0, 0));
		}

		private void RefereshEventCacheCounter() {
			int maxEventQueue = GetApplicationDataItemInt(MageEngineSettings.GAME_ENGINE_MAX_EVENT_COUNTER_QUEUE);
			if (maxEventQueue > 0) {
				// reset the current
				if (apiCounter.Contains("SendUserEventListRequest")) {
					OnlineCacheCounter x = (OnlineCacheCounter)apiCounter["SendUserEventListRequest"];
					if (x!= null) {
						x.SetMax(maxEventQueue);
						apiCounter["SendUserEventListRequest"] = x;
					}
				}
			}
		}

		private bool IsSendable(string apiName) {
			if (this.apiCounter.Contains(apiName)) {
				OnlineCacheCounter tmp = (OnlineCacheCounter) this.apiCounter[apiName];
				bool check = tmp.IsMax();
				this.apiCounter[apiName] = tmp;
				return check;
			} else {
				return true;
			}
		}

		private void ResetSendable(string apiName) {
			if (this.apiCounter.Contains(apiName)) {
				OnlineCacheCounter tmp = (OnlineCacheCounter) this.apiCounter[apiName];
				tmp.ResetCounter();
				this.apiCounter[apiName] = tmp;
			} 
		}

		public DateTime GetServerTimeStamp() {
			return RuntimeParameters.GetInstance().GetParam<DateTime>(ApiSettings.API_SERVER_TIMESTAMP);
		}

		#endregion


		#region messages & notification 
		private void UpdateUserMessageStatusToServer(string msgId, MessageStatus status) {
			if (this.isWorkingOnline) {
				UpdateMessageStatusRequest r = new UpdateMessageStatusRequest (msgId, status);
			
				//call to send action log api
				ApiHandler.instance.SendApi<UpdateMessageStatusResponse>(
					ApiSettings.API_UPDATE_MESSAGE_STATUS,
					r, 
					(result) => {
						//
					},
					(errorStatus) => {
						//
					},
					() => {
						TimeoutHandler();
					}
				);
			}
		}

		
		protected virtual void OnHasNewUserMessagesCallback(List<Message> newMessages) {

		}

		protected virtual void OnNewFirebaseMessageCallback(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {

		}

		
		public void SetupFirebaseMessaging() {
			Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
			Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
		}

		public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
			ApiUtils.Log("Received Registration Token: " + token.Token);

			User u = GetUser();
			if (u.notification_token != token.Token) {
				u.notification_token = token.Token;
				UpdateUserProfile(u);
			}
		}

		public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
			ApiUtils.Log("Received a new message from: " + e.Message.From);
			OnNewFirebaseMessageCallback(sender, e);
		}

		public void UpdateMessageStatus(string msgId, MessageStatus status) {
			MessageHelper.GetInstance().UpdateMessageStatus(msgId, status, this.UpdateUserMessageStatusToServer);
		}


		#endregion

		#region friends

		public void GetRandomFriend(Action<User> getRandomFriendCallback, string friendId = "") {
			GetUserProfileRequest r = new GetUserProfileRequest ();
			if (friendId != "") {
				r.ProfileId = friendId;
			}

			//call to login api
			ApiHandler.instance.SendApi<GetUserProfileResponse>(
				ApiSettings.API_GET_USER_PROFILE,
				r, 
				(result) => {
					ApiUtils.Log("Success: get user profile successfully");
					ApiUtils.Log("Profile result: " + result.ToJson());
					getRandomFriendCallback(result.UserProfile);
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					//timeout handler here
					ApiUtils.Log("Api call is timeout");
				}
			);
		}

		#endregion


		#region security check
		private byte[] GetSignatureHash ()
		{
			#if !UNITY_EDITOR && UNITY_ANDROID
					try {
						using (AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
							using (AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")) {
								using (AndroidJavaObject packageManager = curActivity.Call<AndroidJavaObject> ("getPackageManager")) {
									// Get Android application name
									string packageName = curActivity.Call<string> ("getPackageName");
									// Get Signature
									int signatureInt = packageManager.GetStatic<int> ("GET_SIGNATURES");  
									using (AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject> ("getPackageInfo", packageName, signatureInt)) {
										AndroidJavaObject[] signatures = packageInfo.Get<AndroidJavaObject[]> ("signatures");  
										// return Signature hash
										if (signatures != null && signatures.Length > 0) {
											//
											AndroidJavaObject obj = signatures[0].Call<AndroidJavaObject> ("toByteArray");  
											//int hashCode = signatures[0].Call<int> ("hashCode");  
											return GetByteArrayFromJava(obj) ;
										}
									}
								}
							}
						}
					} catch (System.Exception e) {
						ApiUtils.Log (e.Message);
						return new byte[0];
					}
			
					return new byte[0];
			#endif
					return new byte[0];
		}


		private byte[] GetByteArrayFromJava(AndroidJavaObject obj) {
			if (obj.GetRawObject().ToInt32() != 0)
			{
			// String[] returned with some data!
				byte[] result = AndroidJNIHelper.ConvertFromJNIArray<byte[]>
							(obj.GetRawObject());
				return result;
			}
			else
			{
				return new byte[0];
			}
		}

		private string Sha1HashFile(byte[] file)
		{
			using (SHA1Managed sha1 = new SHA1Managed())
			{
				return BitConverter.ToString(sha1.ComputeHash(file)).Replace("-", ":");
			}
		}

		private bool CheckApplicationSignature() {
			string enforedAndroidSignature = GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_ENFORECED_ANDROID_SIGNATURE);

			// if not enforced from server then always return true
			if (enforedAndroidSignature != "true") {
				return true;
			}

			// if enforeced is set, then check signature
			string signatureFromServer = GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_ANDROID_SIGNATURE_SHA1);
			if (signatureFromServer != "") {
				this.signatureHashAndroid = signatureFromServer;
			}
			
			return (string.Compare(this.Sha1HashFile(this.GetSignatureHash()), this.signatureHashAndroid) == 0);
		}
		#endregion

		#region File upload
		public void UploadFile(string sourcePath, Action<string> onUploadCompleteCallback, Action<int> onErrorCallback = null) {
			#if BUNNY_CDN

			#else
				UploadFileRequest r = new UploadFileRequest ();
				r.SetUploadFile (File.ReadAllBytes(sourcePath));

				//call to login api
				ApiHandler.instance.UploadFile<UploadFileResponse>(
					r, 
					(result) => {
						ApiUtils.Log("Success: Upload file successfully");
						ApiUtils.Log("Upload URL: " + result.UploadedURL);
						onUploadCompleteCallback(result.UploadedURL);
					},
					(errorStatus) => {
						ApiUtils.Log("Error: " + errorStatus);
						//do some other processing here
						if (null != onErrorCallback) {
							onErrorCallback(errorStatus);
						}
					},
					() => {
						//timeout handler here
						ApiUtils.Log("Api call is timeout");
					}
				);
			#endif
		}

		public void UploadAvatar(Texture2D image, Action<string> onUploadCompleteCallback = null) {
			ApiUtils.Log("Upload avatar");
			#if BUNNY_CDN
				ApiUtils.Log("Calling bunny upload");
				UploadPNGToBunnyCDN(image, onUploadCompleteCallback);
			#else
				UploadFileRequest r = new UploadFileRequest ();
				r.SetUploadFile (image.EncodeToPNG());

				//call to login api
				ApiHandler.instance.UploadFile<UploadFileResponse>(
					r, 
					(result) => {
						ApiUtils.Log("Success: Upload file successfully");
						ApiUtils.Log("Upload URL: " + result.UploadedURL);

						User u = GetUser();
						u.avatar = result.UploadedURL;
						string[] keys = result.UploadedURL.Split ('/');
						string path = keys [keys.Length - 1];
						ES2.SaveImage(image,path);
						UpdateUserProfile(u);
						if (onUploadCompleteCallback != null) {
							onUploadCompleteCallback(result.UploadedURL);
						}
					},
					(errorStatus) => {
						ApiUtils.Log("Error: " + errorStatus);
						//do some other processing here
					},
					() => {
						//timeout handler here
						ApiUtils.Log("Api call is timeout");
					}
				);
			#endif

		}

		#if BUNNY_CDN
		private IEnumerator UploadPNGToBunnyCDN(Texture2D image, Action<string> onUploadCompleteCallback = null  ) {
			if (null == bunnyCDNStorage) {
				ApiUtils.Log("Bunny is null");
				yield return null;
			}

			string targetFilename = GetUser().id+"_"+DateTime.Now.ToString("YYYYMMDDhhmmss")+".png";
			ApiUtils.Log("Target file: " + targetFilename);

			Stream stream = new MemoryStream(image.EncodeToPNG());
			yield return bunnyCDNStorage.UploadAsync(stream, "/virtupet/" + targetFilename);

			string avatarUrl = "https://virtupet.b-cdn.net/virtupet/" + targetFilename;
			User u = GetUser();
			u.avatar = avatarUrl;

			ES2.SaveImage(image, targetFilename);
			UpdateUserProfile(u);
			if (onUploadCompleteCallback != null) {
				onUploadCompleteCallback(avatarUrl);
			}
		}

		async void LoadImageFromBunnyCDN(string avatarUrl, Action<Texture2D> onLoadCompleteCallback){


			string[] keys = avatarUrl.Split ('/');
			string path = keys [keys.Length - 1];
			Texture2D tex = new Texture2D(128, 128,TextureFormat.ARGB32,false);
			if (ES3.FileExists (path)) {
				tex = ES3.LoadImage (path);
			} else {
				ApiUtils.Log ("start Download");
				Stream stream = await bunnyCDNStorage.DownloadObjectAsStreamAsync("/virtupet/" + path) ;

				using(var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					tex.LoadImage(memoryStream.ToArray());
					ApiUtils.Log ("downloaded");
					ES3.SaveImage (tex, path);
					ApiUtils.Log ("saved");
				}
			}

			if (tex != null) {
				onLoadCompleteCallback(tex);
			}
		}

		#endif //BUNNY_CDN

		IEnumerator LoadImageCoroutine(string avatarUrl, Action<Texture2D> onLoadCompleteCallback)
		{
			string[] keys = avatarUrl.Split ('/');
			string path = keys [keys.Length - 1];
			Texture2D tex = new Texture2D(128, 128,TextureFormat.ARGB32,false);
			if (ES3.FileExists (path)) {
				tex = ES3.LoadImage (path);
			} else {
				WWW url = new WWW (avatarUrl);
				ApiUtils.Log ("start Download");
				yield return url;
				url.LoadImageIntoTexture (tex);
				ApiUtils.Log ("downloaded");
				ES3.SaveImage (tex, path);
				ApiUtils.Log ("saved");
			}

			if (tex != null) {
				onLoadCompleteCallback(tex);
			}
		}

		public void LoadImage(string avatarUrl, Action<Texture2D> onLoadCompleteCallback) {
			if (avatarUrl != "") {
				#if BUNNY_CDN
					LoadImageFromBunnyCDN(avatarUrl, onLoadCompleteCallback);
				#else 
					StartCoroutine(LoadImageCoroutine(avatarUrl, onLoadCompleteCallback));
				#endif
				
			}
			
		}

		public void LoadAvatar(Action<Texture2D> onLoadCompleteCallback) {
			LoadImage(GetUser().avatar, onLoadCompleteCallback);
		}
		#endregion
		

		#region Leaderboard

		public void GetLeaderBoardFromObject(object t, string fieldName, Action<List<LeaderBoardItem>> onCompleteCallback = null, int index = -1) {
			
			string name = "Field_" + t.GetType().Name + "_" + fieldName + (index >= 0 ? "_" + index : "");
			if (null != onCompleteCallback) {
				GetLeaderBoardFromServer(name, onCompleteCallback);
			} else {
				GetLeaderBoardFromServer(name);
			}
			
		}

		public void GetLeaderBoardFromServer(string fieldName, Action<List<LeaderBoardItem>> onCompleteCallback = null) {
			GetLeaderBoardRequest r = new GetLeaderBoardRequest (fieldName);

			//call to login api
			ApiHandler.instance.SendApi<GetLeaderBoardResponse>(
				ApiSettings.API_GET_LEADER_BOARD,
				r, 
				(result) => {
					ApiUtils.Log("Success: get leaderboard successfully");
					ApiUtils.Log("Leaderboard result: " + result.ToJson());

					if (null != onCompleteCallback) {
						onCompleteCallback(result.Leaders);
					}
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					//timeout handler here
					ApiUtils.Log("Api call is timeout");
				}
			);
		}

		#endregion

	}
		
}

