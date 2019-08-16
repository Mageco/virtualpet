using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models.Users;
using Mage.Models.Application;

namespace MageApi.Models.Response{
	[Serializable]
	public class GetApplicationDataResponse : BaseResponse
	{
		public List<ApplicationData> ApplicationDatas;

		public Hashtable GetApplicationDatas() {
			Hashtable tmp = new Hashtable ();

			foreach (ApplicationData i in ApplicationDatas) {
				tmp.Add (i.attr_name, i);
			}

			return tmp;
		}

		public ApplicationData GetData(string attrName) {
			
			foreach (ApplicationData i in ApplicationDatas) {
				if (i.attr_name == attrName) {
					return i;
				}
			}

			return null;
		}
	}
}
