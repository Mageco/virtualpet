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
using Mage.Models.Application;
using Mage.Models.Users;
#if USE_UNITY_ADMOB && !UNITY_STANDALONE
using MageSDK.Client.Adaptors;
#endif

namespace MageSDK.Client.Helper
{
    public class MageAdsHelper
    {
        private static MageAdsHelper _instance;

        private bool _isInitialized = false;

        private AdsConfigurations adsConfigurations;

        private Action<MageEventType> processMageEventType;

        public MageAdsHelper()
        {
            //load something from local
        }

        public static MageAdsHelper GetInstance()
        {
            if (null == _instance)
            {
                _instance = new MageAdsHelper();
            }
            return _instance;
        }

        #region functions

        public void Initialize(Action<MageEventType> processMageEventTypeCallback)
        {
            // select ads distribution network
            processMageEventType = processMageEventTypeCallback;

            adsConfigurations = MageEngine.instance.GetApplicationDataItem<AdsConfigurations>(MageEngineSettings.GAME_ENGINE_ADS_UNIT_CONFIGURAIONS);
            Debug.Log("Json format ads: " + adsConfigurations.ToJson());


#if UNITY_EDITOR
            adsConfigurations.videoDistributor = AdDistribute.Unity;
            adsConfigurations.interstitialDistributor = AdDistribute.Unity;
#endif

            // Initialize correspondent Ads network
#if USE_UNITY_ADMOB && !UNITY_STANDALONE
            foreach(AdDistribute adDistribute in adsConfigurations.adDistributors) {
                switch (adDistribute)
                {
                    case AdDistribute.Admob:
                        AdmobAdaptor.GetInstance().Initialize(processMageEventType);
                        break;
#if YODO1MAS_ENABLED
                    case AdDistribute.Yodo1MAS:
                        YodoMASAdaptor.GetInstance().Initialize(processMageEventType);
                        break;
#endif
#if IRON_SOURCE_ENABLED
                    case AdDistribute.IronSource:
                        IronSourceAdaptor.GetInstance().Initialize(processMageEventType);
                        break;
#endif
                    case AdDistribute.Unity:
                    default:
                        UnityAdsAdaptor.GetInstance().Initialize(processMageEventType);
                        break;
                }
            }
#endif  
            this._isInitialized = true;
        }

        public void ShowVideoAd()
        {
            if (!IsInitialized())
            {
                return;
            }
#if USE_UNITY_ADMOB && !UNITY_STANDALONE
        switch (adsConfigurations.videoDistributor)
        {
            case AdDistribute.Admob:
                AdmobAdaptor.GetInstance().ShowVideoAd();
                break;
            case AdDistribute.Yodo1MAS:
#if YODO1MAS_ENABLED
				ApiUtils.Log("Show Yodo1MAS VideoAds");
				Yodo1U3dAds.ShowVideo();
#endif
                break;
            case AdDistribute.IronSource:
#if IRON_SOURCE_ENABLED
				ApiUtils.Log("Show Iron Source VideoAds");
				IronSource.Agent.showRewardedVideo();
#endif
                break;
            case AdDistribute.Unity:
            default:
                ApiUtils.Log("Show Unity Ads VideoAds");
                UnityAdsAdaptor.GetInstance().ShowVideoAd();
                break;
        }
#else
            processMageEventType(MageEventType.VideoAdRewarded);
#endif
        }

        public void ShowInterstitialAd()
        {
            if (!IsInitialized())
            {
                return;
            }
#if USE_UNITY_ADMOB && !UNITY_STANDALONE
        switch (adsConfigurations.interstitialDistributor)
        {
            case AdDistribute.Admob:
                AdmobAdaptor.GetInstance().ShowInterstitial();
                break;
            case AdDistribute.Yodo1MAS:
#if YODO1MAS_ENABLED
				ApiUtils.Log("Show Yodo1MAS Interstitial");
				Yodo1U3dAds.ShowInterstitial();
#endif
                break;
            case AdDistribute.IronSource:
#if IRON_SOURCE_ENABLED
				ApiUtils.Log("Show IronSource Interstitial");
				IronSource.Agent.showInterstitial();
#endif
                break;
            case AdDistribute.Unity:
            default:
                ApiUtils.Log("Show UnityAds Interstitial");
                UnityAdsAdaptor.GetInstance().ShowInterstitialAd();
                break;

        }
#else
            processMageEventType(MageEventType.InterstitialAdShow);
#endif
        }

        private bool IsInitialized()
        {
            return _isInitialized;
        }

        public string GetVideoDistributor()
        {
            return adsConfigurations.videoDistributor.ToString();
        }

        public string GetInterstitialDistributor()
        {
            return adsConfigurations.interstitialDistributor.ToString();
        }
        #endregion
    }



}

