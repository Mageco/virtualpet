using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace MageApi.Models.Request {
	/**
	 * This api will be used to activate subscirption for user who by subscirption via Store function (such as In-app purchase or Trial)
	 * In such case, client application need to activate subscription by product for user
	 */
	[Serializable]
	public class AddGameCharacterRequest: BaseRequest {

		public string CharacterName = "";
		public string CharacterType = "";


		public AddGameCharacterRequest() : base() {

		}

		public AddGameCharacterRequest(string characterName, string characterType = "") : base() {
			this.CharacterName = characterName;
			this.CharacterType = characterType;
		}
	}
}
