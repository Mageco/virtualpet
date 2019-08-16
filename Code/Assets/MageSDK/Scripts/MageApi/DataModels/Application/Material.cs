using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class Material : BaseModel
	{
		public string application_key = ""; 
		public string material_local_id = "";
		public string language = "";

		public string content = "";

		public MaterialType type = MaterialType.Text;

		public string short_description = "";

		public Material() : base () {
		}

	}

	public enum MaterialType {
		Text = 1,
		Image = 2,
		Audio = 3,

		Video = 4
	}
}
