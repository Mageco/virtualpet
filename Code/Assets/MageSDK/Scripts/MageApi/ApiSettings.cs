
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MageApi {
	public class ApiSettings {
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
		public const string API_UPDATE_PROFILE = "UpdateProfile";
		public const string API_UPDATE_USER_DATA = "UpdateUserData";
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

		//defines Login method
		public const string LOGIN_DEVICE_UUID = "0";
		public const string LOGIN_FACEBOOK = "1";
		public const string LOGIN_GOOGLE = "2";
		public const string LOGIN_USERNAME_PASSWORD = "3";


	}
}

