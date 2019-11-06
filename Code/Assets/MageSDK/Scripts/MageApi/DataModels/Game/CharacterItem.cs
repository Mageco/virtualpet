using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Game{
	[Serializable]
	public class CharacterItem : BaseModel
	{
		public string id = "";
		public string item_local_id = "";

		public string character_id = "";
		public string status = "";
		public CharacterItem() : base () {
			
		}
		
	}

	public enum  CharacterItemStatus {
		NOT_BUY = 0,
		AVAILABLE = 1,
		IN_USED = 2,
		CONSUMED = 101,
		DELETED = 100
	}
}
