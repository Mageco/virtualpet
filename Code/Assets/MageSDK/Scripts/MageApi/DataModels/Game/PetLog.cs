using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Game{
	[Serializable]
	public class PetLog : BaseModel 
	{
		public int itemId = 0;

		public string action = "";

		
        public PetLog() : base () {
			
		}
		
	}
}
