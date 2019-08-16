using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Users{
	[Serializable]
	public class User : BaseModel
	{
		public string id = "";
		public string uuid = "";
		public string username = "";
		public string password = "";
		public string facebook_id = "";
		public string authentication_method = "0";
		public string fullname = "";
		public string phone = "";
		public string email = "";
		public string avatar = "";
		public string status = "";
		public string notification_token = "";
		public string country_code = "";

		public List<UserData> user_datas = new List<UserData>();

		public User() : base () {
			user_datas = new List<UserData>();
		}
		
		public string GetUserData(string key)
		{
			foreach (UserData u in user_datas) {
				if (u.attr_name == key)
					return u.attr_value;
			}
			return null;
		}

		public void SetUserData(UserData data) {
			bool found = false;

			foreach (UserData u in user_datas) {
				if (u.attr_name == data.attr_name) {
					u.attr_value = data.attr_value;
					u.attr_type = data.attr_type;
					found = true;
					break;
				}
			}

			if (!found) {
				user_datas.Add (data);
			}
		}
	}
}
