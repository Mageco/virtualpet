using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class ApplicationData : BaseModel
	{
		public string attr_name = ""; 
		public string attr_value = "";
		public string attr_type = "";

		public ApplicationData() : base () {
		}

		public ApplicationData(string attrName, string attrValue, string attrType) : base () {
			this.attr_name = attrName;
			this.attr_value = attrValue;
			this.attr_type = attrType;
		}

	}

	public enum ApplicationBasicData {
		Version,
		AdminUserList
	}
}
