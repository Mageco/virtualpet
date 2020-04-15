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
using MageApi;

namespace MageSDK.Tools.Google {
	public class GoogleHelper : MonoBehaviour {

		public string TranslationApiUrl = "https://texttospeech.googleapis.com/v1/text:synthesize?fields=audioContent&key=AIzaSyAWkXjUdaNcf3EobdAuF63Nilq7lAuTwb8";
		[HideInInspector]
		public float  TimeOut = 200;


		[HideInInspector]
		public static GoogleHelper instance;

		public void Awake() {
			if (instance == null)
				instance = this;
			else
				GameObject.Destroy (this.gameObject);
		
		}

		public void SendTranslationApi(GoogleTranslationRequest request, Action<GoogleTranslationResponse> callback, Action<int> errorCallback, Action timeoutCallback) {
			//prepare request
			var form = new WWWForm();

			var formData = System.Text.Encoding.UTF8.GetBytes(request.ToJson());
			ApiUtils.Log ("Request: " + request.ToJson ());
			var header = form.headers;
			header.Remove("Content-Type");
			header.Add("Content-Type", "application/json");

			ApiUtils.Log("API URL: " + TranslationApiUrl);
			var www = new WWW(TranslationApiUrl, formData, header);

			StartCoroutine(WaitForReceiveInfo(callback, errorCallback, timeoutCallback, www));
		}



		private IEnumerator WaitForReceiveInfo(Action<GoogleTranslationResponse> callback, Action<int> errorCallback, Action timeoutCallback, WWW www)
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
					GenericResponse<GoogleTranslationResponse> result = new GenericResponse<GoogleTranslationResponse>();
					try {
						result = BaseResponse.CreateFromJSON<GenericResponse<GoogleTranslationResponse>>(www.text);
					} catch (Exception e) {
						//ApiUtils.Log("Invalid server response: " + www.text);
						//errorCallback (-1);
						result.status = -1;
					}

					if (result != null && result.data != null) {
						
							callback (result.data);
						
						
					} else {
						
							errorCallback(-1);
						
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
				timeoutCallback();
			}
			www.Dispose();
		}

        public void TranslateMeExample() {
            GoogleTranslationRequest request = new GoogleTranslationRequest ("vi", "Translate me to Vietnamese");  
		
            //call to login api
            GoogleHelper.instance.SendTranslationApi(
                request, 
                (result) => {
                    ApiUtils.Log("Get translation text successful");
                    ApiUtils.Log("result: " + result.translations[0].ToJson());
                    //do all things like login
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

	}
}

