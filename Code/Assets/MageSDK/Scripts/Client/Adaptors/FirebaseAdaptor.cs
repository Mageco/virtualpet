#if USE_FIREBASE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.RemoteConfig;
using Firebase.Storage;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using UnityEngine;


namespace MageSDK.Client.Adaptors
{
    public class FirebaseAdaptor
    {
        public static FirebaseAuth auth;
        public static FirebaseStorage storage;
        public static FirebaseApp app;

        public static bool isStackInitialized = false;

        ///<summary>Initialize Firebase Analytic</summary>
        public static void InitializeFirebaseStacks()
        {
            if (MageEngine.instance.useFirebaseAnalytic)
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    try
                    {
                        if (task.Result == DependencyStatus.Available)
                        {
                            ApiUtils.Log("Firebase: initialized analytic " + task.Result);
                            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                            // Set the user's sign up method.
                            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(
                                Firebase.Analytics.FirebaseAnalytics.UserPropertySignUpMethod,
                                "Anonymous");

                            if (RuntimeParameters.GetInstance().GetStringValule(MageEngineSettings.GAME_ENGINE_FB_USER_ID) != "")
                                Firebase.Analytics.FirebaseAnalytics.SetUserId(RuntimeParameters.GetInstance().GetStringValule(MageEngineSettings.GAME_ENGINE_FB_USER_ID));

                            // init other components
                            // init storage
                            if (MageEngine.instance.useFirebaseStorage)
                                InitializeStorage();

                            // init default app
                            app = FirebaseApp.DefaultInstance;

                            // init database and firestore
                            isStackInitialized = true;
                        }
                        else
                        {
                            ApiUtils.LogError("Firebase: Could not resolve all Firebase dependencies: " + task.Result);
                        }
                    }
                    catch (Exception e)
                    {
                        ApiUtils.LogError(e.StackTrace);
                    }

                });
            }


        }

        public static void InitializeStorage()
        {
            storage = FirebaseStorage.DefaultInstance;
        }

        ///<summary>Get Remote Config from firebase and convert to List of ApplicationData of MageSDK</summary>
        public static List<ApplicationData> GetApplicationDataToList()
        {
            List<ApplicationData> result = new List<ApplicationData>();

            ConfigValue keys = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(MageEngineSettings.GAME_ENGINE_FB_LIST_OF_REMOTE_CONFIG_KEYS);
            ApiUtils.Log("Firebase: Remote key: " + keys.StringValue);
            if (keys.StringValue != "")
            {
                string[] otherKeys = keys.StringValue.Split(',');
                if (otherKeys.Length > 0)
                {
                    foreach (string otherKey in otherKeys)
                    {
                        string val = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(otherKey).StringValue;
                        ApiUtils.Log("Firebase: Key: " + otherKey + " value: " + val);
                        if (val != "")
                        {
                            ApplicationData d = new ApplicationData(otherKey, val, "");
                            result.Add(d);
                        }

                    }
                }
            }

            return result;

        }

        ///<summary>Get Remote Config from firebase and convert to List of ApplicationData of MageSDK</summary>
        public static IEnumerator GetApplicationDataFromServer(Action<List<ApplicationData>> onCompleteCallback, bool isQueueTask = true)
        {
            ApiUtils.Log("Firebase: Fetching data...");

            Task t = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(
                TimeSpan.Zero);

            yield return t;
            try
            {
                Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                ApiUtils.Log("Firebase: Fetching data complete");
                if (isQueueTask)
                {
                    MageEngine.instance.EnqueueCallbackTask(onCompleteCallback, new object[] { GetApplicationDataToList() });
                }
                else
                {
                    onCompleteCallback(GetApplicationDataToList());
                }


            }
            catch (Exception e)
            {
                ApiUtils.Log(e.StackTrace);
            }
        }

        ///<summary>Login to firebase anonymous</summary>
        public static IEnumerator LoginAnonymous(Action<FirebaseUser> onCompleteCallback)
        {
            ApiUtils.Log("Firebase: Login anonymous ...");
            auth = FirebaseAuth.DefaultInstance;

            if (auth.CurrentUser == null)
            {
                Task<FirebaseUser> t = auth.SignInAnonymouslyAsync();

                yield return t;

                if (t.IsCanceled)
                {
                    ApiUtils.LogError("Firebase: LoginAnonymous was canceled.");
                }
                if (t.IsFaulted)
                {
                    ApiUtils.LogError("Firebase: LoginAnonymous encountered an error: " + t.Exception);
                }

                if (t.IsCompleted)
                {
                    Firebase.Auth.FirebaseUser newUser = t.Result;
                    ApiUtils.Log("Firebase: LoginAnonymous - User signed in successfully: " + newUser.DisplayName + " : " + newUser.UserId);
                    RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_FB_USER_ID, newUser.UserId);

                    // callback if any additional action is required
                    onCompleteCallback(newUser);
                }
            }
            else
            {
                RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_FB_USER_ID, auth.CurrentUser.UserId);
                onCompleteCallback(auth.CurrentUser);
            }

        }


        ///<summary>Update user Properties</summary>
        public static void UpdateUserProperty(UserData data)
        {
            // if not initialized then skip next actions
            if (!IsStackInitialized()) return;

            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(data.attr_name, data.attr_value);
        }

        ///<summary>send event to firebase</summary>
        public static void OnEvent(MageEventType type, string eventDetail, int eventValue)
        {
            // if not initialized then skip next actions
            if (!isStackInitialized) return;

            eventDetail = (eventDetail == "" ? "No detail" : eventDetail);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(type.ToString(), eventDetail, eventValue);
        }

        public static void OnEvent(MageEventType type, string eventDetail, string eventValue)
        {
            // if not initialized then skip next actions
            if (!isStackInitialized) return;

            eventDetail = (eventDetail == "" ? "No detail" : eventDetail);
            eventValue = (eventValue == "" ? "Empty" : eventValue);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(type.ToString(), eventDetail, eventValue);
        }

        public static void OnEvent(string type, string eventDetail, string eventValue)
        {
            // if not initialized then skip next actions
            if (!isStackInitialized) return;

            eventDetail = (eventDetail == "" ? "No detail" : eventDetail);
            eventValue = (eventValue == "" ? "Empty" : eventValue);
            Firebase.Analytics.FirebaseAnalytics.LogEvent(type, eventDetail, eventValue);
        }

        public static IEnumerator UploadFile(string file, Action<string> onCompleteCallback)
        {
            // Create a root reference
            string local_file = Application.dataPath + file;
            Firebase.Storage.StorageReference storage_ref = storage.RootReference;
            Firebase.Storage.StorageReference file_ref = storage_ref.Child(MageEngine.instance.ApplicationKey + "/" + file);

            Task<StorageMetadata> task = file_ref.PutFileAsync(local_file);

            yield return task;

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                onCompleteCallback("");
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                Task<Uri> t = storage_ref.GetDownloadUrlAsync();
                yield return t;

                Debug.Log("Finished uploading...");
                Debug.Log("download url = " + t.Result.AbsoluteUri);
                onCompleteCallback(t.Result.AbsoluteUri);
            }
        }

        public static IEnumerator UploadImage(Texture2D image, string file, Action<string> onCompleteCallback)
        {

            // Create a root reference
            Firebase.Storage.StorageReference storage_ref = storage.RootReference;
            Firebase.Storage.StorageReference file_ref = storage_ref.Child(MageEngine.instance.ApplicationKey + "/" + file);

            Task<StorageMetadata> task = file_ref.PutBytesAsync(image.EncodeToPNG());

            yield return task;

            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                // Uh-oh, an error occurred!
                onCompleteCallback("");
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                Task<Uri> t = storage_ref.GetDownloadUrlAsync();
                yield return t;

                Debug.Log("Finished uploading...");
                Debug.Log("download url = " + t.Result.AbsoluteUri);
                MageEngine.instance.UpdateUserAvatar(image, t.Result.AbsoluteUri, onCompleteCallback);

            }
        }


        private static bool IsStackInitialized()
        {
            if (!isStackInitialized)
            {
                InitializeFirebaseStacks();
            }
            return isStackInitialized;
        }
    }


}

#endif