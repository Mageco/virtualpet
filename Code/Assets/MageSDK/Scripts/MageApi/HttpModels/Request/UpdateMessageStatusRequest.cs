using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateMessageStatusRequest: BaseRequest {

		public string MessageId = "";
		public MessageStatus Status = MessageStatus.New;
		public UpdateMessageStatusRequest() : base() {

		}

		public UpdateMessageStatusRequest(string messageId, MessageStatus status) : base() {
			this.MessageId = messageId;
			this.Status = status;

		}

	}
}
