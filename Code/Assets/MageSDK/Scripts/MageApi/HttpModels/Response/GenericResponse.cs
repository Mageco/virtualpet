using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mage.Models.Application;

namespace MageApi.Models.Response {
	[Serializable]
	public class GenericResponse<TResponse> : BaseResponse where TResponse:BaseResponse {

		public int code  = 0;
		public int status = 0;
		public string error = "";
		public TResponse data;

		public string timestamp = "";

		public ApiCache cache = new ApiCache();

		public GenericResponse() : base() {
		}

	}
}
