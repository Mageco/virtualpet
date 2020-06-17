using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.RemoteConfig;
using Mage.Models;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageApi.Models.Request;
using MageApi.Models.Response;
using MageSDK.Client;
using UnityEngine;

namespace MageSDK.Client.Adaptors 
{
	public class MageAdaptor 
	{
		public static void GetApplicationDataFromServer(Action<List<ApplicationData>> onCompleteCallback, Action<int> onError = null, Action onTimeout = null)
		{
			GetApplicationDataRequest r = new GetApplicationDataRequest ();
			//call to login api
			ApiHandler.instance.SendApi<GetApplicationDataResponse>(
				ApiSettings.API_GET_APPLICATION_DATA,
				r, 
				(result) => {
					// store application data
					onCompleteCallback(result.ApplicationDatas);
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
					if (onError != null) {
						onError(errorStatus);
					}
					
				},
				() => {
					if (onTimeout != null) {
						onTimeout();
					}
				}
			);
		}

		public void LoginWithDeviceID(Action<LoginResponse> onCompleteCallback, Action<int> onError = null, Action onTimeout = null)
		{
			//text.text += RuntimeParameters.GetInstance().ToString();
			LoginRequest r = new LoginRequest (ApiSettings.LOGIN_DEVICE_UUID);

			//text.text += "-----\r\n" + r.ToJson();

			//call to login api
			ApiHandler.instance.SendApi<LoginResponse>(
				ApiSettings.API_LOGIN, 
				r, 
				(result) => {
					onCompleteCallback(result);
				},
				(errorStatus) => {
					ApiUtils.Log("Error: " + errorStatus);
					//do some other processing here
					if (onError != null) {
						onError(errorStatus);
					}
					
				},
				() => {
					if (onTimeout != null) {
						onTimeout();
					}
				}
			);
		}

	}
	
		
}

