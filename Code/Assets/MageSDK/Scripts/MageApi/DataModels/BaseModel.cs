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
	}
}
