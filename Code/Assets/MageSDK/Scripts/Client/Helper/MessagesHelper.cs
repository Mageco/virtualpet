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
	public class MessageHelper {
        private static MessageHelper _instance;

		public MessageHelper() {
			//load something from local
		}

		public static MessageHelper GetInstance() {
			if (null == _instance) {
				_instance = new MessageHelper ();
			} 
			return _instance;
		}

        #region functions

		////<summary>Send push notification</summary>
		
		public List<Message> LoadUserMessages() {
			
			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_USER_MESSAGE)) {
				List<Message> t = ES2.LoadList<Message>(MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
				if (t == null) {
					t = new List<Message>();
				}
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER_MESSAGE, t);
				return t;
				
			} else {
				List<Message> t = new List<Message>();
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER_MESSAGE, t);
				return t;
			}
		}

		public void UpdateMessageStatus(string msgId, MessageStatus status, Action<string, MessageStatus> apiCallback) {
			List<Message> cachedUserMessages = GetUserMessages();

			bool found = false;
			for (int i = 0; i < cachedUserMessages.Count; i++) {
				if (cachedUserMessages[i].id == msgId) {
					cachedUserMessages[i].status = status;
					found = true;
					break;			
				}
			}

			SaveUserMessages(cachedUserMessages);
			if (found ) {
				apiCallback(msgId, status);
				//UpdateUserMessageStatusToServer(msgId, status);
			}
		}

		public void SaveUserMessages(List<Message> data) {
			#if PLATFORM_TEST
				if (!MageEngine.instance.resetUserDataOnStart) {
					ES2.Save(data, MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
				}
			#else
				ES2.Save(data, MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
			#endif

			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_USER_MESSAGE, data);
		}

		
		public List<Message> GetUserMessages() {
			return RuntimeParameters.GetInstance().GetParam<List<Message>>(MageEngineSettings.GAME_ENGINE_USER_MESSAGE);
		}

		public void AddNewMessages(List<Message> newMessages, Action<string, MessageStatus> apiUpdateMsgStatusCallback) {

			List<Message> cachedUserMessages = GetUserMessages();

			//check and remove messages that in the local list
			List<Message> tmp = new List<Message>();
			for (int j = 0; j < newMessages.Count; j++) {
				bool found = false;
				for (int i = 0; i < cachedUserMessages.Count; i++) {
					if (cachedUserMessages[i].id == newMessages[j].id) {
						//get status from local
						newMessages[j].status = cachedUserMessages[i].status;
						found = true;
						break;			
					}
				}

				if (!found) {
					tmp.Add(newMessages[j]);
				} else {
					apiUpdateMsgStatusCallback(newMessages[j].id, newMessages[j].status);
				}
			}

			for (int i = 0; i < tmp.Count; i++) {
				cachedUserMessages.Add(tmp[i]);
			}

			SaveUserMessages(cachedUserMessages);
		}

	

		#endregion
	}

}

