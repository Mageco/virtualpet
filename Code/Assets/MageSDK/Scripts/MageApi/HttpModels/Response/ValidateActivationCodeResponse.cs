using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;


namespace MageApi.Models.Response{
	[Serializable]
	public class ValidateActivationCodeResponse : BaseResponse
	{
		public List<string> LicenseItems;
	}
}
