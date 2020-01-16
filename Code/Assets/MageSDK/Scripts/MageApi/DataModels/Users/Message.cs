using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Users{
	[Serializable]
	public class Message : BaseModel
	{
		public string id = "";
		public string message = "";
		public string title = "";
		public System.DateTime sent_at = DateTime.Now;
		public System.DateTime read_at = DateTime.Now;
		public MessageType message_type = MessageType.PrivateMessage;
		public MessageStatus status = MessageStatus.New;
		public User sender = new User();
		public User receiver = new User();

		public string action_ios = "";
		public string action_android = "";
		public string action_windows = "";
		public string action_others = "";

		public Message() : base () {
			sender = new User();
			receiver = new User ();
		}
	}

	public enum MessageType {
		PrivateMessage = 1,
		PublicMessage = 2,
		PushNotification = 3
			
	}

	public enum MessageStatus {
		New,
		Delivered,
		Read,
		Deleted
	}
}
