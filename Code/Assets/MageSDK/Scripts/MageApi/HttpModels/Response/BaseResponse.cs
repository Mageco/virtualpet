using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MageApi.Models.Response {
	[Serializable]
	public class BaseResponse {

		public BaseResponse() {
		}

		public string ToJson() {
			return JsonUtility.ToJson (this);
		}

		public static TResult CreateFromJSON<TResult>(string jsonString) where TResult: BaseResponse
		{
			return JsonUtility.FromJson<TResult>(jsonString);
		}
	}
}
