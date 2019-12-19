using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class ApiCache : BaseModel
	{
		public int[] mage_core_application_event_counters = {}; 

		public int to_day_event_counter = 0;
		public string user_ages = "";

		public int[] mage_core_user_event_counters = {};

		public int[] mage_core_logs = {};

		public int[] mage_core_users = {};

		public IPAddress ip_cache = new IPAddress();

		public int cache_timestamp;
		public ApiCache() : base () {
		}

	}
}
