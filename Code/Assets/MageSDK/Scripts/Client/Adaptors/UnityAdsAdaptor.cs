#if USE_UNITY_ADS && !UNITY_STANDALONE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.RemoteConfig;
using Mage.Models;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.Advertisements;

namespace MageSDK.Client.Adaptors
{
    public class UnityAdsAdaptor : IUnityAdsListener
    {

#if UNITY_IOS
            private static string gameId = "3508454";
#elif UNITY_ANDROID
        private static string gameId = "3508455";
#else
            private static string gameId = "3508454";
#endif

        private static UnityAdsAdaptor _instance;
        public static string rewardedVideoPlacementId = "rewardedVideo";
        public static string interstitialPlacementId = "Interstitial";

        private static bool testMode = false;

        public Action<MageEventType> processMageEventType;

        public UnityAdsAdaptor()
        {
        }

        public static UnityAdsAdaptor GetInstance()
        {
            if (null == _instance)
            {
                _instance = new UnityAdsAdaptor();
            }
            return _instance;
        }

        ///<summary>Initialize Unity Ads</summary>
        public void Initialize(Action<MageEventType> processMageEventTypeCallback)
        {
            AdsConfigurations adsConfigurations = MageEngine.instance.GetApplicationDataItem<AdsConfigurations>(MageEngineSettings.GAME_ENGINE_ADS_UNIT_CONFIGURAIONS);
            ApiUtils.Log("Initialized Unity Ads: " + adsConfigurations.ToJson());
#if UNITY_IOS
            gameId = adsConfigurations.unityIOSGameId;
            UnityAdsAdaptor.rewardedVideoPlacementId = adsConfigurations.unityIOSVideoUnitId;
            UnityAdsAdaptor.interstitialPlacementId = adsConfigurations.unityIOSInterstitialUnitId;
#elif UNITY_ANDROID
            gameId = adsConfigurations.unityAndroidGameId;
            UnityAdsAdaptor.rewardedVideoPlacementId = adsConfigurations.unityAndroidVideoUnitId;
            UnityAdsAdaptor.interstitialPlacementId = adsConfigurations.unityAndroidInterstitialUnitId;
#else
            gameId = adsConfigurations.unityIOSGameId;
            UnityAdsAdaptor.rewardedVideoPlacementId = adsConfigurations.unityIOSVideoUnitId;
            UnityAdsAdaptor.interstitialPlacementId = adsConfigurations.unityIOSInterstitialUnitId;
#endif


            Advertisement.AddListener(this);
            Advertisement.Initialize(gameId, testMode);

            processMageEventType = processMageEventTypeCallback;
        }

        #region Unity Ad

        // Implement IUnityAdsListener interface methods:
        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                if (placementId == rewardedVideoPlacementId)
                {
                    processMageEventType(MageEventType.VideoAdRewarded);
                }
                if (placementId == interstitialPlacementId)
                {
                    processMageEventType(MageEventType.InterstitialAdShow);
                }

            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
                if (placementId == interstitialPlacementId)
                {
                    processMageEventType(MageEventType.InterstitialAdShow);
                }
                else
                {
                    //MageManager.instance.OnNotificationPopup(157);
                }
            }
            else if (showResult == ShowResult.Failed)
            {
                if (placementId == rewardedVideoPlacementId)
                {
                    //MageManager.instance.OnNotificationPopup(157);
                }
                Debug.LogWarning("The ad did not finish due to an error.");
            }
        }

        public void OnUnityAdsReady(string placementId)
        {

        }

        public void OnUnityAdsDidError(string message)
        {
            // Log the error.
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            // Optional actions to take when the end-users triggers an ad.
        }
        #endregion

        private void ShowUnityAd(string placementId)
        {
            if (!Advertisement.isInitialized)
            {
                if (this.processMageEventType != null)
                {
                    Initialize(this.processMageEventType);
                }
            }
            if (Advertisement.IsReady())
            {
                Advertisement.Show(placementId);
                ApiUtils.Log("Unity ad is ready:" + placementId);
            }
            else
            {
                ApiUtils.Log("Unity ad is not ready:" + placementId);
            }
        }

        public void ShowVideoAd()
        {
            if (!Advertisement.isInitialized)
            {
                if (this.processMageEventType != null)
                {
                    Initialize(this.processMageEventType);
                }
            }
            ShowUnityAd(UnityAdsAdaptor.rewardedVideoPlacementId);
        }

        public void ShowInterstitialAd()
        {
            if (!Advertisement.isInitialized)
            {
                if (this.processMageEventType != null)
                {
                    Initialize(this.processMageEventType);
                }
            }
            ShowUnityAd(UnityAdsAdaptor.interstitialPlacementId);
        }
    }

}
#endif