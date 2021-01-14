using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using MageApi.Models.Request;
using MageApi.Models.Response;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Application;
using Mage.Models;
using MageSDK.Client.Helper;
using System.Security.Cryptography;
using System.Text;
using MageSDK.Client.Adaptors;
using Mage.Models.Attributes;
using Mage.Models.Game;

namespace MageSDK.Client
{
    public class MageEngine : MonoBehaviour
    {

        #region public variables
        public static MageEngine instance;
        [Header("API connection details")]

        public string ApplicationSecretKey = "";
        public string ApplicationKey = "";
        public string ApiVersion = "";
        public string ApiUrl = "http://localhost:8080/portal/api";
        [Header("API testing details")]
        public bool TestMode;
        public string TestUUID;

        ///<summary>isLocalApplicationData is using to indicate where to get Application Data.</summary>
        // this is used to test application in Editor mode, if this is true then Application Data will be load from local resources
        [Header("Working mode")]
        public bool isLocalApplicationData = true;
        ///<summary>resetUserDataOnStart is used during test in Unity editor to reset user data whenever editor is running.</summary>
        // if this variable is false, then data will be save to local storage and server accordingly
        public bool resetUserDataOnStart = true;
        public bool isWorkingOnline = false;
        public ClientLoginMethod loginMethod;
        [Header("Firebase packages")]
        public bool useFirebaseAnalytic = false;
        public bool useFirebaseApplicationData = false;
        public bool useFirebaseStorage = false;
        public bool useFirebaseRealtimeDatabase = false;

        [Header("Android specific")]
        public string signatureHashAndroid = "";

        [HideInInspector]
        public Hashtable apiCounter = new Hashtable();
        [HideInInspector]
        public bool isAppActive = true;
        public TextAsset gameConfigText;
        #endregion

        #region private variables
        private bool _isLogin = false;
        private bool _isLoginRequestSent = false;
        private bool _isReloadRequired = false;
        private bool _isApplicationDataLoaded = false;
        private bool _isLoaded = false;
        private DateTime _lastUserDataUpdate = DateTime.Now;
        private bool _completedSignatureCheckForAndroid = false;
        //private bool _hasDataEncrypted = true;
        private string _encryptKey = "wgIl3ZjLdwYviMeTPo90QhVk1BHLA4YgWt5ES1avh8Lace8wqp5SfetYqRFJvg2xh6Kn1pIrHRyVBGCtgCSn3V8FbsVROZSosJEN5CcHQpX6Roj89TDk0sRYzxPKzbqjzPbjk7PxZhVYAO8vg6kFPF7px8Hwl5yDxElhwxRdlpvmU9La96qelkXyAuTK55JuYOvohN0zdEJp5MSlkfpYxAMcudeB7dxL973Y6B835RIKB8Yq7Usr0IaxvF8QostF";
        private float TimeOut = 200;
        public DateTime _startTime = DateTime.Now;
        public DateTime _lastTimeStampUpdate = DateTime.Now;
        private DateTime _lastEventSent = DateTime.Now;
        private List<Action> _onApplicationDataReloadActions = new List<Action>();
        private DateTime _lastApplicationDataFetched = DateTime.Now;
        private string _serverApplicationDataHash = "";
        private float _callbackQueueDeltaTime = 0;
        private bool _isCallbackQueueProcessing = false;
        private bool _isPollingAllowed = true;
        #endregion

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if (!_isLoaded)
            {
                ApiUtils.Log("[Time Checking]: Start loading: " + _startTime.ToString("dd-MM-yyyy HH:mm:ss"));
                ApiUtils.Log("[Time Checking]: Start loading after: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                // init api cache
                InitApiCache();

                ApiHandler.GetInstance().Initialize();
                // Call to override load function
                Load();

                // get application data from server
                GetApplicationData();

                // at start initiate Default user
                InitDefaultUser();

                // load engine cache data from local storage
                LoadEngineCache();

                _isLoaded = true;
            }
        }

        void Update()
        {

            if (_callbackQueueDeltaTime > 1.0f)
            {
                if (!_isCallbackQueueProcessing)
                {
                    StartCoroutine(ProcessCallbackQueue());
                }
                _callbackQueueDeltaTime = 0;
            }
            else
            {
                _callbackQueueDeltaTime += Time.deltaTime;
            }

        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                SceneTrackerHelper.GetInstance().ResetScreenTime();
            }
            else
            {
                isAppActive = false;
            }
        }

