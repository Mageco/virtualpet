using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MageApi;

namespace MageApi.Models.Request {
	[Serializable]
	public class CheckSubscriptionRequest: BaseRequest {

		// indicator to use default activation pool or not. If default pool is used, then all code, subsbscription validation is based on items defined in 'activate' application
		public bool UseDefaultActvationPool = true;
		public CheckSubscriptionRequest(bool useDefaultActivationPool = true) : base() {
			this.UseDefaultActvationPool = useDefaultActivationPool;
		}

	}
}
