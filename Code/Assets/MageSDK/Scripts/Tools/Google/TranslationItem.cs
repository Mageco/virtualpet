using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace MageSDK.Tools.Google{
	[Serializable]
	public class TranslationItem : BaseModel
	{		
			public string translatedText = "";
			public string model = "";

		public TranslationItem() : base () {

		}

	}
}
