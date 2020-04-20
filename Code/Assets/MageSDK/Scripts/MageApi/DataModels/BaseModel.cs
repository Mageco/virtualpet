using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace Mage.Models {
	[Serializable]
	public class BaseModel : ExtractFieldAttribute {

		public BaseModel() {
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
	}
}
