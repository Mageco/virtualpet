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
using Mage.Models.Application;

namespace MageApi {
	public class ApiHandler : MonoBehaviour {

		public bool TestMode;
		public string TestUUID;
		public string ApplicationSecretKey = "";
		public string ApplicationKey = "";
		public string ApiVersion = "";
		public string ApiUrl = "http://localhost:8080/portal/api";
		[HideInInspector]
		public float  TimeOut = 200;


		[HideInInspector]
		public static ApiHandler instance;

		public void Awake() {
			if (instance == null)
				instance = this;
			else
				GameObject.Destroy (this.gameObject);

			RuntimeParameters.GetInstance().SetParam (ApiSettings.API_VERSION, ApiHandler.instance.ApiVersion);
			
			#if UNITY_EDITOR
			if (ApiHandler.instance.TestMode) {

					if (ApiHandler.instance.TestUUID == "" ) {
						RuntimeParameters.GetInstance().SetParam (ApiSettings.DEVICE_ID, ApiUtils.GetInstance ().GenerateGuid ());
					} else {
						RuntimeParameters.GetInstance().SetParam (ApiSettings.DEVICE_ID, ApiHandler.instance.TestUUID);
					}
				} else {
					RuntimeParameters.GetInstance().SetParam (ApiSettings.DEVICE_ID, ApiUtils.GetInstance().GetDeviceID());
				}
			#else
			RuntimeParameters.GetInstance().SetParam (ApiSettings.DEVICE_ID, ApiUtils.GetInstance().GetDeviceID());
			#endif

			RuntimeParameters.GetInstance().SetParam (ApiSettings.API_KEY, ApiUtils.GetInstance().GenerateApiKey(RuntimeParameters.GetInstance().GetParam(ApiSettings.DEVICE_ID).ToString(), ApiHandler.instance.ApiVersion));
			RuntimeParameters.GetInstance().SetParam (ApiSettings.DEVICE_TYPE, ApiUtils.GetInstance ().GetDeviceType ());
			RuntimeParameters.GetInstance().SetParam (ApiSettings.APPLICATION_VERSION, Application.version);
			RuntimeParameters.GetInstance().SetParam (ApiSettings.SYSTEM_LANGUAGE, Application.systemLanguage.ToString());
			
		}

		public void SendApi<TResult>(string apiName, BaseRequest request, Action<TResult> callback, Action<int> errorCallback, Action timeoutCallback) where TResult : BaseResponse{
			//prepare request
			var form = new WWWForm();

			var formData = System.Text.Encoding.UTF8.GetBytes(request.ToJson());
			ApiUtils.Log ("Request: " + request.ToJson ());
			var header = form.headers;
			header.Remove("Content-Type");
			header.Add("Content-Type", "application/json");

			ApiUtils.Log("API URL: " + ApiUrl + "/" +ApplicationKey + "/" + apiName);
			var www = new WWW(ApiUrl + "/" +ApplicationKey + "/" + apiName, formData, header);

			StartCoroutine(WaitForReceiveInfo(apiName, callback, errorCallback, timeoutCallback, www));
		}

		public void UploadFile<TResult>(BaseRequest request, Action<TResult> callback, Action<int> errorCallback, Action timeoutCallback) where TResult : BaseResponse{
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
			ApiUtils.Log("API URL: " + ApiUrl + "/" +ApplicationKey + "/" + "UploadFile");
			//ApiUtils.Log ("Form data: " + form.data);
			var www = new WWW(ApiUrl + "/" +ApplicationKey + "/" + "UploadFile", form.data, header);

			StartCoroutine(WaitForReceiveInfo("UploadFile", callback, errorCallback, timeoutCallback, www));
		}

		private IEnumerator WaitForReceiveInfo<TResult>(string apiName, Action<TResult> callback, Action<int> errorCallback, Action timeoutCallback, WWW www) where TResult : BaseResponse
		{
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
					time +=Time.deltaTime;
				}
				if (www.isDone)
				{
					isTimeout = false;
					break;
				}
				yield return null;
			}

			//this.loadingCircular.Hide ();
			if (!isTimeout)
			{
				if (www.text != null) {
					//ApiUtils.Log ("Response: " + www.text);
					//File.AppendAllText (Application.dataPath + "/Images/result.txt", "\r\n" + www.text);
					// handle error in response
					GenericResponse<TResult> result = new GenericResponse<TResult>();
					try {
						result = BaseResponse.CreateFromJSON<GenericResponse<TResult>>(www.text);
					} catch (Exception e) {
						//ApiUtils.Log("Invalid server response: " + www.text);
						//errorCallback (-1);
						result.status = -1;
					}

					if (result != null && result.status == 0) {
						//save cache to runtime
						//ApiUtils.Log(result.cache.ToJson());
						if (null != result.cache) {
							RuntimeParameters.GetInstance().SetParam (ApiSettings.API_CACHE, result.cache);
						} else {
							RuntimeParameters.GetInstance().SetParam (ApiSettings.API_CACHE, new ApiCache());
						}
						
						//ApiUtils.Log("Server time: " + (DateTime.Parse(result.timestamp)).ToString("yyyy-MM-dd hh:mm:ss"));
						if (null != result.timestamp) {
							RuntimeParameters.GetInstance().SetParam (ApiSettings.API_SERVER_TIMESTAMP, DateTime.Parse(result.timestamp));
						} else {
							RuntimeParameters.GetInstance().SetParam (ApiSettings.API_SERVER_TIMESTAMP, DateTime.Now);
						}

						if (null != result.data) {
							callback ((TResult)result.data);
						} else {
							errorCallback(-1);
						}
						
					} else {
						//ApiUtils.Log ("Error message: " + result.error);
						if (null != result) {
							errorCallback (result.status);
						} else {
							errorCallback(-1);
						}
						
					} 
					
					
				} 
				else {
					ApiUtils.Log ("Error message: Response is null");
					errorCallback (-1);
				}

			}
			else
			{
				ApiUtils.LogError("\n www is null or have error: "+ www.error + "\n" + www.url);
				ApiUtils.LogError("timeout "+ apiName);
				timeoutCallback();
			}
			www.Dispose();
		}

	}
}

