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
using Mage.Models.Application;
using Mage.Models.Users;

namespace MageSDK.Client.Helper {
	public class SceneTrackerHelper {
        private static SceneTrackerHelper _instance;


		public SceneTrackerHelper() {
			//load something from local
		}

		public static SceneTrackerHelper GetInstance() {
			if (null == _instance) {
				_instance = new SceneTrackerHelper ();
			} 
			return _instance;
		}

        #region functions

		public string ConvertCacheScreenJson() {

			List<CacheScreenTime> cachedSceneTime = GetCacheScreenTimes();
			if (null != cachedSceneTime) {
				string output = "{";
				for (int i = 0; i < cachedSceneTime.Count; i++) {
					output += "\"" + MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE_PREFIX + cachedSceneTime[i].Key + "\": " + cachedSceneTime[i].Value + ", ";
				}

				if (cachedSceneTime.Count > 0) {
					output = output.Substring(0, output.Length - 2);
				}

				output += "}";
				return output;
			} else {
				return "";
			}
		}
	
		public void AddScreenTime(string currentScene) {
			List<CacheScreenTime> cachedSceneTime = GetCacheScreenTimes();
			// don't count when app is not active
			if (!MageEngine.instance.isAppActive) {
				return;
			}

			if (cachedSceneTime == null) {
				cachedSceneTime = new List<CacheScreenTime>();
			}
			DateTime now = DateTime.Now;

			DateTime lastScreenTime = MageCacheHelper.GetInstance().GetCacheData<DateTime>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);
			string lastScreen = MageCacheHelper.GetInstance().GetCacheData<string>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN);
			//string currentScreen = SceneManager.GetActiveScene().name;

			double timeToAdd = (lastScreen == currentScene) ? now.Subtract(lastScreenTime).TotalSeconds : 0;

			// prevent user change local time, so rounding to 1s
			timeToAdd = (timeToAdd > 1 ? 1 : timeToAdd);

			bool found = false;
			for (int i = 0; i < cachedSceneTime.Count; i++) {
				if (cachedSceneTime[i].Key == currentScene) {
					found = true;
					var newKvp = new CacheScreenTime(cachedSceneTime[i].Key, cachedSceneTime[i].Value + timeToAdd);
					cachedSceneTime.RemoveAt(i);
					cachedSceneTime.Insert(i, newKvp);
				}
			}

			if (!found) {
				var newKvp = new CacheScreenTime(currentScene, timeToAdd);
				cachedSceneTime.Insert(cachedSceneTime.Count, newKvp);
			}

			SaveSceneCacheListData(cachedSceneTime, MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE);
			MageCacheHelper.GetInstance().SaveCacheData<string>(currentScene, MageEngineSettings.GAME_ENGINE_LAST_SCREEN);
			MageCacheHelper.GetInstance().SaveCacheData<DateTime>(now, MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);
		}

		public void ResetScreenTime() {
			MageCacheHelper.GetInstance().SaveCacheData<DateTime>(DateTime.Now, MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);
			MageEngine.instance.isAppActive = true;
		}
		
		public void SaveSceneCacheListData(List<CacheScreenTime> data, string cacheName) {
			#if PLATFORM_TEST
				if (!MageEngine.instance.resetUserDataOnStart) {
					ES2.Save(data, cacheName);
				}
			#else
				ES2.Save(data, cacheName);
			#endif

			RuntimeParameters.GetInstance().SetParam(cacheName, data);
		}

		public List<CacheScreenTime> LoadSceneCacheListData() {
			// load other tmp information
			MageCacheHelper.GetInstance().LoadCacheData<string>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN);
			MageCacheHelper.GetInstance().LoadCacheData<DateTime>(MageEngineSettings.GAME_ENGINE_LAST_SCREEN_TIMESTAMP);

			// load cache scene time
			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE)) {
				List<CacheScreenTime> t = ES2.LoadList<CacheScreenTime>(MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE);
				if (t == null) {
					t = new List<CacheScreenTime>();
				}
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE, t);
				return t;
				
			} else {
				List<CacheScreenTime> t = new List<CacheScreenTime>();
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE, t);
				return t;
			}
		}

		private List<CacheScreenTime> GetCacheScreenTimes() {
			return RuntimeParameters.GetInstance().GetParam<List<CacheScreenTime>>(MageEngineSettings.GAME_ENGINE_SCREEN_TIME_CACHE);
		}
		#endregion
	}

}

