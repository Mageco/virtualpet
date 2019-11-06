﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models.Game;

namespace MageApi.Models.Response{
	[Serializable]
	public class GetAvailableGameCharacterItemsResponse : BaseResponse
	{
		public List<GameItem> Items;

		public GetAvailableGameCharacterItemsResponse() : base() {
		}
	}

}
