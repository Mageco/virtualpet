using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Game{
	[Serializable]
	public class Character : BaseModel
	{
		public string id = "";
		public string character_name = "";
		public string character_type = "";
		public string status = "";

		public List<CharacterData> character_datas = new List<CharacterData>();

		public Character() : base () {
			character_datas = new List<CharacterData>();
		}
		
		public string GetCharacterData(string key)
		{
			foreach (CharacterData u in character_datas) {
				if (u.attr_name == key)
					return u.attr_value;
			}
			return null;
		}

		public void SetCharacterData(CharacterData data) {
			bool found = false;

			foreach (CharacterData u in character_datas) {
				if (u.attr_name == data.attr_name) {
					u.attr_value = data.attr_value;
					u.attr_type = data.attr_type;
					found = true;
					break;
				}
			}

			if (!found) {
				character_datas.Add (data);
			}
		}
	}
}
