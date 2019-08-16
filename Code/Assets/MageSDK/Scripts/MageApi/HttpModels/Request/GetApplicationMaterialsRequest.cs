using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetApplicationMaterialsRequest: BaseRequest {

		public string Language = "";

		public List<string> LocalIds;

		public GetApplicationMaterialsRequest() : base() {
			this.LocalIds = new List<string>();
		}

		public GetApplicationMaterialsRequest(string language) : base() {
			this.Language = language;
			this.LocalIds = new List<string>();
		}
		public GetApplicationMaterialsRequest(string language, List<string> localIds) : base() {
			this.Language = language;
			this.LocalIds = localIds;
		}

	}
}
