using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class IPAddress : BaseModel
	{		
			public string client_ip = "";
			public string client_ip_long = "";
			public string country_code = "";
			public string country_name = "";
			public string region_name = "";
			public string latitude = "";
			public string longitude = "";
			public string time_zone = "";
			public string zip_ocd = "";

		public IPAddress() : base () {

		}

	}
}
