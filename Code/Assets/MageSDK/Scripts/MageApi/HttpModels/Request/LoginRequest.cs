using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace MageApi.Models.Request {
	[Serializable]
	public class LoginRequest: BaseRequest {

		public string AuthenticationMethod = "";
		public string FacebookId = "";

		public string Username = "";
		public string Password = "";
		public LoginRequest(string authenticationMethod, string facebookId = "", string username = "", string password = "") : base() {
			this.AuthenticationMethod = ApiUtils.GetInstance().EncodeXor(authenticationMethod);
			if (authenticationMethod == ApiSettings.LOGIN_FACEBOOK) {
				this.FacebookId = facebookId;
			}
			
			if (authenticationMethod == ApiSettings.LOGIN_USERNAME_PASSWORD) {
				this.Username = username;
				this.Password = ApiUtils.GetInstance().Md5Sum(password);
			}
			
		}

		public void setAuthenticationMethod(string authenticationMethod) {
			this.AuthenticationMethod = ApiUtils.GetInstance().EncodeXor(authenticationMethod);
		}

		public void setPassword(string password) {
			this.Password = ApiUtils.GetInstance().Md5Sum(password);
		}
	}
}
