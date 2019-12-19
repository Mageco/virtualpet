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
			string client_ip = "";
			string client_ip_long = "";
			string country_code = "";
			string country_name = "";
			string region_name = "";
			string latitude = "";
			string longitude = "";
			string time_zone = "";
			string zip_ocd = "";

		public IPAddress() : base () {

		}

	}
}
