using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models.Users;

namespace MageApi.Models.Response{
	[Serializable]
	public class GetUserMessagesResponse : BaseResponse
	{
		public List<Message> Messages;

		public GetUserMessagesResponse() : base() {
		}
	}

}
