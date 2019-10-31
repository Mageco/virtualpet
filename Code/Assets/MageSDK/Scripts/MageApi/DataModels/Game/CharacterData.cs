using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Game{
	[Serializable]
	public class CharacterData : BaseModel
	{
		public string id = "";
		public string character_id = "";
		public string attr_name = "";
		public string attr_value = "";
		public string attr_type = "";

		public CharacterData() : base () {
		}

		public CharacterData(string attrName, string attrValue, string attrType) : base () {
			this.attr_name = attrName;
			this.attr_value = attrValue;
			this.attr_type = attrType;
		}

		public CharacterData(string attrName) : base () {
			this.attr_name = attrName;
		}
	}
}
