using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class DeleteUserDataRequest: BaseRequest {

		public List<UserData> UserDatas;

		public DeleteUserDataRequest() : base() {
			UserDatas = new List<UserData> ();
		}
	}
}
