using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace MageSDK.Tools.Google{
	[Serializable]
	public class GoogleTranslationRequest : BaseModel
	{		
			public string model = "nmt";
			public string format = "text";
			public string source = "en";
			public string target = "";
			public string q = "";
		public GoogleTranslationRequest() : base () {

		}

        public GoogleTranslationRequest(string targetLanguage, string text) : base () {
            this.target = targetLanguage;
            this.q = text;
		}

	}
}
