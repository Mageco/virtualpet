using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;
using MageApi.Models.Response;

namespace MageSDK.Tools.Google{
	[Serializable]
	public class GoogleTranslationResponse : BaseResponse
	{		
			public List<TranslationItem> translations = new List<TranslationItem>();

		public GoogleTranslationResponse() : base () {

		}

	}
}
