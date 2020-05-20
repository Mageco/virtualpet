using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models;

namespace MageApi.Models.Filters {
	[Serializable]
	public class LeaderBoardFilters: BaseModel {

        public string original_user_app_version = "";
        public string device_os = "";

		public LeaderBoardFilters() : base() {

		}

		public LeaderBoardFilters(string originalAppVersion) : base() {
			this.original_user_app_version = originalAppVersion;
		}
	}
}
