using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class CacheScreenTime : BaseModel
	{
		public string Key = ""; 
		public double Value = 0;

		public CacheScreenTime() {
			
		}
        public CacheScreenTime(string key, double value) {
            Key = key;
            Value = value;
        }


	}
}
