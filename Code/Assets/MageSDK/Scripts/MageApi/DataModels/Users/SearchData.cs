using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Users{
	[Serializable]
	public class SearchData : BaseModel
	{
		public string attr_name = "";
		public string attr_value = "";
		public SearchOperator condition = SearchOperator.Equal;

		public SearchData() : base () {
		}

		public SearchData(string attrName, SearchOperator condition, string attrValue ) : base () {
			this.attr_name = attrName;
			this.attr_value = attrValue;
			this.condition = condition;
		}

	}

	public enum SearchOperator {
		Equal = 0,
		Greater = 1,
		Lesser = 2,
		GreaterOrEqual = 3,
		LesserOrEqual = 4
	}
}
