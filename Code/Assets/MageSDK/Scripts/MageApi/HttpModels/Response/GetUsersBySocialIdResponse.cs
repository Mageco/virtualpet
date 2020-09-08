using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models.Users;

namespace MageApi.Models.Response{
	[Serializable]
	public class GetUsersBySocialIdResponse : BaseResponse
	{
		public List<User> Users;
	}
}
