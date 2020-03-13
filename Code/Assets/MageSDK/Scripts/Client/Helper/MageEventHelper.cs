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
using Mage.Models;

namespace MageSDK.Client.Helper {
	public class MageEventHelper {
        private static MageEventHelper _instance;

		public MageEventHelper() {
			//load something from local
		}

		public static MageEventHelper GetInstance() {
			if (null == _instance) {
				_instance = new MageEventHelper ();
			} 
			return _instance;
		}

		public void LoadMageEventData() {
			LoadEventCounterList();
			LoadMageEventCache();
		}
        #region event counter
		
		private void SaveEventCounterList(List<EventCounter> data) {
			#if PLATFORM_TEST
				if (!MageEngine.GetInstance().resetUserDataOnStart) {
					ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
				}
			#else
				ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
			#endif

			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, data);
		}

		private List<EventCounter> LoadEventCounterList() {
			
			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE)) {
				List<EventCounter> t = ES2.LoadList<EventCounter>(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
				if (t == null) {
					t = new List<EventCounter>();
				}
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, t);
				return t;
				
			} else {
				List<EventCounter> t = new List<EventCounter>();
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE, t);
				return t;
			}
		}

		private List<EventCounter> GetEventCounterList() {
			return RuntimeParameters.GetInstance().GetParam<List<EventCounter>>(MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE);
		}

		private void AddEventCounter(string eventName) {
			List<EventCounter> eventCounterList = GetEventCounterList();
			bool found = false;
			//search for eventName
			for (int i = 0; i < eventCounterList.Count; i++) {
				if (eventCounterList[i].Key == eventName) {
					found = true;
					eventCounterList[i].Value++;
					var newKvp = new EventCounter(eventCounterList[i].Key, eventCounterList[i].Value);
					eventCounterList.RemoveAt(i);
					eventCounterList.Insert(i, newKvp);
				}
			}

			if (!found) {
				var newKvp = new EventCounter(eventName, 1);
				eventCounterList.Insert(eventCounterList.Count, newKvp);
			}

			SaveEventCounterList(eventCounterList);
		}

		public string ConvertEventCounterListToJson() {
			List<EventCounter> eventCounterList = GetEventCounterList();

			if (null != eventCounterList) {
				string output = "{";
				for (int i = 0; i < eventCounterList.Count; i++) {
					output += "\"" + MageEngineSettings.GAME_ENGINE_EVENT_COUNTER_CACHE_PREFIX + eventCounterList[i].Key + "\": " + eventCounterList[i].Value + ", ";
				}

				if (eventCounterList.Count > 0) {
					output = output.Substring(0, output.Length - 2);
				}

				output += "}";
				return output;
			} else {
				return "";
			}
		}

		private List<MageEvent> LoadMageEventCache() {

			if (ES2.Exists(MageEngineSettings.GAME_ENGINE_EVENT_CACHE)) {
				List<MageEvent> t = ES2.LoadList<MageEvent>(MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
				if (t == null) {
					t = new List<MageEvent>();
				}
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_CACHE, t);
				return t;
				
			} else {
				List<MageEvent> t = new List<MageEvent>();
				RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_CACHE, t);
				return t;
			}
		}

		public List<MageEvent> GetMageEventsList() {
			return RuntimeParameters.GetInstance().GetParam<List<MageEvent>>(MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
		}

		public void SaveMageEventsList(List<MageEvent> data) {
			#if PLATFORM_TEST
				if (!MageEngine.GetInstance().resetUserDataOnStart) {
					ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
				}
			#else
				ES2.Save(data, MageEngineSettings.GAME_ENGINE_EVENT_CACHE);
			#endif

			RuntimeParameters.GetInstance().SetParam(MageEngineSettings.GAME_ENGINE_EVENT_CACHE, data);
		}

		public void OnEvent(MageEventType type, Action<MageEvent> callbackSendEventApi, string eventDetail = "" ) {
			/*this.cachedEvent.Add(new MageEvent(type, eventDetail));
			SaveEvents();
			SendAppEvents();*/
			// temporary fix to send single event
			AddEventCounter(type.ToString());
			callbackSendEventApi(new MageEvent(type, eventDetail));
		}

		public void OnEvent<T>(MageEventType type, T obj, Action<MageEvent> callbackSendEventApi) where T:BaseModel {
			/*this.cachedEvent.Add(new MageEvent(type, obj.ToJson()));
			SaveEvents();
			SendAppEvents();*/
			// temporary fix to send single event
			AddEventCounter(type.ToString());
			callbackSendEventApi(new MageEvent(type, obj.ToJson()));
		}

		#endregion
	}

}

