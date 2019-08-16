using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using MageApi.Models.Request;

namespace MageApi.Models.Request {
	[Serializable]
	public class LinkFacebookAccountRequest: BaseRequest {
		public string FacebookId = "";

		public LinkFacebookAccountRequest() : base() {

		}

		public LinkFacebookAccountRequest(string facebookId) : base() {
			this.FacebookId = facebookId;
		}

	}
}
