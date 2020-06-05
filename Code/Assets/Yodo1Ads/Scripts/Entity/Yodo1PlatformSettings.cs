using System;
using UnityEngine;

namespace Yodo1Ads
{
    [Serializable]
    public class Yodo1PlatformSettings
    {
        public string AppKey;
        public string AdmobAppID;

        public Yodo1PlatformSettings()
        {
            this.AppKey = string.Empty;
            this.AdmobAppID = string.Empty;
        }
    }
}



