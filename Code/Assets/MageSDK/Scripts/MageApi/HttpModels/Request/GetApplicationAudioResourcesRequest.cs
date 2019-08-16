using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetApplicationAudioResourcesRequest: BaseRequest {

		public int BuildVersion;

		public List<string> LocalIds;

		public GetApplicationAudioResourcesRequest() : base() {
			this.BuildVersion = 1;
			this.LocalIds = new List<string>();
		}

		public GetApplicationAudioResourcesRequest(int buildVersion) : base() {
			this.BuildVersion = buildVersion;
			this.LocalIds = new List<string>();
		}
		public GetApplicationAudioResourcesRequest(int buildVersion, List<string> localIds) : base() {
			this.BuildVersion = buildVersion;
			this.LocalIds = localIds;
		}

	}
}
