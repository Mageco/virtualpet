using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using MageSDK.Client;

public class RewardVideoAdManager : MonoBehaviour {

	public static RewardVideoAdManager instance;
	public RewardType rewardType = RewardType.Minigame;
	RewardBasedVideoAd rewardBasedVideo;
	ChestItem chestItem;
	int petId = 0;

	#if UNITY_ANDROID
	string appId = "ca-app-pub-6818802678275174~2905900525";
#elif UNITY_IPHONE
	string appId = "ca-app-pub-6818802678275174~2905900525";
#else
	string appId = "unexpected_platform";
#endif

    #if UNITY_ANDROID
	string adUnitId = "ca-app-pub-6818802678275174/9014893744";
    //Test unit
	//string adUnitId = "ca-app-pub-3940256099942544/5224354917";
	#elif UNITY_IPHONE
	string adUnitId = "ca-app-pub-6818802678275174/5961180954";
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
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdLoaded,rewardType.ToString());
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoFailedToLoad event received with message: "+ args.Message);
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdFailtoLoaded, rewardType.ToString());
	}

	public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoOpened event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdOpened, rewardType.ToString());
	}

	public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoStarted event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdStarted, rewardType.ToString());
	}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoClosed event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdClosed, rewardType.ToString());
		RequestRewardBasedVideo ();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdRewarded, rewardType.ToString());
        if(rewardType == RewardType.Minigame)
        {
			StartCoroutine(OnMinigame());
		}
        else if(rewardType == RewardType.Chest)
        {
			StartCoroutine(OnRewardChest());
		}
		else if (rewardType == RewardType.Sick)
		{
			StartCoroutine(OnTreatment());
		}
		else if (rewardType == RewardType.Map)
		{
			StartCoroutine(OnMap());
		}

	}

    IEnumerator OnRewardChest()
    {
		yield return new WaitForSeconds(0.5f);
		if (chestItem != null)
			chestItem.OnActive();
	}

    IEnumerator OnTreatment()
    {
		yield return new WaitForSeconds(0.5f);
		if (rewardType == RewardType.Sick)
			GameManager.instance.OnTreatment(GameManager.instance.GetPet(petId), SickType.Sick, 0);
        else if(rewardType == RewardType.Injured)
			GameManager.instance.OnTreatment(GameManager.instance.GetPet(petId), SickType.Injured, 0);
	}

    IEnumerator OnMinigame()
    {
		yield return new WaitForEndOfFrame();
		if (Minigame.instance != null)
		{
			Minigame.instance.winPanel.OnWatchedAd();
	    }
	}

	IEnumerator OnMap()
	{
		yield return new WaitForEndOfFrame();
		if (UIManager.instance != null)
		{
			UIManager.instance.mapRequirementPanel.OnWatchedAd();
		}
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

	public void ShowAd(RewardType type)
	{
		//MageManager.instance.OnNotificationPopup("Ad requested");
		if (rewardBasedVideo.IsLoaded())
		{
			rewardType = type;
			rewardBasedVideo.Show();
		}
	}

	public void ShowAd(RewardType type,int petId)
	{
		//MageManager.instance.OnNotificationPopup("Ad requested");
		if (rewardBasedVideo.IsLoaded())
		{
			rewardType = type;
			rewardBasedVideo.Show();
			this.petId = petId;
		}
	}

	public void ShowAd(RewardType type,ChestItem item)
	{
		//MageManager.instance.OnNotificationPopup("Ad requested");
		if (rewardBasedVideo.IsLoaded())
		{
			rewardType = type;
			rewardBasedVideo.Show();
			chestItem = item;
		}
	}

}

