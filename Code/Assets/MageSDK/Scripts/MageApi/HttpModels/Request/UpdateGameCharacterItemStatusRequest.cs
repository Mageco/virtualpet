using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Game;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateGameCharacterItemStatusRequest: BaseRequest {

		public string CharacterId = "";
		public string GameCharacterItemId = "";
		public CharacterItemStatus ItemStatus = CharacterItemStatus.AVAILABLE;
		

		public UpdateGameCharacterItemStatusRequest() : base() {
		}


		public UpdateGameCharacterItemStatusRequest(string characterId = "", string gameCharacterItemId = "", CharacterItemStatus itemStatus = CharacterItemStatus.AVAILABLE) : base() {
			this.CharacterId = characterId;
			this.GameCharacterItemId = gameCharacterItemId;
			this.ItemStatus = itemStatus;
		}
	}
}
