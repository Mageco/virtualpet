using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Application;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateApplicationDataRequest: BaseRequest {

		public List<ApplicationData> ApplicationDatas;
		public string UnityAdminToken = "";

		public UpdateApplicationDataRequest() : base() {
			ApplicationDatas = new List<ApplicationData>();
		}

		public UpdateApplicationDataRequest(List<ApplicationData> applicationDatas, string unityAdminToken) : base() {
			ApplicationDatas = applicationDatas;
			this.UnityAdminToken = ApiUtils.GetInstance().EncodeXor(unityAdminToken);
		}
	}
}
