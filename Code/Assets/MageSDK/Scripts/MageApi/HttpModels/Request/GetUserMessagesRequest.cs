using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetUserMessagesRequest: BaseRequest {

		public GetUserMessagesRequest() : base() {

		}

	}
}
