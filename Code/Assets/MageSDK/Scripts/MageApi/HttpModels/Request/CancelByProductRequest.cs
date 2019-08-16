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
	public class CancelByProductRequest: BaseRequest {

		public string ProductCode = "";
		public bool UseDefaultActvationPool = true;
		
		public CancelByProductRequest() : base() {

		}

		public CancelByProductRequest(string productCode, bool useDefaultActivationPool = true) : base() {
			this.ProductCode = ApiUtils.GetInstance().EncodeXor(productCode);
			this.UseDefaultActvationPool = useDefaultActivationPool;
		}

		public void setProductCode(string productCode) {
			this.ProductCode = ApiUtils.GetInstance().EncodeXor(productCode);
		}
	}
}
