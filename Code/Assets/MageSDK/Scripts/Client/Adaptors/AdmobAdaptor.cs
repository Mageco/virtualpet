#if USE_UNITY_ADMOB && !UNITY_STANDALONE
using System;
using GoogleMobileAds.Api;
using Mage.Models.Application;
using MageApi;
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
        public RewardBasedVideoAd rewardBasedVideo;
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

        public static Action<MageEventType> processMageEventType;

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
            this.rewardBasedVideo = RewardBasedVideoAd.Instance;

            // Called when an ad request has successfully loaded.
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            // Called when an ad request failed to load.
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            // Called when an ad is shown.
            rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            // Called when the ad starts to play.
            rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            // Called when the user should be rewarded for watching a video.
            rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            // Called when the ad is closed.
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            // Called when the ad click caused the user to leave the application.
            rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

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
            this.RequestRewardBasedVideo();

            this.RequestInterstitial();
        }

        private void RequestRewardBasedVideo()
        {
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();

            // Load the rewarded video ad with the request.
            this.rewardBasedVideo.LoadAd(request, videoAdUnitId);
        }

        private void RequestInterstitial()
        {
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();

            // Load the interstitial with the request.
            interstitial.LoadAd(request);
        }

        public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoLoaded event received");
        }

        public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
        }

        public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoOpened event received");
        }

        public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoStarted event received");
        }

        public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoClosed event received");
            RequestRewardBasedVideo();
        }

        public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleAdmobAdaptor.GetInstance().rewardBasedVideoLeftApplication event received");
        }


        public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            processMageEventType(MageEventType.VideoAdRewarded);
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
            Debug.Log("HandleAdOpened event received");
            processMageEventType(MageEventType.InterstitialAdShow);
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
            if (this.rewardBasedVideo.IsLoaded())
            {
                this.rewardBasedVideo.Show();
            }
        }

        public void ShowInterstitial()
        {
            if (this.interstitial.IsLoaded())
            {
                this.interstitial.Show();
            }
        }
    }

}
#endif