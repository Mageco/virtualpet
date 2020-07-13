using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Attributes;

namespace Mage.Models {
	[Serializable]
	public class BaseModel {

		private Hashtable _valueTracker = new Hashtable();
		private bool _valueTrackerLoaded = false;
		public BaseModel() {
			_valueTracker = new Hashtable ();
		}

		public string ToJson() {
			return JsonUtility.ToJson (this);
		}

		public static TResult CreateFromJSON<TResult>(string jsonString) where TResult: BaseModel
		{
			try {
				return JsonUtility.FromJson<TResult>(jsonString);
			} catch (Exception e) {
				return default(TResult);
			}
			
		}

		public static T Clone<T>(T obj) where T: BaseModel
		{
			try {
				return JsonUtility.FromJson<T>(obj.ToJson());
			} catch (Exception e) {
				return default(T);
			}
			
		}

		public string ToEncryptedJson(string key) {
			return ApiUtils.GetInstance().EncryptStringWithKey(JsonUtility.ToJson (this), key);
		}

		public static TResult CreatFromEncryptJson<TResult>(string encryptedString, string key) where TResult: BaseModel
		{
			try {
				string jsonString = ApiUtils.GetInstance().DecryptStringWithKey(encryptedString, key);
				return JsonUtility.FromJson<TResult>(jsonString);
			} catch (Exception e) {
				return default(TResult);
			}
		}

		#region OldValueTracking
		public T GetMemberOldValue<T>(string key) {

			if (_valueTracker.Contains(key)) {
				return (T)_valueTracker [key];
			} else {
				return default(T);
			}
		}


		public void SetMemberOldValue(string key, object obj) {
			if (_valueTracker.Contains(key)) {
				_valueTracker [key] = obj;
			} else {
				_valueTracker.Add(key, obj);
			}
		}

		public void LoadValueTracker<T>(T obj) where T:BaseModel {
			//if (!_valueTrackerLoaded) {
			_valueTracker = MageAttributeHelper.CopyMetaFields<T>(obj);
			//}
		}

		public string PrintValueTracker() {
			string result = "";
			foreach (DictionaryEntry info in _valueTracker) {
				result += (info.Key.ToString () + ": " + info.Value.ToString ()) + "\r\n";
			}

			return result;
		}

		#endregion
	
	}
}
