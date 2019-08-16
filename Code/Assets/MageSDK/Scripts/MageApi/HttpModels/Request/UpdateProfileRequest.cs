using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;
using Mage.Models.Users;

namespace MageApi.Models.Request {
	[Serializable]
	public class UpdateProfileRequest: BaseRequest {

		public string Fullname = "";
		public string Phone = "";
		public string Email = "";
		public string Avatar = "";
		public string NotificationToken = "";

		public UpdateProfileRequest() : base() {
		}


		public UpdateProfileRequest(string fullname = "", string phone = "", string email = "", string avatar = "", string notificationToken = "") : base() {
			this.Fullname = fullname;
			this.Phone = phone;
			this.Email = email;
			this.Avatar = avatar;
			this.NotificationToken = notificationToken;
		}
	}
}
