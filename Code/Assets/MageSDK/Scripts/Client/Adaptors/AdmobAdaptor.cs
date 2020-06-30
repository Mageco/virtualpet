using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.RemoteConfig;
using GoogleMobileAds.Api;
using Mage.Models;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.Advertisements;

namespace MageSDK.Client.Adaptors 
{
	public class AdmobAdaptor 
	{
        
        #if UNITY_ANDROID
            string appId = "ca-app-pub-6818802678275174~2905900525";
        #elif UNITY_IPHONE
            string appId = "ca-app-pub-6818802678275174~2905900525";
        #else
            string appId = "unexpected_platform";
        #endif

        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-6818802678275174/9014893744";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-6818802678275174/5961180954";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        private static AdmobAdaptor _instance;
        public RewardBasedVideoAd rewardBasedVideo;

        public AdmobAdaptor() {
		}

		public static AdmobAdaptor GetInstance() {
			if (null == _instance) {
				_instance = new AdmobAdaptor ();
			} 
			return _instance;
		}

        public static Action<MageEventType> processMageEventType;

		///<summary>Initialize Unity Ads</summary>
		public void Initialize(Action<MageEventType> processMageEventTypeCallback) {
            processMageEventType = processMageEventTypeCallback;
            // Initialize the Google Mobile Ads SDK.
			MobileAds.Initialize(appId);

			// Get singleton reward based video ad reference.
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

			this.RequestRewardBasedVideo();
		}

        private void RequestRewardBasedVideo()
        {
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();

            // Load the rewarded video ad with the request.
            this.rewardBasedVideo.LoadAd(request, adUnitId);
        }

        public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoLoaded event received");
        }

        public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            ApiUtils.Log("HandleRewardBasedVideoFailedToLoad event received with message: "+ args.Message);
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
            RequestRewardBasedVideo ();
        }
        
        public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            ApiUtils.Log("HandleAdmobAdaptor.GetInstance().rewardBasedVideoLeftApplication event received");
        }
        
        
        public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            processMageEventType(MageEventType.VideoAdRewarded);
        }
	}
		
}
