using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class Script : BaseModel
	{
		public string LocalId = "";
		public string Character = "";
		public string Package = "";
        public float StartTime = 0;

		public string Category = "";

        public List<ScriptContent> Scripts = new List<ScriptContent>();
		public Script() : base () {
		}

		public Script(string localId, string character, string package, string category, float startTime) : base () {
			this.LocalId = localId;
            this.Character = character;
            this.Package = package;
			this.Category = category;
            this.StartTime = startTime;
		}

		public ScriptContent GetContent(string language) {
			foreach (ScriptContent i in this.Scripts) {
				if (i.Language == language) {
					return i;
				}
			}

			return null;
		}

	}
}
