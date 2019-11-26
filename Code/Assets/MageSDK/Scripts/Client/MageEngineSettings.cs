using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using SimpleJSON;
using MageApi.Models;
using MageApi.Models.Request;
using MageApi.Models.Response;
using MageApi;

namespace MageSDK.Client {
	public class MageEngineSettings {
		public const string GAME_ENGINE_USER = "GameEngineUser";

		public const string GAME_ENGINE_APPLICATION_DATA = "GameEngineApplicationData";
		public const string GAME_ENGINE_DEFAULT_CHARACTER_DATA = "DefaultCharacterData";
		public const string GAME_ENGINE_DEFAULT_USER_DATA = "DefaultUserData";

		public static string[] GAME_ENGINE_APPLICATION_DATA_ITEM = {
			"DialogData",
			"ItemData",
			"LanguageData",
			"PetData",
			"QuestData",
			"SkillData",
			"DefaultUserData",
			"DefaultCharacterData"
		};
	}

	public enum ClientLoginMethod {		
		LOGIN_DEVICE_UUID = 0,
		LOGIN_FACEBOOK = 1,
		LOGIN_GOOGLE = 2,
		LOGIN_USERNAME_PASSWORD = 3,
	}
}

