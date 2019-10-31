﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models.Users;
using Mage.Models.Application;

namespace MageApi.Models.Response{
	[Serializable]
	public class GetAudioFromTextResponse : BaseResponse
	{
		public GeneratedVoice voice;

		public GetAudioFromTextResponse() : base() {
		}
	}

}
