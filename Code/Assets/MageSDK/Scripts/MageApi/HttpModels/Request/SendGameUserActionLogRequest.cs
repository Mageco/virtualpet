using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Application;
using Mage.Models.Game;
using Mage.Models;

namespace MageApi.Models.Request {
	[Serializable]
	public class SendGameUserActionLogRequest: BaseRequest
    {

		public List<ActionLog> ActionList = new List<ActionLog>();

		public SendGameUserActionLogRequest() : base() {

		}

		public SendGameUserActionLogRequest(List<ActionLog> actionList) : base() {
			this.ActionList = actionList;
		}

	}
}
