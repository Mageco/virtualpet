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
using Mage.Models.Application;
using MageSDK.Client;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MageApi
{
    public class ApiHandler
    {

        /*public bool TestMode;
        public string TestUUID;
        public string ApplicationSecretKey = "";
        public string ApplicationKey = "";
        public string ApiVersion = "";
        public string ApiUrl = "http://localhost:8080/portal/api";*/

        public float TimeOut = 200;


        [HideInInspector]
        private static ApiHandler _instance;

        public static ApiHandler GetInstance()
        {
            if (_instance == null)
                _instance = new ApiHandler();

            return _instance;
        }

        public void Initialize()
        {
            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_VERSION, MageEngine.instance.ApiVersion);

#if UNITY_EDITOR
            if (MageEngine.instance.TestMode)
            {

                if (MageEngine.instance.TestUUID == "")
                {
                    RuntimeParameters.GetInstance().SetParam(ApiSettings.DEVICE_ID, ApiUtils.GetInstance().GenerateGuid());
                }
                else
                {
                    RuntimeParameters.GetInstance().SetParam(ApiSettings.DEVICE_ID, MageEngine.instance.TestUUID);
                }
            }
            else
            {
                RuntimeParameters.GetInstance().SetParam(ApiSettings.DEVICE_ID, ApiUtils.GetInstance().GetDeviceID());
            }
#else
			RuntimeParameters.GetInstance().SetParam (ApiSettings.DEVICE_ID, ApiUtils.GetInstance().GetDeviceID());
#endif

            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_KEY, ApiUtils.GetInstance().GenerateApiKey(RuntimeParameters.GetInstance().GetParam(ApiSettings.DEVICE_ID).ToString(), MageEngine.instance.ApiVersion));
            RuntimeParameters.GetInstance().SetParam(ApiSettings.DEVICE_TYPE, ApiUtils.GetInstance().GetDeviceType());
            RuntimeParameters.GetInstance().SetParam(ApiSettings.APPLICATION_VERSION, Application.version);
            RuntimeParameters.GetInstance().SetParam(ApiSettings.SYSTEM_LANGUAGE, Application.systemLanguage.ToString());
            RuntimeParameters.GetInstance().SetParam(ApiSettings.API_SERVER_TIMESTAMP, DateTime.Now);

        }

        

    }
}

