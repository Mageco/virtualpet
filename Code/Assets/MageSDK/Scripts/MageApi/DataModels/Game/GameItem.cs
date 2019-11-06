using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Game{
	[Serializable]
	public class GameItem : BaseModel
	{
		public string item_local_id = "";

		public string item_name = "";

		public string item_type = "";
		public Boolean is_consumable = false;
		public int in_app_prince = 0;
		public string in_app_currency = "";

		public int total_bought = 0;
		public string status = "";
		public GameItem() : base () {
			
		}
		
	}
}
