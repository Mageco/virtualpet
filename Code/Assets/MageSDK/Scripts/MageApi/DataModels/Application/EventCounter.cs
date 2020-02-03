using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application{
	[Serializable]
	public class EventCounter : BaseModel
	{
		public string Key = ""; 
		public int Value = 0;

		public EventCounter() {
			
		}
        public EventCounter(string key, int value) {
            Key = key;
            Value = value;
        }


	}
}
