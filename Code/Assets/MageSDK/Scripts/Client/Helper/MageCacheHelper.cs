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
	public class MageCacheHelper {
        private static MageCacheHelper _instance;

		public MageCacheHelper() {
			//load something from local
		}

		public static MageCacheHelper GetInstance() {
			if (null == _instance) {
				_instance = new MageCacheHelper ();
			} 
			return _instance;
		}

        #region functions

		///<summary>Use this function to save data to both Engine / Local file</summary>
		public void SaveCacheData<T>(T data, string cacheName) {
			#if PLATFORM_TEST
				if (!MageEngine.instance.resetUserDataOnStart) {
					ES2.Save<T>(data, cacheName);
				}
			#else
				ES2.Save<T>(data, cacheName);
			#endif

			RuntimeParameters.GetInstance().SetParam(cacheName, data);
		}

		public T GetCacheData<T>(string cacheName) {
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

		public T LoadCacheData<T>(string cacheName) {
			
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
		#endregion
	}

}

