using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetUserProfileRequest: BaseRequest {

		public string ProfileId = "";

		public GetUserProfileRequest() : base() {

		}

		public GetUserProfileRequest(string profileId) : base() {
			this.ProfileId = profileId;
		}

	}
}
