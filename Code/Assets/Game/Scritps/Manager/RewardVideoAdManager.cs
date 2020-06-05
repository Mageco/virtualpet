using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using MageSDK.Client;
using UnityEngine.Advertisements;
using MageSDK.Client.Helper;
using Mage.Models.Application;
using MageApi;

public class RewardVideoAdManager : MonoBehaviour
{

	public static RewardVideoAdManager instance;
	public RewardType rewardType = RewardType.Minigame;
	RewardBasedVideoAd rewardBasedVideo;
	ChestItem chestItem;
	int petId = 0;
	[HideInInspector]
	public bool isRemoveAd = false;
	[HideInInspector]
	public bool isUnityVideoLoaded = false;
	public string bannerPlacementId = "bannerForest";

#if UNITY_IOS
	private string gameId = "3508454";
#elif UNITY_ANDROID
	private string gameId = "3508455";
#else
    private string gameId = "3508454";
#endif
	string myPlacementId = "rewardedVideo";
	bool testMode = false;
	public AdDistribute adDistribute = AdDistribute.Unity;


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
	IEnumerator Start () {
        while (!MageEngine.instance.IsApplicationDataLoaded())
        {
			yield return new WaitForEndOfFrame();
        }
		Debug.Log(MageEngine.instance.GetApplicationDataItem("CurrentAdDistrubution"));
		if (MageEngine.instance.GetApplicationDataItem("CurrentAdDistrubution") == "Unity")
		{
			adDistribute = AdDistribute.Unity;
		}
		else if (MageEngine.instance.GetApplicationDataItem("CurrentAdDistrubution") == "Admob")
		{
			adDistribute = AdDistribute.Admob;
		}
		else if (MageEngine.instance.GetApplicationDataItem("CurrentAdDistrubution") == "Yodo1MAS")
		{
			adDistribute = AdDistribute.Yodo1MAS;
		}

		// for testing, hardcode to Yodo1MAS
		adDistribute = AdDistribute.Yodo1MAS;
			

        //Get quest
		if (MageEngine.instance.GetApplicationDataItem("QuestMax") != null)
		{
			GameManager.instance.questMax = int.Parse(MageEngine.instance.GetApplicationDataItem("QuestMax"));
		}

		if (adDistribute == AdDistribute.Admob)
        {

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
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Initialize Yodo1MAS");
			
			// dont' collect user information
			Yodo1U3dAds.SetUserConsent(false);
			Yodo1U3dAds.SetLogEnable(true);
			// set user age restriction
			Yodo1U3dAds.SetTagForUnderAgeOfConsent(true);
			// don't sell personal information
			Yodo1U3dAds.SetDoNotSell(true);
			// for Yodo1
			Yodo1U3dAds.InitializeSdk();
			//set callback to handle video reward
			Yodo1U3dSDK.setRewardVideoDelegate(OnYodoVideoAdsHandler);
			//set callback to handle interstitial
			Yodo1U3dSDK.setInterstitialAdDelegate(OnYodoInterstitialAdsHandler);
		}

		if (MageEventHelper.GetInstance().GetEventCounter(MageEventType.ConfirmPaymentItem.ToString()) > 0)
			isRemoveAd = true;
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
		ProcessReward(MageEventType.VideoAdRewarded);
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
			GameManager.instance.OnTreatment(GameManager.instance.GetPetObject(petId), SickType.Sick, 0);
        else if(rewardType == RewardType.Injured)
			GameManager.instance.OnTreatment(GameManager.instance.GetPetObject(petId), SickType.Injured, 0);
	}

    IEnumerator OnMinigame()
    {
		yield return new WaitForEndOfFrame();
		if (Minigame.instance != null && Minigame.instance.winPanel != null)
		{
			Minigame.instance.winPanel.OnWatchedAd();
	    }
	}

	IEnumerator OnMap()
	{
		yield return new WaitForEndOfFrame();
		if (UIManager.instance != null && UIManager.instance.mapRequirementPanel != null)
		{
			UIManager.instance.mapRequirementPanel.OnWatchedAd();
		}
	}

	IEnumerator OnWelcome()
	{
		yield return new WaitForEndOfFrame();
		if (UIManager.instance != null && UIManager.instance.welcomeBackPanel != null)
		{
			UIManager.instance.welcomeBackPanel.WatchedAd();
		}
	}

	IEnumerator OnService()
	{
		yield return new WaitForEndOfFrame();
		if (UIManager.instance != null && UIManager.instance.servicePanel != null)
		{
			UIManager.instance.servicePanel.OnWatchedAd();
		}
	}


	IEnumerator OnForestDiamond()
	{
		yield return new WaitForEndOfFrame();
		if (UIManager.instance != null && UIManager.instance.rewardDiamondPanel != null)
		{
			UIManager.instance.rewardDiamondPanel.WatchedAd();
		}
	}

	IEnumerator OnSpinWheel()
	{
		yield return new WaitForEndOfFrame();
		if (UIManager.instance != null && UIManager.instance.spinWheelPanel != null)
		{
			UIManager.instance.spinWheelPanel.OnWatched();
		}
	}

	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
	{

		Debug.Log("HandleRewardBasedVideoLeftApplication event received");
	}

