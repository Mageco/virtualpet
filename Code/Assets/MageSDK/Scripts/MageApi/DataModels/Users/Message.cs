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
		public System.DateTime sent_at = DateTime.Now;
		public System.DateTime read_at = DateTime.Now;
		public MessageType message_type = MessageType.PrivateMessage;
		public string status = "";
		public User sender = new User();
		public User receiver = new User();

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
}
