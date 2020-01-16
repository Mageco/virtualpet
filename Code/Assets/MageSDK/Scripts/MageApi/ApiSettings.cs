
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MageApi {
	public class ApiSettings {

		private static ApiSettings _instance;

		public ApiSettings ()
		{
		}

		public static ApiSettings GetInstance() {
			if (null == _instance) {
				_instance = new ApiSettings ();
			} 
			return _instance;
		}
		//defines constant key used for system parameters
		public const string API_VERSION = "API_VERSION";
		public const string DEVICE_ID = "DEVICE_ID";
		public const string DEVICE_TYPE = "DEVICE_TYPE";
		public const string SESSION_LOGIN_TOKEN = "SESSION_LOGIN_TOKEN";
		public const string LOGGED_IN_USER = "LOGGED_IN_USER";
		public const string API_KEY = "API_KEY";
		public const string APPLICATION_VERSION = "APPLICATION_VERSION";
		public const string APPLICATION_KEY = "APPLICATION_KEY";
		public const string USE_DEFAULT_ACTIVATION_POOL = "USE_DEFAULT_ACTIVATION_POOL";

		public const string SYSTEM_LANGUAGE = "SYSTEM_LANGUAGE";

		public const string API_CACHE = "API_CACHE";

		//defines api names
		public const string RESPONSE_DATA = "data";
		public const string RESPONSE_ERROR = "error";
		public const string RESPONSE_STATUS = "status";
		public const string RESPONSE_CODE = "code";

		//defines constant key for API name
		public const string API_CONFIRM_LINK_FB_ACCOUNT = "ConfirmLinkFacebookAccount";
		public const string API_DELETE_USER_DATA = "DeleteUserData";
		public const string API_GET_APPLICATION_DATA = "GetApplicationData";
		public const string API_GET_LEADER_BOARD = "GetLeaderBoard";
		public const string API_GET_USER_MESSAGE = "GetUserMessage";
		public const string API_GET_USER_PROFILE = "GetUserProfile";
		public const string API_LINK_FB_ACCOUNT = "LinkFacebookAccount";
		public const string API_LOGIN = "Login";
		public const string API_SEND_MESSAGE = "SendMessage";
		public const string API_SEND_USER_EVENT = "SendUserEvent";
		public const string API_SEND_USER_EVENT_LIST = "SendUserEventList";
		public const string API_UPDATE_PROFILE = "UpdateProfile";
		public const string API_UPDATE_USER_DATA = "UpdateUserData";
		public const string API_UPDATE_MESSAGE_STATUS = "UpdateMessageStatus";
		public const string API_UPLOAD_FILE = "UploadFile";
		public const string API_GET_AUDIO_FROM_TEXT = "GetAudioFromText";
		public const string API_GET_APPLICATION_AUDIO_RESOURCES = "GetApplicationAudioResources";

		public const string API_GET_APPLICATION_MATERIALS = "GetApplicationMaterials";
		public const string API_CHECK_SUBSCRIPTION = "CheckSubscription";
		public const string API_VALIDATE_ACTIVATION_CODE = "ValidateActivationCode";
		public const string API_ACTIVATE_BY_PRODUCT = "ActivateByProduct";
		public const string API_CANCEL_BY_PRODUCT = "CancelByProduct";
		public const string API_UPDATE_USER_LEADER_BOARD = "UpdateUserLeaderboard";

		public const string API_GET_USERS_BY_USER_DATAS = "GetUsersByUserData";
		public const string API_ADD_GAME_CHARACTER = "AddGameCharacter";
		public const string API_UPDATE_GAME_CHARACTER_NAME = "UpdateGameCharacterName";
		public const string API_UPDATE_GAME_CHARACTER_DATAS = "UpdateGameCharacterData";
		public const string API_UPDATE_GAME_CHARACTER_ITEM_STATUS = "UpdateGameCharacterItemStatus";
		public const string API_BUY_GAME_CHARACTER_ITEM = "BuyGameCharacterItem";
		public const string API_GET_AVAILABLE_GAME_CHARACTER_ITEMS = "GetAvailableGameCharacterItems";

		public const string API_SEND_GAME_USER_ACTION_LOG = "SendGameUserActionLog";

		//defines Login method
		public const string LOGIN_DEVICE_UUID = "0";
		public const string LOGIN_FACEBOOK = "1";
		public const string LOGIN_GOOGLE = "2";
		public const string LOGIN_USERNAME_PASSWORD = "3";

		public readonly string[] LANGUAGE_OPTIONS = {"vi", "en"};

		public readonly string[] RESOURCES_GOOGLE_VOICES = {
			"vi-VN-Wavenet-A",
			"vi-VN-Wavenet-B",
			"vi-VN-Wavenet-C",
			"vi-VN-Wavenet-D",
			"vi-VN-Standard-A",
			"vi-VN-Standard-B",
			"vi-VN-Standard-C",
			"vi-VN-Standard-D",
			"en-AU-Wavenet-A",
			"en-AU-Wavenet-B",
			"en-AU-Wavenet-C",
			"en-AU-Wavenet-D",
			"en-GB-Wavenet-A",
			"en-GB-Wavenet-B",
			"en-GB-Wavenet-C",
			"en-GB-Wavenet-D",
			"en-IN-Wavenet-A",
			"en-IN-Wavenet-B",
			"en-IN-Wavenet-C",
			"en-US-Wavenet-A",
			"en-US-Wavenet-B",
			"en-US-Wavenet-C",
			"en-US-Wavenet-D",
			"en-US-Wavenet-E",
			"en-US-Wavenet-F",
			"en-IN-Standard-A",
			"en-IN-Standard-B",
			"en-IN-Standard-C",
			"en-GB-Standard-A",
			"en-GB-Standard-B",
			"en-GB-Standard-C",
			"en-GB-Standard-D",
			"en-US-Standard-B",
			"en-US-Standard-C",
			"en-US-Standard-D",
			"en-US-Standard-E",
			"en-AU-Standard-A",
			"en-AU-Standard-B",
			"en-AU-Standard-C",
			"en-AU-Standard-D",
		};


		public static string GetLanguageOption(LanguageOptions i) {
			return ApiSettings.GetInstance().LANGUAGE_OPTIONS[(int)i];
		}

		public static string GetVoiceCharacter(VoiceCharacterOptions i) {
			return ApiSettings.GetInstance().RESOURCES_GOOGLE_VOICES[(int)i];
		}

	}

	public enum LanguageOptions {
		vi_VN = 0,
		en_EN = 1
	}

	public enum VoiceCharacterOptions {
		vi_VN_Wavenet_A = 0,
		vi_VN_Wavenet_B = 1,
		vi_VN_Wavenet_C = 2,
		vi_VN_Wavenet_D = 3,
		vi_VN_Standard_A = 4,
		vi_VN_Standard_B = 5,
		vi_VN_Standard_C = 6,
		vi_VN_Standard_D = 7,
		en_AU_Wavenet_A = 8,
		en_AU_Wavenet_B = 9,
		en_AU_Wavenet_C = 10,
		en_AU_Wavenet_D = 11,
		en_GB_Wavenet_A = 12,
		en_GB_Wavenet_B = 13,
		en_GB_Wavenet_C = 14,
		en_GB_Wavenet_D = 15,
		en_IN_Wavenet_A = 16,
		en_IN_Wavenet_B = 17,
		en_IN_Wavenet_C = 18,
		en_US_Wavenet_A = 19,
		en_US_Wavenet_B = 20,
		en_US_Wavenet_C = 21,
		en_US_Wavenet_D = 22,
		en_US_Wavenet_E = 23,
		en_US_Wavenet_F = 24,
		en_IN_Standard_A = 25,
		en_IN_Standard_B = 26,
		en_IN_Standard_C = 27,
		en_GB_Standard_A = 28,
		en_GB_Standard_B = 29,
		en_GB_Standard_C = 30,
		en_GB_Standard_D = 31,
		en_US_Standard_B = 32,
		en_US_Standard_C = 33,
		en_US_Standard_D = 34,
		en_US_Standard_E = 35,
		en_AU_Standard_A = 36,
		en_AU_Standard_B = 37,
		en_AU_Standard_C = 38,
		en_AU_Standard_D = 39

	}
}

