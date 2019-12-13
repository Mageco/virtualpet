using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Game{
	[Serializable]
	public class ActionLog : BaseModel
	{
		public int sequence = 0;

		public string action_date = "";

		public string action_detail ="";

		public string time_stamp = String.Format("{0:s}", DateTime.Now);
        public ActionLog() : base () {
			time_stamp = String.Format("{0:s}", DateTime.Now);
		}

		public T GetAction<T>() where T: BaseModel {
			return BaseModel.CreateFromJSON<T>(action_detail);
		}
		
	}
}
