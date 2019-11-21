using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mage.Models.Users;

namespace MageApi {
	public class RuntimeParameters {
		private Hashtable _container;

		private static RuntimeParameters _instance;

		public RuntimeParameters() {
			_container = new Hashtable ();
			//add in some already known parameters
			
		}

		public static RuntimeParameters GetInstance() {
			if (null == _instance) {
				_instance = new RuntimeParameters ();
			} 
			return _instance;
		}

		public string GetStringValule(string key) {
			if (_container.Contains(key)) {
				return _container [key].ToString();
			} else {
				return "";
			}
	
		}

		public object GetParam(string key) {

			if (_container.Contains(key)) {
				return _container [key];
			} else {
				return null;
			}
		}


		public void SetParam(string key, object obj) {
			if (_container.Contains(key)) {
				_container [key] = obj;
			} else {
				_container.Add(key, obj);
			}
		}

		public void SetHashTableValue(string paramKey, string tableKey, object obj) {

			Debug.Log ("SetHashtableValue: " + paramKey + " table key: " + tableKey);

			if (_container.Contains(paramKey)) {
				Hashtable tmp = (Hashtable)_container[paramKey];
				tmp[tableKey] = obj;
				_container[paramKey] = tmp;
			} else {
				Hashtable tmp = new Hashtable ();
				tmp.Add (tableKey, obj);
				_container[paramKey] = tmp;
			}
		}

		public object GetHashTableValue(string paramKey, string tableKey) {
			Debug.Log ("GetHashTableValue: " + paramKey + " table key: " + tableKey);
			if (_container.Contains(paramKey) && ((Hashtable)_container[paramKey]).Contains(tableKey)) {
				return ((Hashtable)_container[paramKey])[tableKey];
			} else {
				return null;
			}
			
		}

		public void RemoveHashtableValue(string paramKey, string tableKey) {
			Debug.Log ("SetHashtableValue: " + paramKey + " table key: " + tableKey);

			if (_container.Contains(paramKey)) {
				Hashtable tmp = (Hashtable)_container[paramKey];
				tmp.Remove(tableKey);
				_container[paramKey] = tmp;
			} else {
				Debug.Log ("Key: " + tableKey + " not found.");
			}
		}

		override public string ToString() {
			string result = "";
			foreach (DictionaryEntry info in _container) {
				result += (info.Key.ToString () + ": " + info.Value.ToString ()) + "\r\n";
			}

			return result;
		}

		public string GetLoggedInUserId() {
			if (null != RuntimeParameters.GetInstance().GetParam(ApiSettings.LOGGED_IN_USER)) {
				User user = (User)RuntimeParameters.GetInstance().GetParam(ApiSettings.LOGGED_IN_USER);
				if ("" != user.id) {
					return user.id;
				} else {
					return "";
				}
			} else {
				return "";
			}
		}
	}
}

