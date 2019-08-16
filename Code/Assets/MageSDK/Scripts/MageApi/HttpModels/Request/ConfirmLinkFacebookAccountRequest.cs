using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using MageApi.Models.Request;

namespace MageApi.Models.Request {
	[Serializable]
	public class ConfirmLinkFacebookAccountRequest: BaseRequest {

		public string FacebookId = "";

		public ConfirmLinkFacebookAccountRequest() : base() {
		}

		public ConfirmLinkFacebookAccountRequest(string facebookId) : base() {
			this.FacebookId = facebookId;
		}

	}
}
