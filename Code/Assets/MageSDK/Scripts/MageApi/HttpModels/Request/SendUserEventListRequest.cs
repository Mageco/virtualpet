using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;
using Mage.Models.Application;

namespace MageApi.Models.Request {
	[Serializable]
	public class SendUserEventListRequest: BaseRequest {

		public List<MageEvent> EventList = new List<MageEvent>();

		public SendUserEventListRequest() : base() {

		}

		public SendUserEventListRequest(List<MageEvent> eventList) : base() {
			this.EventList = eventList;
		}

	}
}
