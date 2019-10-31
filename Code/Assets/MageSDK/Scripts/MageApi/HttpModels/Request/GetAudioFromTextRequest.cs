using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetAudioFromTextRequest: BaseRequest {

		public string VoiceCharacter = "";
		public string Language = "";
		public string Text = "";
		public string TextSSML = "";

		public GetAudioFromTextRequest() : base() {
		}

		public GetAudioFromTextRequest(string voiceCharacter, string text, string language, string textSSML) : base() {
			this.VoiceCharacter = voiceCharacter;
			this.Text = text;
			this.Language = language;
			this.TextSSML = textSSML;
		}

	}
}
