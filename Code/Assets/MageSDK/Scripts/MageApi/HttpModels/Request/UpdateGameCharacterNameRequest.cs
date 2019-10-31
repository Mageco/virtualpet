using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateGameCharacterNameRequest: BaseRequest {

		public string CharacterId = "";
		public string CharacterName = "";
		

		public UpdateGameCharacterNameRequest() : base() {
		}


		public UpdateGameCharacterNameRequest(string characterId = "", string characterName = "") : base() {
			this.CharacterId = characterId;
			this.CharacterName = characterName;
		}
	}
}
