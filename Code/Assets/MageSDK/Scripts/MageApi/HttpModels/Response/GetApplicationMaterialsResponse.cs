using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models.Users;
using Mage.Models.Application;

namespace MageApi.Models.Response{
	[Serializable]
	public class GetApplicationMaterialsResponse : BaseResponse
	{
		public List<Material> Materials;

		public Material GetMaterial(string localId) {
			
			foreach (Material i in Materials) {
				if (i.material_local_id == localId) {
					return i;
				}
			}

			return null;
		}
	}
}