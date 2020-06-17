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

		public static void LoginWithDeviceID(Action<LoginResponse> onCompleteCallback, Action<int> onError = null, Action onTimeout = null)
		{
			LoginRequest r = new LoginRequest (ApiSettings.LOGIN_DEVICE_UUID);

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

		///<summary>Save Application Data to server</summary>
		public static void AdminSaveApplicationDataToServer(List<ApplicationData> applicationDatas, string unityAdminToken, Action successCallback = null, Action<int> onError = null, Action onTimeout = null) {
			UpdateApplicationDataRequest r = new UpdateApplicationDataRequest (applicationDatas, unityAdminToken);

			//call to login api
			ApiHandler.instance.SendApi<UpdateApplicationDataResponse>(
				ApiSettings.API_UPDATE_APPLICATION_DATA,
				r, 
				(result) => {
					if (null != successCallback) {
						successCallback();
					}
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

		///<summary>Send list of user events to server</summary>
		public static void SendUserEventList(List<MageEvent> cachedEvent, Action onSendComplete, Action<int> onError = null, Action onTimeout = null) {
			SendUserEventListRequest r = new SendUserEventListRequest (cachedEvent);

			//call to login api
			ApiHandler.instance.SendApi<SendUserEventListResponse>(
				ApiSettings.API_SEND_USER_EVENT_LIST,
				r, 
				(result) => {
					onSendComplete();
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

		///<summary>Send user events to server</summary>
		public static void SendUserEvent(MageEvent t, Action onSendComplete, Action<int> onError = null, Action onTimeout = null) {
			SendUserEventRequest r = new SendUserEventRequest(t.eventName, t.eventDetail);
			//call to login api
			ApiHandler.instance.SendApi<SendUserEventResponse>(
				ApiSettings.API_SEND_USER_EVENT,
				r, 
				(result) => {
					onSendComplete();
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

