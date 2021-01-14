using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class SendMessageRequest: BaseRequest {

		public string ReceiverId = "";
		public MessageType MessageType = MessageType.PushNotification;
		public string Message = "";
		public string Title = "";
		public string Data = "";
		public SendMessageRequest() : base() {

		}

		public SendMessageRequest(string receiverId, MessageType messageType, string message = "", string title = "", string data = "") : base() {
			this.ReceiverId = receiverId;
			this.MessageType = messageType;
			this.Message = message;
			this.Title = title;
			this.Data = data;
		}

	}
}
