using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.RemoteConfig;
using Mage.Models;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageSDK.Client;
using UnityEngine;

namespace MageSDK.Client.Adaptors 
{
	public class FirebaseAdaptor 
	{
		private static FirebaseAuth auth;

		public static IEnumerator InitializeFirebaseAnalyticWithResolveDependency() {
			yield return FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
				//dependencyStatus = task.Result;
				if (task.Result == DependencyStatus.Available) {
					ApiUtils.Log("Firebase: initialized analytic " + task.Result);
					InitializeAnalytic();
				} else {
					ApiUtils.LogError("Firebase: Could not resolve all Firebase dependencies: " + task.Result);
				}
			});
		}
		///<summary>Initialize Firebase Analytic</summary>
		public static void InitializeAnalytic() {
			FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
				if (task.Result == DependencyStatus.Available) {
					ApiUtils.Log("Firebase: initialized analytic " + task.Result);
					FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
				} else {
					ApiUtils.LogError("Firebase: Could not resolve all Firebase dependencies: " + task.Result);
				}
			});
			//Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

			// Set the user's sign up method.
			Firebase.Analytics.FirebaseAnalytics.SetUserProperty(
				Firebase.Analytics.FirebaseAnalytics.UserPropertySignUpMethod,
				"Anonymous");

			if (RuntimeParameters.GetInstance().GetStringValule(MageEngineSettings.GAME_ENGINE_FB_USER_ID) != "")
				Firebase.Analytics.FirebaseAnalytics.SetUserId(RuntimeParameters.GetInstance().GetStringValule(MageEngineSettings.GAME_ENGINE_FB_USER_ID));
		}

		public static List<ApplicationData> GetApplicationDataToList() 
		{
			List<ApplicationData> result = new List<ApplicationData>();

			ConfigValue keys = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(MageEngineSettings.GAME_ENGINE_FB_LIST_OF_REMOTE_CONFIG_KEYS);
			ApiUtils.Log("Firebase: Remote key: " + keys.StringValue);
			if (keys.StringValue != "") {
				string[] otherKeys = keys.StringValue.Split(',');
				if (otherKeys.Length > 0) {
					foreach(string otherKey in otherKeys) {
						string val = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(otherKey).StringValue;
						ApiUtils.Log("Firebase: Key: " + otherKey + " value: " + val);
						if (val != "") {
							ApplicationData d = new ApplicationData(otherKey, val, "");
							result.Add(d);
						}
						
					}
				}
			}
			
			return result;

		}

		public static IEnumerator GetApplicationDataFromServer(Action<List<ApplicationData>> onCompleteCallback) {
			ApiUtils.Log("Firebase: Fetching data...");
			
			Task t =  Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(
				TimeSpan.Zero);

			yield return t;

			Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
			ApiUtils.Log("Firebase: Fetching data complete");
			onCompleteCallback(GetApplicationDataToList());
		}

		public static IEnumerator LoginAnonymous(Action<FirebaseUser> onCompleteCallback) {
			ApiUtils.Log("Firebase: Login anonymous ...");
			FirebaseAuth auth = FirebaseAuth.DefaultInstance;
			
			if (auth.CurrentUser == null) {
				Task<FirebaseUser> t =  auth.SignInAnonymouslyAsync();

				yield return t;

				if (t.IsCanceled) {
					ApiUtils.LogError("Firebase: LoginAnonymous was canceled.");
				}
				if (t.IsFaulted) {
					ApiUtils.LogError("Firebase: LoginAnonymous encountered an error: " + t.Exception);
				}

				if (t.IsCompleted) {
					Firebase.Auth.FirebaseUser newUser = t.Result;
					ApiUtils.Log("Firebase: LoginAnonymous - User signed in successfully: " + newUser.DisplayName + " : " + newUser.UserId);
					RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_FB_USER_ID, newUser.UserId);

					// callback if any additional action is required
					onCompleteCallback(newUser);
				}
			} else {
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_FB_USER_ID, auth.CurrentUser.UserId);
				onCompleteCallback(auth.CurrentUser);
			}
			
		}

		/*public static IEnumerator LoginWithMageUser(User user, Action<FirebaseUser> onCompleteCallback) {
			ApiUtils.Log("Firebase: Login with MageUser ...");
			var uid = user.id + "-" + user.uuid;
			var additionalClaims = new Dictionary<string, object>()
			{
				{ "full_name", user.fullname },
				{ "avatar", user.avatar },
				{ "mage_id", user.id}
			};

			FirebaseAuth auth = FirebaseAuth.DefaultInstance;
			Task<FirebaseUser> t = FirebaseAuth.DefaultInstance.SignInWithCustomTokenAsync(uid);

			yield return t;

			if (t.IsCanceled) {
				ApiUtils.LogError("Firebase: LoginWithMageUser was canceled.");
			}
			if (t.IsFaulted) {
				ApiUtils.LogError("Firebase: LoginWithMageUser encountered an error: " + t.Exception);
			}

			if (t.IsCompleted) {
				Firebase.Auth.FirebaseUser newUser = t.Result;
				ApiUtils.Log("Firebase: LoginWithMageUser - User signed in successfully: " + newUser.DisplayName + " : " + newUser.UserId);
				onCompleteCallback(newUser);
			}

			Firebase.Auth.FirebaseUser u = auth.CurrentUser;
			if (u != null) {
				string name = u.DisplayName;
				string email = u.Email;
				System.Uri photo_url = u.PhotoUrl;
				// The user's Id, unique to the Firebase project.
				// Do NOT use this value to authenticate with your backend server, if you
				// have one; use User.TokenAsync() instead.
				string tx = u.UserId;

				ApiUtils.Log("id: " + tx);
			}
		}*/

		public static void UpdateUserData(UserData data) {
			ApiUtils.Log("Firebase: UpdateUserData - " + data.ToJson());
			Firebase.Analytics.FirebaseAnalytics.SetUserProperty(data.attr_name, data.attr_type);
		}

		public static void OnEvent(MageEventType type, string eventDetail = "") {
			ApiUtils.Log("Firebase: OnEvent - " + type.ToString() + eventDetail);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(type.ToString(), eventDetail, "");
		}

	}
	
		
}

