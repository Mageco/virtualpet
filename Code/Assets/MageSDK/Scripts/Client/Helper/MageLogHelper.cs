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
using Mage.Models.Game;
using Mage.Models;

namespace MageSDK.Client.Helper {
	public class MageLogHelper {
        private static MageLogHelper _instance;

		
		private List<string> actionLogsKeyLookup = new List<string>();

		private Hashtable cachedActionLog = new Hashtable();

		public MageLogHelper() {
			//load something from local
		}

		public static MageLogHelper GetInstance() {
			if (null == _instance) {
				_instance = new MageLogHelper ();
			} 
			return _instance;
		}

        #region functions

		
		private void AddLogger<T>() where T:BaseModel {
			if (!this.cachedActionLog.Contains(LoggerID<T>())) {
				List<ActionLog> tmp = new List<ActionLog>();
				this.cachedActionLog.Add(LoggerID<T>(), tmp);
				MageEngine.instance.apiCounter.Add("SendGameUserActionLogRequest"+LoggerID<T>(), new OnlineCacheCounter(0, 3));
			}
		}

		public void LogAction<T>(T actionData) where T: BaseModel {
			if (!this.cachedActionLog.Contains(LoggerID<T>())) {
				// add action
				List<ActionLog> tmp = new List<ActionLog>();
				tmp.Add(new ActionLog() {
					action_date = DateTime.Now.ToString("dd-MM-yyyy"),
					sequence = int.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
					action_detail = actionData.ToJson()
				});
				this.cachedActionLog.Add(LoggerID<T>(), tmp);

				//init api cache counter
				MageEngine.instance.apiCounter.Add("SendGameUserActionLogRequest"+LoggerID<T>(), new OnlineCacheCounter(0, 3));
				//save action
				SaveActionsLogs<T>(tmp);
			} else {
				List<ActionLog> tmp = (List<ActionLog>)this.cachedActionLog[LoggerID<T>()];
				tmp.Add(new ActionLog() {
					action_date = DateTime.Now.ToString("dd-MM-yyyy"),
					sequence = int.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")),
					action_detail = actionData.ToJson()
				});
				this.cachedActionLog[LoggerID<T>()] = tmp;
				SaveActionsLogs<T>(tmp);
			}

			//sendActionLogCallback(LoggerID<T>());
		}

		public void ClearLogger<T>() where T:BaseModel {
			List<ActionLog> tmp = new List<ActionLog>();
			this.cachedActionLog[LoggerID<T>()] = tmp;
			SaveActionsLogs<T>(tmp);
		}

		public string LoggerID<T>() {
			string className = typeof(T).Name;
			return MageEngineSettings.GAME_ENGINE_ACTION_LOGS + "_" + className;
		}
		
		public List<ActionLog> GetLogger<T>() where T: BaseModel{
			if (!this.cachedActionLog.Contains(LoggerID<T>())) {
				AddLogger<T>();
				return (List<ActionLog>)this.cachedActionLog[LoggerID<T>()];
			} else {
				return (List<ActionLog>)this.cachedActionLog[LoggerID<T>()];
			}
		}

		private void SaveActionsLogs<T>(List<ActionLog> actionLogs) where T: BaseModel {
			ES2.Save(actionLogs, LoggerID<T>());
		}

		public void LoadActionLogCache() {
			// check key lookup
			if(ES2.Exists(MageEngineSettings.GAME_ENGINE_ACTION_LOGS_KEY_LOOKUP)){
				this.actionLogsKeyLookup = ES2.LoadList<string>(MageEngineSettings.GAME_ENGINE_ACTION_LOGS_KEY_LOOKUP);
				if (this.actionLogsKeyLookup == null) {
					this.actionLogsKeyLookup = new List<string>();
				}
			} else  {
				this.actionLogsKeyLookup = new List<string>();
			}

			// Load all saved action logs
			foreach(string key in this.actionLogsKeyLookup) {
				if(ES2.Exists(key)){
					List<ActionLog> tmp = ES2.LoadList<ActionLog>(key);
					if (tmp == null) {
						tmp =new List<ActionLog>();
					} 
					
					this.cachedActionLog.Add(key, tmp);
				} else  {
					List<ActionLog> tmp =new List<ActionLog>();
					this.cachedActionLog.Add(key, tmp);
				}
			}
		}
		#endregion
	}

}

