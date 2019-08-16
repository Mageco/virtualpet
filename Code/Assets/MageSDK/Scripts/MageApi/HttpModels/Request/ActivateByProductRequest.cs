using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace MageApi.Models.Request {
	/**
	 * This api will be used to activate subscirption for user who by subscirption via Store function (such as In-app purchase or Trial)
	 * In such case, client application need to activate subscription by product for user
	 */
	[Serializable]
	public class ActivateByProductRequest: BaseRequest {

		public string ProductCode = "";
		public string Fullname = "";
		public string Phone = "";
		public string Email = "";


		public ActivateByProductRequest() : base() {

		}

		public ActivateByProductRequest(string productCode, string fullname = "", string phone = "", string email = "") : base() {
			this.ProductCode = ApiUtils.GetInstance().EncodeXor(productCode);
			this.Fullname = fullname;
			this.Phone = phone;
			this.Email = email;
		}

		public void setProductCode(string productCode) {
			this.ProductCode = ApiUtils.GetInstance().EncodeXor(productCode);
		}
	}
}
