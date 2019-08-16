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
	public class GetApplicationAudioResourcesResponse : BaseResponse
	{
		public List<Script> Resources;

		public Script GetScript(string scriptId) {
			
			foreach (Script i in Resources) {
				if (i.LocalId == scriptId) {
					return i;
				}
			}

			return null;
		}
	}
}