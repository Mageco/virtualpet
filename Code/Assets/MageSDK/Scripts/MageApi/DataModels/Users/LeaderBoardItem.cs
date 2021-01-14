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
		public int rank = 0;
		public string board_name = "";
		public int score = 0;
		public string user_id = "";
		public string fullname = "";
		public string avatar = "";
		public string title = "";
		public string is_test_user = "0";
		public LeaderBoardItem() : base () {
		}
	}

	public enum SortType {
		Ascendent = 1,
		Descendent = 2
	}
}
