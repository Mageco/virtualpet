using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class GeneratedVoice : BaseModel
	{
		
		public string character = "";
		public string language = "";
        public string audio = "";
		public string voice_cache_checksum = "";

		public string audio_wav = "";

		public GeneratedVoice() : base () {
		}

	}

}
