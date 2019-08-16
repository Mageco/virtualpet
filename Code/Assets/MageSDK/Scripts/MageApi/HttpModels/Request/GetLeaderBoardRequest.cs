using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetLeaderBoardRequest: BaseRequest {

		public string DataName = "";

		public SortType SortMethod = SortType.Descendent;

		public GetLeaderBoardRequest() : base() {

		}

		public GetLeaderBoardRequest(string dataName) : base() {
			this.DataName = dataName;
		}
	}
}
