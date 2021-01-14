using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetUserDataByUserListRequest: BaseRequest {

		public string[] UserIdList;
		public string DataName = "";

		public GetUserDataByUserListRequest() : base() {
			
		}

		public GetUserDataByUserListRequest(string[] userIdList, string dataName)
		{
			this.UserIdList = userIdList;
			this.DataName = dataName;
		}
	}
}
