using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MageApi;
using MageApi.Models;
using MageApi.Models.Request;
using MageApi.Models.Response;
using SimpleJSON;
using UnityEngine;

namespace MageSDK.Client
{
    public class MageEngineSettings
    {
        public const string GAME_ENGINE_OWNER = "OwnerInfo";

        public const string GAME_ENGINE_USER = "GameEngineUser";

        public const string
            GAME_ENGINE_APPLICATION_DATA = "GameEngineApplicationData";

        public const string
            GAME_ENGINE_DEFAULT_CHARACTER_DATA = "DefaultCharacterData";

        public const string GAME_ENGINE_DEFAULT_USER_DATA = "DefaultUserData";

        public const string GAME_ENGINE_ACTION_LOGS = "MageActionLogs";

        public const string
            GAME_ENGINE_ACTION_LOGS_KEY_LOOKUP = "MageActionLogsKeyLookup";

        public const string
            GAME_ENGINE_ACTION_LOGS_COUNTER = "MageActionLogsCounter";

        public const string GAME_ENGINE_VARIABLES = "Variables";

        public const string
            GAME_ENGINE_MIN_USER_DATA_UPDATE_DURATION
            =
            "MinUserDataUpdateDuration";

        public const string
            GAME_ENGINE_ANDROID_SIGNATURE_SHA1 = "AndroidSignatureSHA1";

        public const string
            GAME_ENGINE_ENFORECED_ANDROID_SIGNATURE
            =
            "EnforcedAndroidSignature";

        public const string GAME_ENGINE_EVENT_CACHE = "MageEventCache";

        public const string
            GAME_ENGINE_SCREEN_TIME_CACHE = "MageScreenTimeCache";

        public const string
            GAME_ENGINE_SCREEN_TIME_CACHE_PREFIX = "screenTime_";

        public const string GAME_ENGINE_LAST_SCREEN = "MageLastScreen";

        public const string
            GAME_ENGINE_LAST_SCREEN_TIMESTAMP = "MageLastScreenTimeStamp";

        public const string
            GAME_ENGINE_EVENT_COUNTER_CACHE = "MageEventCounterCache";

        public const string
            GAME_ENGINE_EVENT_COUNTER_CACHE_PREFIX = "eventCounter_";

        public const string
            GAME_ENGINE_MAX_EVENT_COUNTER_QUEUE = "MaxEventCounterQueue";

        public const string
            GAME_ENGINE_FB_LIST_OF_REMOTE_CONFIG_KEYS
            =
            "FirebaseListOfRemoteConfigKeys";

        public const string GAME_ENGINE_FB_USER_ID = "FirebaseUserId";

        // messages sexxion
        public const string GAME_ENGINE_USER_MESSAGE = "GameEngineUserMessages";

        public const string GAME_ENGINE_ADS_DISTRIBUTION = "AdsDistribution";

        public const string
            GAME_ENGINE_ADS_UNIT_CONFIGURAIONS = "AdsUnitConfigurations";

        public const string
            GAME_ENGINE_TIME_LAP_INTERSTITIAL = "TimeLapInterstitial";

        public static string[]
            GAME_ENGINE_APPLICATION_DATA_ITEM =
            {
                "DefaultUserData",
                "DefaultCharacterData",
                "MinUserDataUpdateDuration",
                "MaxEventCounterQueue",
                "OwnerInfo",
                "AdsDistribution",
                "AdsUnitConfigurations",
                "GameEngine_DataConfiguration"
            };
    }

    public enum ClientLoginMethod
    {
        LOGIN_DEVICE_UUID = 0,
        LOGIN_FACEBOOK = 1,
        LOGIN_GOOGLE = 2,
        LOGIN_USERNAME_PASSWORD = 3
    }
}
