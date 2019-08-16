using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace Mage.Models {
	[Serializable]
	public class BaseModel {

		public BaseModel() {
		}

		public string ToJson() {
			return JsonUtility.ToJson (this);
		}

		public static TResult CreateFromJSON<TResult>(string jsonString) where TResult: BaseModel
		{
			return JsonUtility.FromJson<TResult>(jsonString);
		}
	}
}
