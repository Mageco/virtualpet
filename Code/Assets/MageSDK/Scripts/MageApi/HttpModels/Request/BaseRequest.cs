using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Application;

namespace MageApi.Models.Request {
	[Serializable]
	public class BaseRequest {
		
		public string ApiVersion = "";
		public string ApiKey = "";
		public string UUID = "";
		public string DeviceOS = "";
		public string AppVersion = "";
		public string GUID = "";
		public string Token = "";
		public string UserId = "";
		public string SystemLanguage = "";
		//public ApiCache ApiCache = new ApiCache();

 		private byte[] UploadFile;

		public BaseRequest() {
			
			this.ApiVersion = RuntimeParameters.GetInstance ().GetStringValule (ApiSettings.API_VERSION);
			
			this.ApiKey = RuntimeParameters.GetInstance ().GetStringValule (ApiSettings.API_KEY);
			
			this.UUID = ApiUtils.GetInstance().EncodeXor(RuntimeParameters.GetInstance().GetStringValule(ApiSettings.DEVICE_ID));
			
			this.DeviceOS = RuntimeParameters.GetInstance ().GetStringValule (ApiSettings.DEVICE_TYPE);
			
			this.AppVersion = RuntimeParameters.GetInstance ().GetStringValule (ApiSettings.APPLICATION_VERSION);
			
			this.GUID = ApiUtils.GetInstance ().GenerateGuid ();
			
			this.Token = RuntimeParameters.GetInstance ().GetStringValule (ApiSettings.SESSION_LOGIN_TOKEN);
			
			this.UserId = ApiUtils.GetInstance().EncodeXor(RuntimeParameters.GetInstance().GetLoggedInUserId());
			
			this.SystemLanguage = RuntimeParameters.GetInstance().GetStringValule(ApiSettings.SYSTEM_LANGUAGE);

			// add cache for api
			ApiCache tmpApiCache = (ApiCache)RuntimeParameters.GetInstance().GetParam(ApiSettings.API_CACHE);
			if (tmpApiCache == null) {
				tmpApiCache = new ApiCache();
				RuntimeParameters.GetInstance().SetParam(ApiSettings.API_CACHE, tmpApiCache);
			}
			//this.ApiCache = tmpApiCache;
			
			this.UploadFile = new byte[]{};
		}

		public static TResult CreateFromJSON<TResult>(string jsonString) where TResult: BaseRequest
		{
			return JsonUtility.FromJson<TResult>(jsonString);
		}

		public string ToJson() {
			return JsonUtility.ToJson (this);
		}

		public void SetUploadFile(byte[] file) {
			this.UploadFile = file;
		} 

		public byte[] GetUploadFile() {
			return this.UploadFile;
		}
	}
}
