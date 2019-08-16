using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace MageApi.Models.Request {
	[Serializable]
	public class ValidateActivationCodeRequest: BaseRequest {

		public string ActivationCode = "";
		public string Fullname = "";
		public string Phone = "";
		public string Email = "";
		
		public ValidateActivationCodeRequest() : base() {

		}

		public ValidateActivationCodeRequest(string activationCode, string fullname = "", string phone = "", string email = "") : base() {
			this.ActivationCode = ApiUtils.GetInstance().EncodeXor(activationCode);
			this.Fullname = fullname;
			this.Phone = phone;
			this.Email = email;
		}

		public void setActivationCode(string activationCode) {
			this.ActivationCode = ApiUtils.GetInstance().EncodeXor(activationCode);
		}
	}
}