	public void ShowVideoAd()
	{
        if(adDistribute == AdDistribute.Admob)
        {
			if (rewardBasedVideo.IsLoaded())
			{
				rewardBasedVideo.Show();
			}
		}
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			Yodo1U3dAds.ShowVideo();
		} 

	}

	public void ShowVideoAd(RewardType type)
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (rewardBasedVideo.IsLoaded())
			{
				rewardType = type;
				rewardBasedVideo.Show();
			}
		}
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS VideoAds");
			rewardType = type;
			Yodo1U3dAds.ShowVideo();
		} 
	}

	public void ShowVideoAd(RewardType type,int petId)
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (rewardBasedVideo.IsLoaded())
			{
				rewardType = type;
				rewardBasedVideo.Show();
				this.petId = petId;
			}
		}
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS VideoAds - Pet");
			rewardType = type;
			this.petId = petId;
			Yodo1U3dAds.ShowVideo();
		} 
	}

	public void ShowVideoAd(RewardType type,ChestItem item)
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (rewardBasedVideo.IsLoaded())
			{
				rewardType = type;
				rewardBasedVideo.Show();
				chestItem = item;
			}
		}
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS VideoAds - Chest");
			rewardType = type;
			Yodo1U3dAds.ShowVideo();
			chestItem = item;
		} 
	}

	public void ShowIntetestial()
	{
		if (isRemoveAd)
			return;
		if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS Interstitial");
			rewardType = RewardType.None;
			Yodo1U3dAds.ShowInterstitial();
			MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdShow, "Minigame");
		} 
	}

    public void ShowBanner()
    {
		if (isRemoveAd)
			return;
	}

    public void HideBanner()
    {
    }


	#region Unity Ad

	// Implement IUnityAdsListener interface methods:
	/*public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
	{
		// Define conditional logic for each ad completion status:
		if (showResult == ShowResult.Finished)
		{
			ProcessReward(MageEventType.VideoAdRewarded);
		}
		else if (showResult == ShowResult.Skipped)
		{
			// Do not reward the user for skipping the ad.
		}
		else if (showResult == ShowResult.Failed)
		{
			Debug.LogWarning("The ad did not finish due to an error.");
		}
	}*/

	public void OnUnityAdsReady(string placementId)
	{
        if(placementId == "rewardedVideo")
        {
			isUnityVideoLoaded = true;
        }
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


	#region Yodo1MAS

	public void OnYodoInterstitialAdsHandler(Yodo1U3dConstants.AdEvent adEvent,string error)
	{
		Debug.Log ("InterstitialAdDelegate:" + adEvent + "\n" + error);
		switch (adEvent)
		{
			case Yodo1U3dConstants.AdEvent.AdEventClick:
				break;
			case Yodo1U3dConstants.AdEvent.AdEventClose:
				break;
			case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
					this.ProcessReward(MageEventType.InterstitialAdShow);
				break;
			case Yodo1U3dConstants.AdEvent.AdEventShowFail:
				ApiUtils.Log("Interstital ad has been show failed, the error message:" + error);
				break;
		}
	}

	public void OnYodoVideoAdsHandler(Yodo1U3dConstants.AdEvent adEvent,string error)
	{
		ApiUtils.Log("RewardVideoDelegate:" + adEvent + "\n" + error);
		switch (adEvent)
		{
			case Yodo1U3dConstants.AdEvent.AdEventClick:
				ApiUtils.Log("Rewarded video ad has been clicked.");
				break;
			case Yodo1U3dConstants.AdEvent.AdEventClose:
				ApiUtils.Log("Rewarded video ad has been closed.");
				break;
			case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
				ApiUtils.Log("Rewarded video ad has shown successful.");
				break;
			case Yodo1U3dConstants.AdEvent.AdEventShowFail:
				ApiUtils.Log("Rewarded video ad show failed, the error message:" + error);
				break;
			case Yodo1U3dConstants.AdEvent.AdEventFinish:
				ApiUtils.Log("Rewarded video ad has been played finish, give rewards to the player.");
				this.ProcessReward(MageEventType.VideoAdRewarded);
			break;
		}
	}

	private void ProcessReward(MageEventType adsType)
	{
		// Reward the user for watching the ad to completion.
		MageEngine.instance.OnEvent(adsType, rewardType.ToString());
		if (rewardType == RewardType.Minigame)
		{
			StartCoroutine(OnMinigame());
		}
		else if (rewardType == RewardType.Chest)
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
		else if (rewardType == RewardType.Welcome)
		{
			StartCoroutine(OnWelcome());
		}
		else if (rewardType == RewardType.Service)
		{
			StartCoroutine(OnService());
		}
		else if (rewardType == RewardType.ForestDiamond)
		{
			StartCoroutine(OnForestDiamond());
		}
		else if (rewardType == RewardType.SpinWheel)
		{
			StartCoroutine(OnSpinWheel());
		}
	}

	#endregion
    
}

