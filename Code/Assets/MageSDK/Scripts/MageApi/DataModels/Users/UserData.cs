using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;
using Mage.Models.Attributes;

namespace Mage.Models.Users{
	[Serializable]
	public class UserData : BaseModel
	{
		public string id = "";
		public string user_id = "";
		public string attr_name = "";
		public string attr_value = "";
		public string attr_type = "";

		public UserData() : base () {
		}

		public UserData(string attrName, string attrValue, string attrType) : base () {
			this.attr_name = attrName;
			this.attr_value = attrValue;
			this.attr_type = attrType;
		}

		public UserData(string attrName) : base () {
			this.attr_name = attrName;
		}
	}
}
