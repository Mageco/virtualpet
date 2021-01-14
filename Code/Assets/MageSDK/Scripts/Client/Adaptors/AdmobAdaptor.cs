#if USE_GOOGLE_ADMOB && !UNITY_STANDALONE
using System;
using GoogleMobileAds.Api;
using Mage.Models;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageSDK.Client;
using MageSDK.Client.Helper;
using UnityEngine;
using UnityEngine.Advertisements;

namespace MageSDK.Client.Adaptors
{
    public class AdmobAdaptor
    {

#if UNITY_ANDROID
        string appId = "ca-app-pub-8639015397874051~1569311006";
#elif UNITY_IOS
        string appId = "ca-app-pub-8639015397874051~9147777490";
#else
        string appId = "unexpected_platform";
#endif

#if UNITY_ANDROID
        string videoAdUnitId = "ca-app-pub-8639015397874051/4087022500";
        string interstitialAdUnitId = "ca-app-pub-8639015397874051/3896091447";
#elif UNITY_IOS
        string videoAdUnitId = "ca-app-pub-8639015397874051/3844120555";
        string interstitialAdUnitId = "ca-app-pub-8639015397874051/5157202229";
#else
        string videoAdUnitId = "unexpected_platform";
        string interstitialAdUnitId = "unexpected_platform";
#endif
        private static AdmobAdaptor _instance;
        public RewardedAd rewardedAd;
        public InterstitialAd interstitial;

        public AdmobAdaptor()
        {
        }

        public static AdmobAdaptor GetInstance()
        {
            if (null == _instance)
            {
                _instance = new AdmobAdaptor();
            }
            return _instance;
        }

        public Action<MageEventType> processMageEventType;

        ///<summary>Initialize Unity Ads</summary>
        public void Initialize(Action<MageEventType> processMageEventTypeCallback)
        {
            AdsConfigurations adsConfigurations = MageEngine.instance.GetApplicationDataItem<AdsConfigurations>(MageEngineSettings.GAME_ENGINE_ADS_UNIT_CONFIGURAIONS);

#if UNITY_ANDROID
            appId = adsConfigurations.admobAndroidAppId;
            videoAdUnitId = adsConfigurations.admobAndroidVideoUnitId;
            interstitialAdUnitId = adsConfigurations.admobAndroidInterstitialUnitId;
#elif UNITY_IOS
            appId = adsConfigurations.admobIOSAppId;
            videoAdUnitId = adsConfigurations.admobIOSVideoUnitId;
            interstitialAdUnitId = adsConfigurations.admobIOSInterstitialUnitId;
#endif
            processMageEventType = processMageEventTypeCallback;
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);

            // Get singleton reward based video ad reference.
            this.rewardedAd = new RewardedAd(videoAdUnitId);

            // Called when an ad request has successfully loaded.
            this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(interstitialAdUnitId);

            // Called when an ad request has successfully loaded.
            interstitial.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when an ad is shown.
            interstitial.OnAdOpening += HandleOnAdOpened;
            // Called when the ad is closed.
            interstitial.OnAdClosed += HandleOnAdClosed;
            // Called when the ad click caused the user to leave the application.
            interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

            // initialize at begining
            this.RequestRewardedAd();

            this.RequestInterstitial();

        }

        private void RequestRewardedAd()
        {
            // Create an empty ad request.
            //AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();
            AdRequest request = new AdRequest.Builder().Build();

            // Load the rewarded video ad with the request.
            this.rewardedAd.LoadAd(request);
        }

        private void RequestInterstitial()
        {
            // Create an empty ad request.
            //AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            this.interstitial.LoadAd(request);
        }


        public void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardedAdLoaded event received");
        }

        public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
        {
            ApiUtils.Log(
                "HandleRewardedAdFailedToLoad event received with message: "
                                 + args.Message);
        }

        public void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardedAdOpening event received");
        }

        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            ApiUtils.Log(
                "HandleRewardedAdFailedToShow event received with message: "
                                 + args.Message);
        }

        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardedAdClosed event received");
            RequestRewardedAd();
        }

        public void HandleUserEarnedReward(object sender, Reward args)
        {
            MageEngine.instance.EnqueueCallbackTask(processMageEventType, new object[] { MageEventType.VideoAdRewarded });
        }

        public void HandleOnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("HandleAdLoaded event received");
        }

        public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
        }

        public void HandleOnAdOpened(object sender, EventArgs args)
        {
            MageEngine.instance.EnqueueCallbackTask(processMageEventType, new object[] { MageEventType.InterstitialAdShow });
        }

        public void HandleOnAdClosed(object sender, EventArgs args)
        {
            this.RequestInterstitial();
            Debug.Log("HandleAdClosed event received");
        }

        public void HandleOnAdLeavingApplication(object sender, EventArgs args)
        {
        }

        public void ShowVideoAd()
        {
            if (this.rewardedAd.IsLoaded())
            {
                this.rewardedAd.Show();
            }
            else
            {
                this.Initialize(this.processMageEventType);
            }
        }

        public void ShowInterstitial()
        {
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            }
            else
            {
                this.Initialize(this.processMageEventType);
            }
        }
    }

}

#endif
