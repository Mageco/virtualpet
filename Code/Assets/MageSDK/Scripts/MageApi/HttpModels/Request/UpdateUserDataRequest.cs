using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateUserDataRequest: BaseRequest {

		public List<UserData> UserDatas;

		public UpdateUserDataRequest() : base() {
			UserDatas = new List<UserData> ();
		}
	}
}
