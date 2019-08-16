using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class RewardVideoAdManager : MonoBehaviour {

	public static RewardVideoAdManager instance;

	int currentId = 0; 
	RewardBasedVideoAd rewardBasedVideo;

	#if UNITY_ANDROID
	string appId = "ca-app-pub-6818802678275174~9883498495";
	#elif UNITY_IPHONE
	string appId = "ca-app-pub-6818802678275174~9883498495";
	#else
	string appId = "unexpected_platform";
	#endif

	#if UNITY_ANDROID
	string adUnitId = "ca-app-pub-6818802678275174/8718590638";
	#elif UNITY_IPHONE
	string adUnitId = "ca-app-pub-6818802678275174/1271203231";
	#else
	string adUnitId = "unexpected_platform";
	#endif


	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			GameObject.Destroy (this.gameObject);

		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
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
	
	// Update is called once per frame
	void Update () {
		
	}

	public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoLoaded event received");
		ApiManager.instance.SendAppEvent ("VideoAdLoaded");
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoFailedToLoad event received with message: "+ args.Message);
		ApiManager.instance.SendAppEvent ("VideoAdFailtoLoaded");
	}

	public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoOpened event received");
		ApiManager.instance.SendAppEvent ("VideoAdOpened");
	}

	public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoStarted event received");
		ApiManager.instance.SendAppEvent ("VideoAdStarted");
	}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoClosed event received");
		ApiManager.instance.SendAppEvent ("VideoAdClosed");
		RequestRewardBasedVideo ();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		ApiManager.instance.SendAppEvent ("VideoAdRewarded");
		ApiManager.instance.AddCoin (UnityEngine.Random.Range (1,10));
	}


	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoLeftApplication event received");
	}

	public void ShowAd()
	{
		if (rewardBasedVideo.IsLoaded()) {
			rewardBasedVideo.Show();
		}
	}

}
