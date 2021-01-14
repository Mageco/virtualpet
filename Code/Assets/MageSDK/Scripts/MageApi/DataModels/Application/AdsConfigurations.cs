using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Reflection;
using System;
using Mage.Models;

namespace Mage.Models.Application
{
    
    [Serializable]
    public class AdsConfigurations : BaseModel
    {
        
        public List<AdDistribute> adDistributors = new List<AdDistribute>();

        public AdDistribute videoDistributor = AdDistribute.Unity;
        public AdDistribute interstitialDistributor = AdDistribute.Unity;

        #region Admob configurations
        public string admobIOSAppId = "";
        public string admobIOSVideoUnitId = "";
        public string admobIOSInterstitialUnitId = "";
        public string admobAndroidAppId = "";
        public string admobAndroidVideoUnitId = "";
        public string admobAndroidInterstitialUnitId = "";

        #endregion Admob configurations

        #region Unity configurations
        public string unityIOSGameId = "";
        public string unityAndroidGameId = "";
        public string unityAndroidVideoUnitId = "";
        public string unityAndroidInterstitialUnitId = "";
        public string unityIOSVideoUnitId = "";
        public string unityIOSInterstitialUnitId = "";
        #endregion Unity configurations

        public AdsConfigurations() {
            adDistributors = new List<AdDistribute>() {AdDistribute.Admob, AdDistribute.Unity};
        }

    }

}

