using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Game;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateGameCharacterDataRequest: BaseRequest {

		public string CharacterId = "";
		public List<CharacterData> CharacterDatas;


		public UpdateGameCharacterDataRequest() : base() {
			CharacterDatas = new List<CharacterData> ();
		}

		public UpdateGameCharacterDataRequest(string characterId) : base() {
			this.CharacterId = characterId;
			CharacterDatas = new List<CharacterData> ();
		}
	}
}
