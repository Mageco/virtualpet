using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class SendUserEventRequest: BaseRequest {

		public string EventName = "";

		public string EventDetail = "";

		public string EventValue = "";

		public SendUserEventRequest() : base() {

		}

		public SendUserEventRequest(string eventName, string eventDetail = "", string EventValue = "") : base() {
			this.EventName = eventName;
			this.EventDetail = eventDetail;
			this.EventValue = EventValue;
		}

	}
}
