using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;


namespace MageApi.Models.Response{
	[Serializable]
	public class CheckSubscriptionResponse : BaseResponse
	{
		public List<string> LicenseItems;


	}
}
