using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class GetAudioFromTextRequest: BaseRequest {

		public string Text = "";
		public string Language = "";

		public GetAudioFromTextRequest() : base() {
		}

		public GetAudioFromTextRequest(string text, string language) : base() {
			this.Text = text;
			this.Language = language;
		}

	}
}
