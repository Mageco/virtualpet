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
		private bool _isReloadRequired = false;
		private static bool _isLoaded = false;

		private Hashtable variables;

		private Hashtable apiCounter = new Hashtable();

		private List<string> actionLogsKeyLookup = new List<string>();

		private List<MageEvent> cachedEvent = new List<MageEvent>();

		//private List<ActionLog> cachedActionLog = new List<ActionLog>();
		private Hashtable cachedActionLog = new Hashtable();

		private List<CacheScreenTime> cachedScreenTime = new List<CacheScreenTime>();

		private List<Message> cachedUserMessages = new List<Message>();
		private bool isAppActive = true;
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
			AddScreenTime();
		}

		void OnApplicationFocus(bool hasFocus) {
			if (hasFocus) {
				ResetScreenTime();
			} else {
				isAppActive = false;
			}
		}

		public void DoLogin() {
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
					Debug.Log("Login: " + result.ToJson());
					OnCompleteLogin (result);
					//GetApplicationData ();
					OnLoginCompleteCallback();
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

					//assign new messages
					if (result.UserMessages != null && result.UserMessages.Count > 0) {
						AddNewMessages(result.UserMessages);
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
				Debug.Log("Old Version detected");
				_isReloadRequired = true;
			}
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData(UserData data) {
			// check to increase version
			int currentVersion = GetUser().GetUserDataInt(UserBasicData.Version);
			if (data.attr_name != UserBasicData.Version.ToString()) {
				UserData newVersion = new UserData(UserBasicData.Version.ToString(), "" + (currentVersion+1), "MageEngine");
				UpdateUserData(new List<UserData>() {data, newVersion});

			} else {
				UpdateUserData(new List<UserData>() {data});
			}

			//this.OnEvent(MageEventType.UpdateUserData, "Update user data");
		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		public void UpdateUserData<T>(T obj) where T:BaseModel {
			string className = typeof(T).Name;
			UserData d = new UserData() {
				attr_name = ApiHandler.instance.ApplicationKey + "_" + className,
				attr_value = obj.ToJson(),
				attr_type = className
			};

			UpdateUserData(d);

		}

		///<summary>Update user data to current user. Once complete, save data to cache</summary>
		private void UpdateUserData(List<UserData> userDatas) {
			// check if userDatas is valid
			if (null == userDatas || userDatas.Count == 0) {
				return;
			}

			User u = GetUser();
			foreach(UserData d in userDatas) {
				u.SetUserData(d);
			}

			// enrich system auto data
			u.SetUserData(new UserData(ApiHandler.instance.ApplicationKey+"_"+MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE, ConvertCacheScreenJson(), "MageEngine"));
			u.SetUserData(new UserData(ApiHandler.instance.ApplicationKey+"_"+MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, ConvertEventCounterListToJson(), "MageEngine"));

            this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);

            if (this.isWorkingOnline && IsLogin() && this.IsSendable("UpdateUserDataRequest")) {
				SaveUserDataToServer(u);
			}
		}

		private string ConvertCacheScreenJson() {
			if (null != cachedScreenTime) {
				string output = "{";
				for (int i = 0; i < cachedScreenTime.Count; i++) {
					output += "\"" + MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE_PREFIX + cachedScreenTime[i].Key + "\": " + cachedScreenTime[i].Value + ", ";
				}

				if (cachedScreenTime.Count > 0) {
					output = output.Substring(0, output.Length - 2);
				}

				output += "}";
				return output;
			} else {
				return "";
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
					Debug.Log("Success: Update user data");
					this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
					//clear counter cache
					ResetSendable("UpdateUserDataRequest");

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
		public T GetUserData<T>() where T:BaseModel {
			string key = ApiHandler.instance.ApplicationKey + "_" + typeof(T).Name;
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

			UpdateUserProfileToServer(u);

		}


		///<summary>Update user notification token</summary>
		public void UpdateNotificationToken(string notificationToken) {

			User u = GetUser();

			if (notificationToken != "" && notificationToken != u.notification_token) {
				u.notification_token = notificationToken;
			} else {
				return;
			}

			UpdateUserProfileToServer(u);

		}

		///<summary>Update user avatar</summary>
		public void UpdateUserAvatar(string avatarImage) {

			// upload image 
			string avatarUrl = UploadFile(avatarImage);

			User u = GetUser();

			if (avatarUrl != "" && avatarUrl != u.avatar) {
				u.avatar = avatarUrl;
			} else {
				return;
			}

			UpdateUserProfileToServer(u);

		}

		///<summary>Upload file to server and get back the url</summary>
		private void UpdateUserProfileToServer(User u) {
			this.SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);

			// user must logged in
			if (this.isWorkingOnline && IsLogin()) {
				// save data to server
				UpdateProfileRequest r = new UpdateProfileRequest (u.fullname, u.phone, u.email, u.avatar, u.notification_token);
				
				//call to update user data api
				ApiHandler.instance.SendApi<UpdateProfileResponse>(
					ApiSettings.API_UPDATE_PROFILE, 
					r, 
					(result) => {
						Debug.Log("Success: Update user profile");
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
					Debug.Log("Success: Upload file successfully");
					Debug.Log("Upload URL: " + result.UploadedURL);
					output = result.UploadedURL;
				},
				(errorStatus) => {
					Debug.Log("Error: " + errorStatus);
					//do some other processing here
				},
				() => {
					//timeout handler here
					Debug.Log("Api call is timeout");
				}
			);

			return output;
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

		public bool IsReloadRequired() {
			return _isReloadRequired;
		}
		public User GetUser() {
			return RuntimeParameters.GetInstance().GetParam<User>(MageEngineSettings.GAME_ENGINE_USER);
		}

		private void SetUser(User u) {
			//RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER, u);
			SaveCacheData<User>(u, MageEngineSettings.GAME_ENGINE_USER);
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
					//Debug.Log("Load data: " + data + jsonTextFile.text);
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
		private void SaveCacheData<T>(T data, string cacheName) {
			#if PLATFORM_TEST
				if (!this.resetUserDataOnStart) {
					ES2.Save<T>(data, cacheName);
				}
			#else
				ES2.Save<T>(data, cacheName);
			#endif

			RuntimeParameters.GetInstance().SetParam(cacheName, data);
		}

		private T GetCacheData<T>(string cacheName) {
			T t = RuntimeParameters.GetInstance().GetParam<T>(cacheName);

			if (t == null) {
				if (ES2.Exists(cacheName)) {
					t =  ES2.Load<T>(cacheName);
					if (t == null) {
						return default(T);
					}
					return t;
				} else {
					return default(T);
				}
			} else {
				return t;
			}
		}

		private T LoadCacheData<T>(string cacheName) {
			
			if (ES2.Exists(cacheName)) {
				T t = ES2.Load<T>(cacheName);
				if (t == null) {
					t = default(T);
				}

				RuntimeParameters.GetInstance().SetParam(cacheName, t);
				return t;
			} else {
				T t = default(T);
				RuntimeParameters.GetInstance().SetParam(cacheName, t);
				return t;
			}
		}

		private void SaveScreenCacheListData(List<CacheScreenTime> data, string cacheName) {
			#if PLATFORM_TEST
				if (!this.resetUserDataOnStart) {
					ES2.Save(data, cacheName);
				}
			#else
				ES2.Save(data, cacheName);
			#endif

			RuntimeParameters.GetInstance().SetParam(cacheName, data);
		}

		private List<CacheScreenTime> LoadScreenCacheListData(string cacheName) {
			
			if (ES2.Exists(cacheName)) {
				List<CacheScreenTime> t = ES2.LoadList<CacheScreenTime>(cacheName);
				if (t == null) {
					t = new List<CacheScreenTime>();
				}
				RuntimeParameters.GetInstance().SetParam(cacheName, t);
				return t;
				
			} else {
				List<CacheScreenTime> t = new List<CacheScreenTime>();
				RuntimeParameters.GetInstance().SetParam(cacheName, t);
				return t;
			}
		}

		#endregion

		#region Event 
		private void SendAppEvents() {
			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					return;
				}
			#endif
			if (this.IsSendable("SendUserEventListRequest")) {
				
				if (this.cachedEvent.Count > 1) {
					SendUserEventListRequest r = new SendUserEventListRequest (this.cachedEvent);

					//call to login api
					ApiHandler.instance.SendApi<SendUserEventListResponse>(
						ApiSettings.API_SEND_USER_EVENT_LIST,
						r, 
						(result) => {
							this.cachedEvent = new List<MageEvent>();
							SaveEvents();
						},
						(errorStatus) => {
							//Debug.Log("Error: " + errorStatus);
							//do some other processing here
						},
						() => {
							TimeoutHandler();
						}
					);
				} else {
					MageEvent t = this.cachedEvent[0];
					SendUserEventRequest r = new SendUserEventRequest(t.eventName, t.eventDetail);
					//call to login api
					ApiHandler.instance.SendApi<SendUserEventResponse>(
						ApiSettings.API_SEND_USER_EVENT,
						r, 
						(result) => {
							this.cachedEvent = new List<MageEvent>();
							SaveEvents();
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
					//Debug.Log("Error: " + errorStatus);
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
			AddEventCounter(type.ToString());
			SendAppEvent(new MageEvent(type, eventDetail));
		}

		public void OnEvent<T>(MageEventType type, T obj) where T:BaseModel {
			/*this.cachedEvent.Add(new MageEvent(type, obj.ToJson()));
			SaveEvents();
			SendAppEvents();*/
			// temporary fix to send single event
			AddEventCounter(type.ToString());
			SendAppEvent(new MageEvent(type, obj.ToJson()));
		}

		private void SaveEvents(){
			ES2.Save(this.cachedEvent, MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
		}

		private void LoadEngineCache(){
			// load event cache
			if(ES2.Exists(MageEngineSettings.GAME_ENGINE_EVENT_CACHE)){
				this.cachedEvent = ES2.LoadList<MageEvent>(MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
				if (this.cachedEvent == null) {
					this.cachedEvent = new List<MageEvent>();
					SaveEvents();
				}
	
			} else  {
				this.cachedEvent = new List<MageEvent>();
			}

			// load action log cache
			if(ES2.Exists(MageEngineSettings.GAME_ENGINE_ACTION_LOGS_KEY_LOOKUP)){
				this.actionLogsKeyLookup = ES2.LoadList<string>(MageEngineSettings.GAME_ENGINE_ACTION_LOGS_KEY_LOOKUP);
				if (this.actionLogsKeyLookup == null) {
					this.actionLogsKeyLookup = new List<string>();
				}
			} else  {
				this.actionLogsKeyLookup = new List<string>();
			}

			foreach(string key in this.actionLogsKeyLookup) {
				if(ES2.Exists(key)){
					List<ActionLog> tmp = ES2.LoadList<ActionLog>(key);
					if (tmp == null) {
						tmp =new List<ActionLog>();
					} 
					
					this.cachedActionLog.Add(key, tmp);
				} else  {
					List<ActionLog> tmp =new List<ActionLog>();
					this.cachedActionLog.Add(key, tmp);
				}
			}

			// load screen cache
			this.cachedScreenTime = LoadScreenCacheListData(MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE);

			LoadCacheData<string>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN);
			LoadCacheData<DateTime>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);

			// Load User messages
			LoadUserMessages();

			// load event counter
			LoadEventCounterList();
		}


		#endregion

		#region  Action Logs

		private void AddLogger<T>() where T:BaseModel {
			if (!this.cachedActionLog.Contains(LoggerID<T>())) {
				List<ActionLog> tmp = new List<ActionLog>();
				this.cachedActionLog.Add(LoggerID<T>(), tmp);
				this.apiCounter.Add("SendGameUserActionLogRequest"+LoggerID<T>(), new OnlineCacheCounter(0, 3));
			}
		}

		public void LogAction<T>(T actionData) where T: BaseModel {
			if (!this.cachedActionLog.Contains(LoggerID<T>())) {
				// add action
				List<ActionLog> tmp = new List<ActionLog>();
				tmp.Add(new ActionLog() {
					action_date = DateTime.Now.ToString("dd-MM-yyyy"),
					sequence = int.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
					action_detail = actionData.ToJson()
				});
				this.cachedActionLog.Add(LoggerID<T>(), tmp);

				//init api cache counter
				this.apiCounter.Add("SendGameUserActionLogRequest"+LoggerID<T>(), new OnlineCacheCounter(0, 3));
				//save action
				SaveActionsLogs<T>(tmp);
			} else {
				List<ActionLog> tmp = (List<ActionLog>)this.cachedActionLog[LoggerID<T>()];
				tmp.Add(new ActionLog() {
					action_date = DateTime.Now.ToString("dd-MM-yyyy"),
					sequence = int.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
					action_detail = actionData.ToJson()
				});
				this.cachedActionLog[LoggerID<T>()] = tmp;
				SaveActionsLogs<T>(tmp);
			}

			SendActionLogs<T>();
		}

		private void ClearLogger<T>() where T:BaseModel {
			List<ActionLog> tmp = new List<ActionLog>();
			this.cachedActionLog[LoggerID<T>()] = tmp;
			SaveActionsLogs<T>(tmp);
		}

		private string LoggerID<T>() {
			string className = typeof(T).Name;
			return MageEngineSettings.GAME_ENGINE_ACTION_LOGS + "_" + className;
		}
		
		private List<ActionLog> GetLogger<T>() where T: BaseModel{
			if (!this.cachedActionLog.Contains(LoggerID<T>())) {
				AddLogger<T>();
				return (List<ActionLog>)this.cachedActionLog[LoggerID<T>()];
			} else {
				return (List<ActionLog>)this.cachedActionLog[LoggerID<T>()];
			}
		}

		private void SaveActionsLogs<T>(List<ActionLog> actionLogs) where T: BaseModel {
			ES2.Save(actionLogs, LoggerID<T>());
		}

		private void SendActionLogs<T>() where T: BaseModel {

			#if PLATFORM_TEST
				if (this.resetUserDataOnStart || !this.isWorkingOnline) {
					return;
				}
			#endif

			if (this.IsSendable("SendGameUserActionLogRequest" + LoggerID<T>())) {
				SendGameUserActionLogRequest r = new SendGameUserActionLogRequest (GetLogger<T>());
			
				//call to send action log api
				ApiHandler.instance.SendApi<SendGameActionLogResponse>(
					ApiSettings.API_SEND_GAME_USER_ACTION_LOG,
					r, 
					(result) => {
						ClearLogger<T>();
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

		}

		private void AddScreenTime() {
			// don't count when app is not active
			if (!isAppActive) {
				return;
			}

			if (cachedScreenTime == null) {
				cachedScreenTime = new List<CacheScreenTime>();
			}
			DateTime now = DateTime.Now;

			DateTime lastScreenTime = GetCacheData<DateTime>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);
			string lastScreen = GetCacheData<string>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN);
			string currentScreen = SceneManager.GetActiveScene().name;

			double timeToAdd = (lastScreen == currentScreen) ? now.Subtract(lastScreenTime).TotalSeconds : 0;

			bool found = false;
			for (int i = 0; i < cachedScreenTime.Count; i++) {
				if (cachedScreenTime[i].Key == currentScreen) {
					found = true;
					var newKvp = new CacheScreenTime(cachedScreenTime[i].Key, cachedScreenTime[i].Value + timeToAdd);
					cachedScreenTime.RemoveAt(i);
					cachedScreenTime.Insert(i, newKvp);
				}
			}

			if (!found) {
				var newKvp = new CacheScreenTime(currentScreen, timeToAdd);
				cachedScreenTime.Insert(cachedScreenTime.Count, newKvp);
			}

			SaveScreenCacheListData(cachedScreenTime, MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE);
			SaveCacheData<string>(currentScreen, MageEngineSettings.GAME_ENGINE_LAST_SCREEN);
			SaveCacheData<DateTime>(now, MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);
		}

		private void ResetScreenTime() {
			SaveCacheData<DateTime>(DateTime.Now, MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);
			isAppActive = true;
		}


		#endregion

		#region ApiCache
		private void InitApiCache() {
			this.apiCounter.Add("UpdateUserDataRequest", new OnlineCacheCounter(0, 10));
			this.apiCounter.Add("UpdateGameCharacterDataRequest", new OnlineCacheCounter(0, 10));
			this.apiCounter.Add("SendUserEventListRequest", new OnlineCacheCounter(0, 0));
			
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

		#endregion


		#region messages & notification 
		////<summary>Send push notification</summary>
		
		private List<Message> LoadUserMessages() {
			
			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_USER_MESSAGE)) {
				List<Message> t = ES2.LoadList<Message>(MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
				if (t == null) {
					t = new List<Message>();
				}
				this.cachedUserMessages = t;
				return t;
				
			} else {
				List<Message> t = new List<Message>();
				this.cachedUserMessages = t;
				return t;
			}
		}

		public void UpdateMessageStatus(string msgId, MessageStatus status) {
			bool found = false;
			for (int i = 0; i < this.cachedUserMessages.Count; i++) {
				if (this.cachedUserMessages[i].id == msgId) {
					cachedUserMessages[i].status = status;
					found = true;
					break;			
				}
			}

			SaveUserMessages();
			if (found ) {
				UpdateUserMessageStatusToServer(msgId, status);
			}
		}

		private void SaveUserMessages() {
			#if PLATFORM_TEST
				if (!this.resetUserDataOnStart) {
					ES2.Save(this.cachedUserMessages, MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
				}
			#else
				ES2.Save(this.cachedUserMessages, MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
			#endif
		}

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

		public List<Message> GetUserMessages() {
			return this.cachedUserMessages;
		}

		private void AddNewMessages(List<Message> newMessages) {
			//check and remove messages that in the local list
			List<Message> tmp = new List<Message>();
			for (int j = 0; j < newMessages.Count; j++) {
				bool found = false;
				for (int i = 0; i < this.cachedUserMessages.Count; i++) {
					if (this.cachedUserMessages[i].id == newMessages[j].id) {
						//get status from local
						newMessages[j].status = this.cachedUserMessages[i].status;
						found = true;
						break;			
					}
				}

				if (!found) {
					tmp.Add(newMessages[j]);
				} else {
					UpdateUserMessageStatusToServer(newMessages[j].id, newMessages[j].status);
				}
			}

			for (int i = 0; i < tmp.Count; i++) {
				this.cachedUserMessages.Add(tmp[i]);
			}

			SaveUserMessages();
		}

		
		protected virtual void OnHasNewUserMessagesCallback(List<Message> newMessages) {

		}

		
		public void SetupFirebaseMessaging() {
			Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
			Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
		}

		public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
			UnityEngine.Debug.Log("Received Registration Token: " + token.Token);

			User u = GetUser();
			if (u.notification_token != token.Token) {
				u.notification_token = token.Token;
				UpdateUserProfile(u);
			}
		}

		public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
			UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
		}

		#endregion

		#region event counter
		
		private void SaveEventCounterList(List<EventCounter> data) {
			#if PLATFORM_TEST
				if (!this.resetUserDataOnStart) {
					ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
				}
			#else
				ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
			#endif

			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, data);
		}

		private List<EventCounter> LoadEventCounterList() {
			
			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE)) {
				List<EventCounter> t = ES2.LoadList<EventCounter>(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
				if (t == null) {
					t = new List<EventCounter>();
				}
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, t);
				return t;
				
			} else {
				List<EventCounter> t = new List<EventCounter>();
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, t);
				return t;
			}
		}

		private List<EventCounter> GetEventCounterList() {
			return RuntimeParameters.GetInstance().GetParam<List<EventCounter>>(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
		}

		private void AddEventCounter(string eventName) {
			List<EventCounter> eventCounterList = GetEventCounterList();
			bool found = false;
			//search for eventName
			for (int i = 0; i < eventCounterList.Count; i++) {
				if (eventCounterList[i].Key == eventName) {
					found = true;
					var newKvp = new EventCounter(eventCounterList[i].Key, eventCounterList[i].Value++);
					eventCounterList.RemoveAt(i);
					eventCounterList.Insert(i, newKvp);
				}
			}

			if (!found) {
				var newKvp = new EventCounter(eventName, 1);
				eventCounterList.Insert(eventCounterList.Count, newKvp);
			}

			SaveEventCounterList(eventCounterList);
		}

		private string ConvertEventCounterListToJson() {
			List<EventCounter> eventCounterList = GetEventCounterList();

			if (null != eventCounterList) {
				string output = "{";
				for (int i = 0; i < eventCounterList.Count; i++) {
					output += "\"" + MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE_PREFIX + eventCounterList[i].Key + "\": " + eventCounterList[i].Value + ", ";
				}

				if (eventCounterList.Count > 0) {
					output = output.Substring(0, output.Length - 2);
				}

				output += "}";
				return output;
			} else {
				return "";
			}
		}

		#endregion
	}
}

