using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Users{
	[Serializable]
	public class LeaderBoardItem : BaseModel
	{
		public string attr_name = "";
		public int attr_value = 0;
		public User user = new User();

		public LeaderBoardItem() : base () {
			user = new User ();
		}
	}

	public enum SortType {
		Ascendent = 1,
		Descendent = 2
	}
}
