using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class ScriptContent : BaseModel
	{
		public string Language = "";
		public string Text = "";
		public string Audio = "";

		public ScriptContent() : base () {
		}

		public ScriptContent(string language, string text, string audio) : base () {
			this.Language = language;
			this.Text = text;
			this.Audio = audio;
		}

	}
}