        public void DoLogin()
        {
            ApiUtils.Log("[Time Checking]: Login started at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
            // if there is one login request sent, then wait for that request completes
            if (this._isLoginRequestSent)
            {
                return;
            }
            this._isLoginRequestSent = true;

            if (this.resetUserDataOnStart || !this.isWorkingOnline)
            {
                // in unity and test mode don't reuse user saved previously, always initate new user
                _isLogin = true;
                //test callback
                OnLoginCompleteCallback();
            }
            else
            {
                // login user during start
                if (loginMethod == ClientLoginMethod.LOGIN_DEVICE_UUID)
                {
                    LoginWithDeviceID();
                }
            }
        }

        ///<summary>Other implementation will override this function</summary>
        protected virtual void Load()
        {
        }

        public bool IsLoaded()
        {
            return this._isLoaded;
        }

        // Test callback first, needs to implement event handler
        protected virtual void OnLoginCompleteCallback()
        {

        }

        #region Login & user handling
        ///<summary>Login using device id</summary>
        public void LoginWithDeviceID()
        {
            StartCoroutine(MageAdaptor.LoginWithDeviceID(OnCompleteMageLogin, null, TimeoutHandler));
#if USE_FIREBASE
            // also login to firebase, if the application uses Firebase analytic
            if (this.useFirebaseAnalytic || this.useFirebaseApplicationData)
            {
                StartCoroutine(FirebaseAdaptor.LoginAnonymous((x) =>
                {
                    ApiUtils.Log("Firebase Login: " + x.UserId);
                    ApiUtils.Log("[Time Checking]: Firebase Login started at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                    FirebaseAdaptor.InitializeFirebaseStacks();
                }));
            }
#endif
        }

        ///<summary>Init default user, based on input data from Unity</summary>
        private void InitDefaultUser()
        {
            ApiUtils.Log("Load user from application data: ");
            // initiate default user
            User defaultUser = GetApplicationDataItem<User>(MageEngineSettings.GAME_ENGINE_DEFAULT_USER_DATA);
            defaultUser.id = "0";

            // update user to game engine
            if (resetUserDataOnStart)
            {
                // in unity and test mode don't reuse user saved previously, always initate new user
                SetUser(defaultUser);
            }
            else
            {
                // if not in test mode, check the user saved in local and use the saved one
                // if there is no user saved locally, then initate a default user
                if (ES2.Exists(MageEngineSettings.GAME_ENGINE_USER))
                {
                    byte[] rawData = ES2.LoadRaw(MageEngineSettings.GAME_ENGINE_USER);
                    string encryptedUser = Encoding.UTF8.GetString(rawData, 0, rawData.Length);
                    SetUser(User.CreatFromEncryptJson<User>(encryptedUser, this._encryptKey));
                }
                else
                {
                    SetUser(defaultUser);
                }
            }

            ApiUtils.Log("User data after default load: " + GetUser().ToJson());
        }

        ///<summary>On completed logged in</summary>
        private void OnCompleteMageLogin(LoginResponse result)
        {
            ApiUtils.Log("[Time Checking]: On completed login called at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
            //update information after login
            _isLogin = true;
            RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
            RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);
            this.OnEvent("Login_" + result.User.user_ages.ToString("000"), "", result.User.user_ages.ToString());
            if (resetUserDataOnStart)
            {
                // in unity and test mode don't reuse user saved previously, always initate new user
                CreateNewUser(result.User);
            }
            else
            {
                // handle first time login to system
                if (result.User.status == UserStatus.FIRST_LOGIN)
                {
                    CreateNewUser(result.User);
                }
                else
                {
                    CreateExistingUser(result.User);
                }

                //assign new messages
                if (result.UserMessages != null && result.UserMessages.Count > 0)
                {
                    MessageHelper.GetInstance().AddNewMessages(result.UserMessages, this.UpdateUserMessageStatusToServer);
                    OnHasNewUserMessagesCallback(result.UserMessages);
                }
            }
            ApiUtils.Log("[Time Checking]: Login completed at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
            //pass the call to UI level to continue process data
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
        private void CreateNewUser(User u)
        {
            User tmp = GetUser();
            //in case new user, then update with server information
            tmp.id = u.id;
            if ("" != u.notification_token && tmp.notification_token != u.notification_token)
            {
                tmp.notification_token = u.notification_token;
            }

            // refresh status with from server
            tmp.status = u.status;
            tmp.is_test_user = u.is_test_user;

            SetUser(tmp);
        }

        void CreateExistingUser(User u)
        {
            User tmp = GetUser();

            //if (tmp.id == "0") {
            tmp.id = u.id;
            tmp.last_run_app_version = u.last_run_app_version;
            // refresh status with from server
            tmp.status = u.status;
            //}

            ApiUtils.Log("Local version: " + tmp.GetUserDataInt(UserBasicData.Version) + " server version; " + u.GetUserDataInt(UserBasicData.Version));

            // check and swap version
            if (tmp.GetUserDataInt(UserBasicData.Version) >= u.GetUserDataInt(UserBasicData.Version))
            {
                tmp.is_test_user = u.is_test_user;
                // in case local is newer, then it requires to update server with local
                SetUser(tmp);
                // save user datas to server
                SaveUserDataToServer(tmp);
            }
            else
            {
                // in case data from server is newer, then replace local by copy from server
                SetUser(u);
                _isReloadRequired = true;
            }

        }

        ///<summary>Update user data to current user. Once complete, save data to cache</summary>
        public void UpdateUserData(UserData data, bool forceUpdate = false)
        {
            User u = GetUser();

            // if user is not setup then don't update anything
            if (null == u)
            {
                return;
            }
            // check to increase version
            int currentVersion = GetUser().GetUserDataInt(UserBasicData.Version);
            if (data.attr_name != UserBasicData.Version.ToString())
            {
                UserData newVersion = new UserData(UserBasicData.Version.ToString(), "" + (currentVersion + 1), "MageEngine");
                UpdateUserData(new List<UserData>() { data, newVersion }, forceUpdate);

            }
            else
            {
                UpdateUserData(new List<UserData>() { data }, forceUpdate);
            }

        }

        ///<summary>Update user data to current user. Once complete, save data to cache</summary>
        public void UpdateUserData<T>(T obj, bool forceUpdate = false) where T : BaseModel
        {
            string className = typeof(T).Name;
            UserData d = new UserData()
            {
                attr_name = MageEngine.instance.ApplicationKey + "_" + className,
                attr_value = obj.ToJson(),
                attr_type = className
            };

            // do background enrich data first
            BackgroundEnrichData(MageAttributeHelper.ExtractUserDataFields<T>(obj));
            UpdateUserData(d, forceUpdate);
        }

        public void GetUserData<T>(string[] userIdList, Action<List<KeyValuePair<string, T>>> onCompleteCallback) where T : BaseModel
        {
            string dataName = MageEngine.instance.ApplicationKey + "_" + typeof(T).Name;
            MageAdaptor.GetUserDataByIds(userIdList, dataName,
                (listData) =>
                {
                    List<KeyValuePair<string, T>> result = new List<KeyValuePair<string, T>>();
                    foreach (UserData d in listData)
                    {
                        KeyValuePair<string, T> tmp = new KeyValuePair<string, T>(d.user_id, BaseModel.CreateFromJSON<T>(d.attr_value));
                        result.Add(tmp);
                    }
                    onCompleteCallback(result);
                }
            );
        }

        public void GetUserData<T>(string userId, Action<T> onCompleteCallback) where T : BaseModel
        {
            string dataName = MageEngine.instance.ApplicationKey + "_" + typeof(T).Name;
            MageAdaptor.GetUserDataByIds(new string[] { userId }, dataName,
                (listData) =>
                {
                    if (listData.Count > 0)
                    {
                        foreach (UserData d in listData)
                        {
                            // get first item only
                            onCompleteCallback(BaseModel.CreateFromJSON<T>(d.attr_value));
                            break;
                        }

                    }
                    else
                    {
                        onCompleteCallback(null);
                    }

                }
            );
        }

        public void GetUserData(string[] userIdList, string dataName, Action<List<UserData>> onCompleteCallback)
        {
            MageAdaptor.GetUserDataByIds(userIdList, dataName, onCompleteCallback);
        }

        public void GetUserData(string userId, string dataName, Action<string> onCompleteCallback)
        {
            MageAdaptor.GetUserDataByIds(new string[] { userId }, dataName,
                (listData) =>
                {
                    if (listData.Count > 0)
                    {
                        foreach (UserData d in listData)
                        {
                            // get first item only
                            onCompleteCallback(d.attr_value);
                            break;
                        }
                    }
                    else
                    {
                        onCompleteCallback("");
                    }

                });
        }

        private void BackgroundEnrichData(List<UserData> userDatas)
        {
            User u = GetUser();
            if (null != u && null != userDatas && userDatas.Count > 0)
            {
                foreach (UserData d in userDatas)
                {
                    u.SetUserData(d);
                }
            }
        }

        ///<summary>Update user data to current user. Once complete, save data to cache</summary>
        private void UpdateUserData(List<UserData> userDatas, bool forceUpdate = false)
        {
            // check if userDatas is valid
            if (null == userDatas || userDatas.Count == 0)
            {
                return;
            }

            User u = GetUser();
            foreach (UserData d in userDatas)
            {
                u.SetUserData(d);
            }

            // enrich system auto data
            u.SetUserData(new UserData(MageEngine.instance.ApplicationKey + "_" + MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE,
                                        SceneTrackerHelper.GetInstance().ConvertCacheScreenJson(), MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE)
                        );

            // enrich user event
            u.SetUserData(new UserData(MageEngine.instance.ApplicationKey + "_" + MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE,
                                        MageEventHelper.GetInstance().ConvertEventCounterListToJson(),
                                        MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE)
                        );

            this.SaveUserDataToCache(u);

            // for the first 50 changes, always sends to server
            if (GetUser().GetUserDataInt(UserBasicData.Version) <= 50)
            {
                forceUpdate = true;
            }

            SaveUserDataToServer(u, forceUpdate);
        }

        private void SaveUserDataToServer(User u, bool forceUpdate = false)
        {

            if (this.isWorkingOnline && IsLogin() && (this.IsSendable("UpdateUserDataRequest") || forceUpdate))
            {
                /* decided when to send data */
                DateTime now = DateTime.Now;
                double timeToAdd = now.Subtract(this._lastUserDataUpdate).TotalSeconds;

                /* for this we only send if the last update is more than X seconds ago */
                if (timeToAdd > GetApplicationDataItemInt(MageEngineSettings.GAME_ENGINE_MIN_USER_DATA_UPDATE_DURATION))
                {
                    // save data to server
                    UpdateUserDataRequest r = new UpdateUserDataRequest();
                    r.UserDatas = u.user_datas;
                    //call to update user data api
                    this.SendApi<UpdateUserDataResponse>(
                        ApiSettings.API_UPDATE_USER_DATA,
                        r,
                        (result) =>
                        {
                            ApiUtils.Log("Success: Update user data");
                            this.SaveUserDataToCache(u);
                            //clear counter cache
                            ResetSendable("UpdateUserDataRequest");

                        },
                        (errorStatus) =>
                        {
                            ApiUtils.Log("Error: " + errorStatus);
                            //do some other processing here
                        },
                        () =>
                        {
                            TimeoutHandler();
                        }
                    );
                    this._lastUserDataUpdate = now;
                }
            }

#if USE_FIREBASE
            // save user properties to firebase
            if (this.isWorkingOnline && IsLogin() && useFirebaseAnalytic)
            {
                foreach (UserData d in u.user_datas)
                {
                    if (d.attr_type == FBUserPropertyAttribute.target)
                    {
                        FirebaseAdaptor.UpdateUserProperty(d);
                    }
                }
            }
#endif
        }

        ///<summary>Update user data to current user. Once complete, save data to cache</summary>
        public T GetUserData<T>() where T : BaseModel
        {
            string key = MageEngine.instance.ApplicationKey + "_" + typeof(T).Name;

            if (null != GetUser() && "" != GetUser().GetUserData(key))
            {
                return BaseModel.CreateFromJSON<T>(GetUser().GetUserData(key));
            }
            else
            {
                return null;
            }

        }

        ///<summary>Update user profile</summary>
        public void UpdateUserProfile(User user)
        {
            // check if userDatas is valid
            if (null == user)
            {
                return;
            }

            User u = GetUser();

            // to be updated
            if ("" != user.fullname && user.fullname != u.fullname)
            {
                u.fullname = user.fullname;
            }

            if ("" != user.phone && user.phone != u.phone)
            {
                u.phone = user.phone;
            }

            if ("" != user.email && user.email != u.email)
            {
                u.email = user.email;
            }

            if ("" != user.avatar && user.avatar != u.avatar)
            {
                u.avatar = user.avatar;
            }

            if ("" != user.notification_token && user.notification_token != u.notification_token)
            {
                u.notification_token = user.notification_token;
            }

            UpdateUserProfileToServer(u);

        }

        ///<summary>Upload file to server and get back the url</summary>
        private void UpdateUserProfileToServer(User u)
        {
            this.SaveUserDataToCache(u);

            // user must logged in
            if (this.isWorkingOnline && IsLogin())
            {
                // save data to server
                UpdateProfileRequest r = new UpdateProfileRequest(u.fullname, u.phone, u.email, u.avatar, u.notification_token);

                //call to update user data api
                this.SendApi<UpdateProfileResponse>(
                    ApiSettings.API_UPDATE_PROFILE,
                    r,
                    (result) =>
                    {
                        ApiUtils.Log("Success: Update user profile");
                    },
                    (errorStatus) =>
                    {
                        ApiUtils.Log("Error: " + errorStatus);
                        //do some other processing here
                    },
                    () =>
                    {
                        TimeoutHandler();
                    }
                );
            }
        }

        ///<summary>Upload file to server and get back the url</summary>
        public string UploadFile(string file)
        {
            UploadFileRequest r = new UploadFileRequest();

            string imagePath = Application.dataPath + file;

            r.SetUploadFile(File.ReadAllBytes(imagePath));

            string output = "";

            //call to login api
            this.UploadFile<UploadFileResponse>(
                r,
                (result) =>
                {
                    ApiUtils.Log("Success: Upload file successfully");
                    ApiUtils.Log("Upload URL: " + result.UploadedURL);
                    output = result.UploadedURL;
                },
                (errorStatus) =>
                {
                    ApiUtils.Log("Error: " + errorStatus);
                    //do some other processing here
                },
                () =>
                {
                    //timeout handler here
                    ApiUtils.Log("Api call is timeout");
                }
            );

            return output;
        }

        #endregion



        #region API Error & Exception handler
        ///<summary>Default timeout handler declare here, client implemnetation can overwrite this</summary>
        public void TimeoutHandler()
        {
            //timeout handler here
            ApiUtils.Log("Api call is timeout");
        }

        public void ApiErrorHandler()
        {
            //timeout handler here
            ApiUtils.Log("Api error handler");
        }
        #endregion

        #region Get & Set information
        public bool IsLogin()
        {
            return _isLogin;
        }

        public bool IsReloadRequired()
        {
            return _isReloadRequired;
        }
        public User GetUser()
        {
            return RuntimeParameters.GetInstance().GetParam<User>(MageEngineSettings.GAME_ENGINE_USER);
        }

        private void SetUser(User u)
        {
            RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER, u);
            this.SaveUserDataToCache(u);
        }

        private void SaveUserDataToCache(User u)
        {
            if (!this.resetUserDataOnStart)
            {
                ES2.SaveRaw(u.ToEncryptedJson(this._encryptKey), MageEngineSettings.GAME_ENGINE_USER);
            }
        }


        #endregion

        #region Application Data manipulation
        ///<summary>Get Application Data configured in server to start the application</summary>
        public void GetApplicationData()
        {
            // Load application data
            if (isLocalApplicationData)
            {
                // in unity and test mode application data can be retrieved from local Resources
                LoadApplicationDataFromResources();
                // if user local data then mark application data as loaded
                this._isApplicationDataLoaded = true;
            }
            else
            {
                // load from local first and then overwrite with value from server
                ApiUtils.Log("[Time Checking]: Start loading local resources at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                LoadApplicationDataFromResources();
                ApiUtils.Log("[Time Checking]: Loading local resources completed at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                // load application data from server
                ApiUtils.Log("[Time Checking]: Start loading server resources at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                EnqueueCallbackTask((Action)this.LoadApplicationDataFromServerDuringStart);
            }

        }
        public void LoadApplicationDataFromServerDuringStart()
        {
            StartCoroutine(LoadApplicationDataFromServer(false));
        }

        ///<summary>Get Application Data configured in Resources/Data</summary>
        private void LoadApplicationDataFromResources()
        {
            List<ApplicationData> localResources = new List<ApplicationData>();
            // load latest first
            if (ES2.Exists(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA))
            {
                ApiUtils.Log("[Time Checking]: Loading local resources from ES2 at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                localResources = ES2.LoadList<ApplicationData>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);

                ApiUtils.Log("[Time Checking]: Local resources from ES2 at: " + DateTime.Now.Subtract(_startTime).TotalSeconds + " content: " + localResources.Count);
            }

            // if from ES2 doesn't exist
            if (localResources == null || localResources.Count == 0)
            {
                foreach (string data in MageEngineSettings.GAME_ENGINE_APPLICATION_DATA_ITEM)
                {
                    try
                    {

                        var jsonTextFile = Resources.Load<TextAsset>("Data/" + data);
                        ApiUtils.Log("Load data: " + data + jsonTextFile.text);
                        if (jsonTextFile != null)
                        {

                            localResources.Add(new ApplicationData()
                            {
                                attr_name = data,
                                attr_value = jsonTextFile.text
                            });
                        }
                        else
                        {
                            ApiUtils.Log("Failed to load resource file: " + data);
                        }
                    }
                    catch (Exception e)
                    {
                        ApiUtils.Log("Failed to load resource file: " + data);
                    }

                }
            }

            RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, localResources);
            // default application data is loaded after local resource is loaded.

            this._isApplicationDataLoaded = true;
        }

        ///<summary>Get Application Data configured in Server</summary>
        public IEnumerator LoadApplicationDataFromServer(bool isQueueTask = true)
        {
            yield return null;
#if USE_FIREBASE
            if (useFirebaseApplicationData)
            {
                StartCoroutine(FirebaseAdaptor.GetApplicationDataFromServer(MergeApplicationDataFromServer, isQueueTask));
            }
            else
            {
#endif
                MageAdaptor.GetApplicationDataFromServer(MergeApplicationDataFromServer, null, TimeoutHandler, isQueueTask);
#if USE_FIREBASE
            }
#endif
        }

        ///<summary>After get Application data, it needs to merge with data from local</summary>
        private void MergeApplicationDataFromServer(List<ApplicationData> serverList)
        {
            string serverHash = this.GetServerApplicationDataHash(serverList);
            this._isApplicationDataLoaded = true;

            if (this._serverApplicationDataHash == serverHash)
            {
                return;
            }

            this._serverApplicationDataHash = serverHash;

            List<ApplicationData> localList = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);

            if (null == localList)
            {
                // store application data
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, serverList);
                ES2.Save(serverList, MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
            }
            else
            {
                List<ApplicationData> mergedList = serverList;
                foreach (ApplicationData data in localList)
                {
                    bool found = false;
                    foreach (ApplicationData check in serverList)
                    {
                        if (data.attr_name == check.attr_name && (check.attr_type == "" || data.attr_type == check.attr_type))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        mergedList.Add(data);
                    }
                }

                // save merged list
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA, mergedList);
                ES2.Save(mergedList, MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
            }
            ApiUtils.Log("[Time Checking]: Loading server resources completed at: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
            // refresh Event cache counter
            RefereshEventCacheCounter();

            // incase of other actions is required
            TriggerApplicationDataReload();
        }

        private string GetServerApplicationDataHash(List<ApplicationData> serverList)
        {
            string hash = "";
            foreach (ApplicationData data in serverList)
            {
                hash = ApiUtils.GetInstance().Md5Sum(data.attr_name + "@" + data.attr_value + "@" + hash);
            }
            return hash;
        }
        public void AddApplicationDataReload(Action action)
        {
            this._onApplicationDataReloadActions.Add(action);
        }

        private void TriggerApplicationDataReload()
        {
            foreach (Action actionItem in this._onApplicationDataReloadActions)
            {
                this.EnqueueCallbackTask(actionItem);
            }
        }

        public bool IsApplicationDataLoaded()
        {
            return this._isApplicationDataLoaded;
        }

        ///<summary>Get Application Data item store locally - value as string</summary>
        public string GetApplicationDataItem(string attributeName, string attributeType = "")
        {
            List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
            if (null != applicationDatas || applicationDatas.Count != 0)
            {
                foreach (ApplicationData data in applicationDatas)
                {
                    if (data.attr_name == attributeName && (attributeType == "" || data.attr_type == attributeType))
                    {
                        return data.attr_value;
                    }
                }
            }

            // if not exist then load from local resource file
            var jsonTextFile = Resources.Load<TextAsset>("Data/" + attributeName);
            //ApiUtils.Log("Load data: " + data + jsonTextFile.text);
            if (jsonTextFile != null)
            {
                return jsonTextFile.text;
            }

            return "";
        }

        public int GetApplicationDataItemInt(string attributeName, string attributeType = "")
        {
            List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
            if (null != applicationDatas)
            {
                foreach (ApplicationData data in applicationDatas)
                {
                    if (data.attr_name == attributeName && (attributeType == "" || data.attr_type == attributeType))
                    {
                        return int.Parse(data.attr_value);
                    }
                }
            }
            return 0;
        }

        ///<summary>Get Application Data item store locally - value as object</summary>
        public T GetApplicationDataItem<T>(string attributeName, string attributeType = "") where T : BaseModel
        {
            List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
            if (null != applicationDatas)
            {
                foreach (ApplicationData data in applicationDatas)
                {
                    if (data.attr_name == attributeName && (attributeType == "" || data.attr_type == attributeType))
                    {
                        return BaseModel.CreateFromJSON<T>(data.attr_value);
                    }
                }
            }

            ApiUtils.Log("No default user information");
            return default(T);
        }

        ///<summary>Get Application Data item store locally - value as list object</summary>
        public List<T> GetApplicationDataByType<T>(string attributeType) where T : BaseModel
        {
            List<ApplicationData> applicationDatas = RuntimeParameters.GetInstance().GetParam<List<ApplicationData>>(MageEngineSettings.GAME_ENGINE_APPLICATION_DATA);
            List<T> result = new List<T>();
            if (null != applicationDatas)
            {
                foreach (ApplicationData data in applicationDatas)
                {
                    if (data.attr_type == attributeType)
                    {
                        T i = BaseModel.CreateFromJSON<T>(data.attr_value);
                        if (null != i)
                        {
                            result.Add(i);
                        }
                    }
                }
            }
            return result;
        }

        ///<summary>Save Application Data to server</summary>
        public void AdminSaveApplicationDataToServer(List<ApplicationData> applicationDatas, string unityAdminToken, Action successCallback = null)
        {
            MageAdaptor.AdminSaveApplicationDataToServer(applicationDatas, unityAdminToken, successCallback, (x) => { }, TimeoutHandler);
        }

        #endregion

        #region Event 
        ///<summary>Save User Event to Mage Servers</summary>
        private void SendUserEventsToMage()
        {

            if (this.resetUserDataOnStart || !this.isWorkingOnline)
            {
                return;
            }

            //ApiUtils.Log(" Queue size: " + ((OnlineCacheCounter)this.apiCounter["SendUserEventListRequest"]).GetMax());
            User u = GetUser();
            /// if the size of user events queue reachs limit

            if (IsLogin() && (this.IsSendable("SendUserEventListRequest") || u.GetUserDataInt(UserBasicData.Version) < 100))
            {
                /* decided when to send data */
                DateTime now = DateTime.Now;
                double timeToAdd = now.Subtract(this._lastEventSent).TotalSeconds;

                /* for this we only send if the last update is more than X seconds ago */
                if (timeToAdd > GetApplicationDataItemInt(MageEngineSettings.GAME_ENGINE_MIN_USER_DATA_UPDATE_DURATION))
                {
                    ApiUtils.Log("Event can be sent");
                    List<MageEvent> cachedEvent = MageEventHelper.GetInstance().GetMageEventsList();

                    ApiUtils.Log("Clear cache log");
                    ClearCachedEvent();

                    ApiUtils.Log("Size of Event queue: " + cachedEvent.Count);
                    if (cachedEvent.Count > 1)
                    {
                        MageAdaptor.SendUserEventList(cachedEvent, () => { }, (x) => { }, TimeoutHandler);
                    }
                    else
                    {
                        MageEvent t = cachedEvent[0];
                        SendUserEventRequest r = new SendUserEventRequest(t.eventName, t.eventDetail);
                        //call to login api
                        MageAdaptor.SendUserEvent(t, () => { }, (x) => { }, TimeoutHandler);
                    }
                    this._lastEventSent = now;
                }
            }
        }

        ///<summary>Clear local event cache</summary>
        private void ClearCachedEvent()
        {
            List<MageEvent> cachedEvent = new List<MageEvent>();
            MageEventHelper.GetInstance().SaveMageEventsList(cachedEvent);
        }

        ///<summary>Capture app event</summary>
        public void OnEvent(MageEventType type)
        {
            ApiUtils.Log("OnEvent: " + type);
            MageEventHelper.GetInstance().OnEvent(type, this.SendUserEventsToMage, "");
#if USE_FIREBASE
            // send event to firebase
            if (useFirebaseAnalytic)
            {
                FirebaseAdaptor.OnEvent(type, "", "");
            }
#endif
        }

        public void OnEvent(MageEventType type, string eventDetail)
        {
            ApiUtils.Log("OnEvent: " + type);
            MageEventHelper.GetInstance().OnEvent(type, this.SendUserEventsToMage, eventDetail, "");
#if USE_FIREBASE
            // send event to firebase
            if (useFirebaseAnalytic)
            {
                FirebaseAdaptor.OnEvent(type, eventDetail, "");
            }
#endif

        }

        public void OnEvent(MageEventType type, string eventDetail, string eventValue)
        {
            ApiUtils.Log("OnEvent: " + type);
            MageEventHelper.GetInstance().OnEvent(type, this.SendUserEventsToMage, eventDetail, eventValue);
#if USE_FIREBASE
            // send event to firebase
            if (useFirebaseAnalytic)
            {
                FirebaseAdaptor.OnEvent(type, eventDetail, eventValue);
            }
#endif
        }

        public void OnEvent(string type, string eventDetail, string eventValue)
        {
            ApiUtils.Log("OnEvent: " + type);
            MageEventHelper.GetInstance().OnEvent(type, this.SendUserEventsToMage, eventDetail, eventValue);
#if USE_FIREBASE
            // send event to firebase
            if (useFirebaseAnalytic)
            {
                FirebaseAdaptor.OnEvent(type, eventDetail, eventValue);
            }
#endif
        }

        public void OnEvent(MageEventType type, string eventDetail, int eventValue)
        {
            ApiUtils.Log("OnEvent: " + type);
            MageEventHelper.GetInstance().OnEvent(type, this.SendUserEventsToMage, eventDetail, "" + eventValue);
#if USE_FIREBASE
            // send event to firebase
            if (useFirebaseAnalytic)
            {
                FirebaseAdaptor.OnEvent(type, eventDetail, eventValue);
            }
#endif
        }

        ///<summary>Capture app event</summary>
        public void OnEvent<T>(MageEventType type, T obj) where T : BaseModel
        {
            ApiUtils.Log("OnEvent: " + type);
            MageEventHelper.GetInstance().OnEvent(type, obj, this.SendUserEventsToMage);
#if USE_FIREBASE
            // send event to firebase
            if (useFirebaseAnalytic)
            {
                FirebaseAdaptor.OnEvent(type, obj.ToJson(), "");
            }
#endif
        }


        ///<summary>Load local engine cache</summary>
        private void LoadEngineCache()
        {
            // load screen cache
            SceneTrackerHelper.GetInstance().LoadSceneCacheListData();

            // Load User messages
            MessageHelper.GetInstance().LoadUserMessages();

            // load event counter
            MageEventHelper.GetInstance().LoadMageEventData();
        }


        #endregion

        #region  Action Logs

        public void LogAction<T>(T actionData) where T : BaseModel
        {
            MageLogHelper.GetInstance().LogAction<T>(actionData);

            SendActionLogs<T>();
        }
        private void SendActionLogs<T>() where T : BaseModel
        {

            if (this.resetUserDataOnStart || !this.isWorkingOnline)
            {
                return;
            }

            if (this.IsSendable("SendGameUserActionLogRequest" + MageLogHelper.GetInstance().LoggerID<T>()))
            {
                SendGameUserActionLogRequest r = new SendGameUserActionLogRequest(MageLogHelper.GetInstance().GetLogger<T>());

                //call to send action log api
                this.SendApi<SendGameActionLogResponse>(
                    ApiSettings.API_SEND_GAME_USER_ACTION_LOG,
                    r,
                    (result) =>
                    {
                        MageLogHelper.GetInstance().ClearLogger<T>();
                    },
                    (errorStatus) =>
                    {
                        //ApiUtils.Log("Error: " + errorStatus);
                        //do some other processing here
                    },
                    () =>
                    {
                        TimeoutHandler();
                    }
                );
            }

        }

        #endregion

        #region ApiCache

        private void InitApiCache()
        {
            this.apiCounter.Add("UpdateUserDataRequest", new OnlineCacheCounter(0, 0));
            this.apiCounter.Add("UpdateGameCharacterDataRequest", new OnlineCacheCounter(0, 10));
            this.apiCounter.Add("SendUserEventListRequest", new OnlineCacheCounter(0, 0));
        }

        private void RefereshEventCacheCounter()
        {
            int maxEventQueue = GetApplicationDataItemInt(MageEngineSettings.GAME_ENGINE_MAX_EVENT_COUNTER_QUEUE);
            if (maxEventQueue > 0)
            {
                // reset the current
                if (apiCounter.Contains("SendUserEventListRequest"))
                {
                    OnlineCacheCounter x = (OnlineCacheCounter)apiCounter["SendUserEventListRequest"];
                    if (x != null)
                    {
                        x.SetMax(maxEventQueue);
                        apiCounter["SendUserEventListRequest"] = x;
                    }
                }
            }
        }

        private bool IsSendable(string apiName)
        {
            if (this.apiCounter.Contains(apiName))
            {
                OnlineCacheCounter tmp = (OnlineCacheCounter)this.apiCounter[apiName];
                bool check = tmp.IsMax();
                this.apiCounter[apiName] = tmp;
                return check;
            }
            else
            {
                return true;
            }
        }

        private void ResetSendable(string apiName)
        {
            if (this.apiCounter.Contains(apiName))
            {
                OnlineCacheCounter tmp = (OnlineCacheCounter)this.apiCounter[apiName];
                tmp.ResetCounter();
                this.apiCounter[apiName] = tmp;
            }
        }

        public DateTime GetServerTimeStamp()
        {
            if (MageEngine.instance.isWorkingOnline)
            {
                DateTime serverTime = RuntimeParameters.GetInstance().GetParam<DateTime>(ApiSettings.API_SERVER_TIMESTAMP);
                DateTime localTime = DateTime.Now;

                if (Math.Abs(this._lastTimeStampUpdate.Subtract(localTime).TotalSeconds) >= 120)
                {
                    MageAdaptor.GetServerTimestamp();
                    this._lastTimeStampUpdate = DateTime.Now;
                }

                if (serverTime == default(DateTime) || Math.Abs(serverTime.Subtract(localTime).TotalSeconds) <= 600)
                {
                    return localTime;
                }
                else
                {
                    return serverTime;
                }
            }

            else
                return System.DateTime.Now;
        }

        #endregion


        #region messages & notification 

        public void SendMessage(string userId, MessageType messageType = MessageType.PushNotification, string title = "", string messageBody = "", string additionalData = "")
        {
            MageAdaptor.SendMessage(userId, messageType, title, messageBody, additionalData);

        }
        private void UpdateUserMessageStatusToServer(string msgId, MessageStatus status)
        {
            if (this.isWorkingOnline)
            {
                MageAdaptor.UpdateUserMessageStatusToServer(msgId, status);
            }
        }


        protected virtual void OnHasNewUserMessagesCallback(List<Message> newMessages)
        {

        }

#if USE_FIREBASE
        protected virtual void OnNewFirebaseMessageCallback(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {

        }


        public void SetupFirebaseMessaging()
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
        {
            ApiUtils.Log("Received Registration Token: " + token.Token);

            User u = GetUser();
            if (u.notification_token != token.Token)
            {
                u.notification_token = token.Token;
                UpdateUserProfile(u);
            }
        }

        public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            ApiUtils.Log("Received a new message from: " + e.Message.From);
            OnNewFirebaseMessageCallback(sender, e);
        }
#endif

        public void UpdateMessageStatus(string msgId, MessageStatus status)
        {
            MessageHelper.GetInstance().UpdateMessageStatus(msgId, status, this.UpdateUserMessageStatusToServer);
        }


        #endregion

        #region friends

        public void GetRandomFriend(Action<User> getRandomFriendCallback, string friendId = "")
        {
            MageAdaptor.GetRandomFriend(getRandomFriendCallback, friendId);
        }

        #endregion


        #region security check
        private byte[] GetSignatureHash()
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


        private byte[] GetByteArrayFromJava(AndroidJavaObject obj)
        {
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

        private bool CheckApplicationSignature()
        {
            string enforedAndroidSignature = GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_ENFORECED_ANDROID_SIGNATURE);

            // if not enforced from server then always return true
            if (enforedAndroidSignature != "true")
            {
                return true;
            }

            // if enforeced is set, then check signature
            string signatureFromServer = GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_ANDROID_SIGNATURE_SHA1);
            if (signatureFromServer != "")
            {
                this.signatureHashAndroid = signatureFromServer;
            }

            return (string.Compare(this.Sha1HashFile(this.GetSignatureHash()), this.signatureHashAndroid) == 0);
        }
        #endregion

        #region File upload
        public void UploadAvatar(Texture2D image, Action<string> onUploadCompleteCallback = null)
        {
            ApiUtils.Log("Upload avatar");
#if USE_FIREBASE
            if (this.useFirebaseStorage)
            {
                string targetFilename = GetUser().id + "_" + DateTime.Now.ToString("YYYYMMDDhhmmss") + ".png";
                FirebaseAdaptor.UploadImage(image, targetFilename, onUploadCompleteCallback);
            }
            else
            {
#endif
                MageAdaptor.UploadImage(image, onUploadCompleteCallback);
#if USE_FIREBASE
            }
#endif
        }

        public void UpdateUserAvatar(Texture2D image, string avatarUrl, Action<string> otherCallback)
        {
            User u = GetUser();
            u.avatar = avatarUrl;
            string[] keys = avatarUrl.Split('/');
            string path = keys[keys.Length - 1];
            ES2.SaveImage(image, path);
            UpdateUserProfile(u);

            if (otherCallback != null)
            {
                otherCallback(avatarUrl);
            }
        }


        IEnumerator LoadImageCoroutine(string avatarUrl, Action<Texture2D> onLoadCompleteCallback)
        {
            string[] keys = avatarUrl.Split('/');
            string path = keys[keys.Length - 1];
            Texture2D tex = new Texture2D(128, 128, TextureFormat.ARGB32, false);
            if (ES3.FileExists(path))
            {
                tex = ES3.LoadImage(path);
            }
            else
            {
                WWW url = new WWW(avatarUrl);
                ApiUtils.Log("start Download");
                yield return url;
                url.LoadImageIntoTexture(tex);
                ApiUtils.Log("downloaded");
                ES3.SaveImage(tex, path);
                ApiUtils.Log("saved");
            }

            if (tex != null)
            {
                onLoadCompleteCallback(tex);
            }
        }

        public void LoadImage(string avatarUrl, Action<Texture2D> onLoadCompleteCallback)
        {
            if (avatarUrl != "")
            {
                StartCoroutine(LoadImageCoroutine(avatarUrl, onLoadCompleteCallback));
            }

        }

        public void LoadAvatar(Action<Texture2D> onLoadCompleteCallback)
        {
            LoadImage(GetUser().avatar, onLoadCompleteCallback);
        }
        #endregion


        #region Leaderboard

        public void GetLeaderBoardFromObject(object t, string fieldName, Action<List<LeaderBoardItem>> onCompleteCallback = null, int index = -1)
        {
            // Firebase leaderboard is not being used, so always route to Mage Server
            string name = ExtractFieldAttribute.fieldPrefix + t.GetType().Name + "_" + fieldName + (index >= 0 ? "_" + index : "");
            if (null != onCompleteCallback)
            {
                GetLeaderBoardFromMageServer(name, SelectBoardOption.Both, onCompleteCallback);
            }
            else
            {
                GetLeaderBoardFromMageServer(name);
            }
        }

        public void GetLeaderBoardFromMageServer(
            string fieldName,
            SelectBoardOption selectOption = SelectBoardOption.Both,
            Action<List<LeaderBoardItem>> onCompleteCallback = null,
            SortType sortMethod = SortType.Ascendent,
            int topLimit = 50,
            int nearByLimit = 10)
        {
            MageAdaptor.GetLeaderBoardFromMageServer(fieldName, selectOption, onCompleteCallback, sortMethod, topLimit, nearByLimit);
        }


        public void GetNearByLeaderBoards(
            string userId,
            string boardName,
            SortType sortMethod = SortType.Descendent,
            int limit = 50,
            Action<List<LeaderBoardItem>> onSuccessCallback = null,
            Action onErrorCallback = null
        )
        {
            if (null != onSuccessCallback)
            {
                GetLeaderBoardFromMageServer(boardName, SelectBoardOption.NearByOnly, onSuccessCallback, sortMethod, limit, limit);
            }
            else
            {
                GetLeaderBoardFromMageServer(boardName, SelectBoardOption.NearByOnly, null, sortMethod, limit, limit);
            }
        }

        public void GetTopLeaderBoards(
            string userId,
            string boardName,
            SortType sortMethod = SortType.Descendent,
            int limit = 50,
            Action<List<LeaderBoardItem>> onSuccessCallback = null,
            Action onErrorCallback = null
        )
        {

            if (null != onSuccessCallback)
            {
                GetLeaderBoardFromMageServer(boardName, SelectBoardOption.TopOnly, onSuccessCallback, sortMethod, limit);
            }
            else
            {
                GetLeaderBoardFromMageServer(boardName, SelectBoardOption.TopOnly, null, sortMethod, limit, limit);
            }

        }

        #endregion

        public void SendApi<TResult>(string apiName, BaseRequest request, Action<TResult> callback, Action<int> errorCallback, Action timeoutCallback, bool isQueueTask = true) where TResult : BaseResponse
        {
            //prepare request
            var form = new WWWForm();

            var formData = System.Text.Encoding.UTF8.GetBytes(request.ToJson());
            ApiUtils.Log("Request: " + request.ToJson());
            var header = form.headers;
            header.Remove("Content-Type");
            header.Add("Content-Type", "application/json");

            ApiUtils.Log("API URL: " + MageEngine.instance.ApiUrl + "/" + MageEngine.instance.ApplicationKey + "/" + apiName);
            var www = new WWW(MageEngine.instance.ApiUrl + "/" + MageEngine.instance.ApplicationKey + "/" + apiName, formData, header);

            StartCoroutine(WaitForReceiveInfo(apiName, callback, errorCallback, timeoutCallback, www, isQueueTask));
        }

        public void UploadFile<TResult>(BaseRequest request, Action<TResult> callback, Action<int> errorCallback, Action timeoutCallback) where TResult : BaseResponse
        {
            //prepare request
            var form = new WWWForm();

            form.AddField("ApiVersion", request.ApiVersion);
            form.AddField("ApiKey", request.ApiKey);
            form.AddField("UUID", request.UUID);
            form.AddField("DeviceOS", request.DeviceOS);
            form.AddField("AppVersion", request.AppVersion);
            form.AddField("GUID", request.GUID);
            form.AddField("Token", request.Token);
            form.AddField("UserId", request.UserId);
            form.AddBinaryData("UploadedFile", request.GetUploadFile());


            var header = form.headers;
            //header.Remove("Content-Type");
            //header.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            ApiUtils.Log("API URL: " + MageEngine.instance.ApiUrl + "/" + MageEngine.instance.ApplicationKey + "/" + "UploadFile");
            //ApiUtils.Log ("Form data: " + form.data);
            var www = new WWW(MageEngine.instance.ApiUrl + "/" + MageEngine.instance.ApplicationKey + "/" + "UploadFile", form.data, header);

            StartCoroutine(WaitForReceiveInfo("UploadFile", callback, errorCallback, timeoutCallback, www));
        }

        private IEnumerator WaitForReceiveInfo<TResult>(string apiName, Action<TResult> callback, Action<int> errorCallback, Action timeoutCallback, WWW www, bool isQueueTask = true) where TResult : BaseResponse
        {
            if (apiName == "Login")
            {
                ApiUtils.Log("[Time Checking]: Login request sent after: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
            }
            //this.loadingCircular.Show (true);
            float time = 0;
            bool isTimeout = false;
            while (!isTimeout)
            {
                if (time >= TimeOut)
                {
                    isTimeout = true;
                    break;
                }
                else
                {
                    time += Time.deltaTime;
                }
                if (www.isDone)
                {
                    isTimeout = false;
                    break;
                }
                yield return null;
            }
            if (apiName == "Login")
            {
                ApiUtils.Log("[Time Checking]: Login request received after: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
            }
            //this.loadingCircular.Hide ();
            if (!isTimeout)
            {
                if (www.text != null)
                {

                    ApiUtils.Log("Response: " + www.text);
                    //File.AppendAllText (Application.dataPath + "/Images/result.txt", "\r\n" + www.text);
                    // handle error in response
                    GenericResponse<TResult> result = new GenericResponse<TResult>();
                    try
                    {
                        result = BaseResponse.CreateFromJSON<GenericResponse<TResult>>(www.text);
                    }
                    catch (Exception e)
                    {
                        //ApiUtils.Log("Invalid server response: " + www.text);
                        //errorCallback (-1);
                        result.status = -1;
                    }

                    if (result != null && result.status == 0)
                    {
                        //save cache to runtime
                        //ApiUtils.Log(result.cache.ToJson());
                        if (null != result.cache)
                        {
                            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_CACHE, result.cache);
                        }
                        else
                        {
                            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_CACHE, new ApiCache());
                        }

                        //ApiUtils.Log("Server time: " + (DateTime.Parse(result.timestamp)).ToString("yyyy-MM-dd hh:mm:ss"));
                        if (null != result.timestamp)
                        {
                            DateTime serverTime = DateTime.Parse(result.timestamp);

                            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_SERVER_TIMESTAMP, serverTime);


                        }
                        else
                        {
                            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_SERVER_TIMESTAMP, DateTime.Now);
                        }

                        if (null != result.data)
                        {
                            if (apiName == "Login")
                            {
                                ApiUtils.Log("[Time Checking]: Login callback called after: " + DateTime.Now.Subtract(_startTime).TotalSeconds);
                            }

                            if (isQueueTask)
                            {
                                this.EnqueueCallbackTask(callback, new object[] { (TResult)result.data });
                            }
                            else
                            {
                                callback((TResult)result.data);
                            }

                        }
                        else
                        {
                            errorCallback(-1);
                        }

                    }
                    else
                    {
                        //ApiUtils.Log ("Error message: " + result.error);
                        RuntimeParameters.GetInstance().SetParam(ApiSettings.API_SERVER_TIMESTAMP, DateTime.Now);
                        if (null != result)
                        {
                            errorCallback(result.status);
                        }
                        else
                        {
                            errorCallback(-1);
                        }

                    }
                    this._lastTimeStampUpdate = DateTime.Now;

                }
                else
                {
                    ApiUtils.Log("Error message: Response is null");
                    RuntimeParameters.GetInstance().SetParam(ApiSettings.API_SERVER_TIMESTAMP, DateTime.Now);
                    errorCallback(-1);
                }

            }
            else
            {
                ApiUtils.LogError("\n www is null or have error: " + www.error + "\n" + www.url);
                ApiUtils.LogError("timeout " + apiName);
                RuntimeParameters.GetInstance().SetParam(ApiSettings.API_SERVER_TIMESTAMP, DateTime.Now);
                timeoutCallback();
            }
            www.Dispose();
        }

        #region queue processing
        private void EnqueueCallbackTask(MageTaskQueueItem task)
        {
            MageTaskQueueHelper.GetInstance().Enqueue(task);
        }

        public void EnqueueCallbackTask(object action, object[] parameters = null)
        {

            MageTaskQueueItem task = new MageTaskQueueItem()
            {
                action = action,
                parameters = parameters
            };

            if (parameters == null)
            {
                task.parameters = new object[] { };
            }

            this.EnqueueCallbackTask(task);
        }


        IEnumerator ProcessCallbackQueue()
        {
            _isCallbackQueueProcessing = true;
            // if there are tasks in queue then process
            if (this._isPollingAllowed)
            {
                while (MageTaskQueueHelper.GetInstance().GetQueueSize() > 0)
                {
                    StartCoroutine(MageTaskQueueHelper.GetInstance().ProcessNextQueueTask());
                    yield return new WaitForEndOfFrame();
                }
            }

            _isCallbackQueueProcessing = false;
        }

        public void DisablePolling()
        {
            this._isPollingAllowed = false;
        }

        public void EnablePolling()
        {
            this._isPollingAllowed = true;
        }
        #endregion queue processing

    }

}