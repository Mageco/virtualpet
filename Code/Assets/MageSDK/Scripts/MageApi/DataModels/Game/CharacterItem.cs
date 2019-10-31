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
		public string item_id = "";
		public string status = "";
		public CharacterItem() : base () {
			
		}
		
	}
}
