using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using MageSDK.Client;
using UnityEngine.Advertisements;
using MageSDK.Client.Helper;
using Mage.Models.Application;

public class RewardVideoAdManager : MonoBehaviour , IUnityAdsListener
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
    //unity banner
	public string bannerPlacementId = "bannerForest";

    //Unity ID
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


    //Admob Id
#if UNITY_ANDROID
	string appId = "ca-app-pub-6818802678275174~2905900525";
#elif UNITY_IPHONE
	string appId = "ca-app-pub-6818802678275174~2905900525";
#else
	string appId = "unexpected_platform";
#endif


	//Admob Id
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


    #region Start
    // Use this for initialization
    IEnumerator Start () {

        //Wait Data
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
			adDistribute = AdDistribute.Admob;

        //Get quest
		if (MageEngine.instance.GetApplicationDataItem("QuestMax") != null)
		{
			GameManager.instance.questMax = int.Parse(MageEngine.instance.GetApplicationDataItem("QuestMax"));
		}

        //Admob
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

			this.RequestAdmobRewardBasedVideo();
        //Unity   
		}else if(adDistribute == AdDistribute.Unity)
        {
			Advertisement.AddListener(this);
			Advertisement.Initialize(gameId, testMode);
		}

		if (MageEventHelper.GetInstance().GetEventCounter(MageEventType.ConfirmPaymentItem.ToString()) > 0)
			isRemoveAd = true;
	}
    #endregion

    

    #region Banner Video
    public void ShowBanner()
	{
		if (isRemoveAd)
			return;
		if (adDistribute == AdDistribute.Unity)
        {
			Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
			StartCoroutine(ShowBannerWhenReady());
		}

		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.BannerAdShow, "Scene");
	}

	public void HideBanner()
	{
		if (adDistribute == AdDistribute.Unity)
			Advertisement.Banner.Hide();
	}

	IEnumerator ShowBannerWhenReady()
	{
		while (!Advertisement.IsReady(bannerPlacementId))
		{
			yield return new WaitForEndOfFrame();
		}
		Advertisement.Banner.Show(bannerPlacementId);
	}
	#endregion

	#region Intetestial
	public void ShowIntetestial()
	{
		if (isRemoveAd)
			return;
		rewardType = RewardType.None;

        if(adDistribute == AdDistribute.Unity)
		    Advertisement.Show("Interstitial");

        MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdShow, "Minigame");
	}


	#endregion

	#region Reward Video


	public void ShowVideoAd()
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (rewardBasedVideo.IsLoaded())
			{
				rewardBasedVideo.Show();
			}
		}
		else if (adDistribute == AdDistribute.Unity)
		{
			Advertisement.Show(myPlacementId);
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
		else if (adDistribute == AdDistribute.Unity)
		{
			rewardType = type;
			Advertisement.Show(myPlacementId);
		}
	}

	public void ShowVideoAd(RewardType type, int petId)
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
		else if (adDistribute == AdDistribute.Unity)
		{
			rewardType = type;
			this.petId = petId;
			Advertisement.Show(myPlacementId);
		}


	}

	public void ShowVideoAd(RewardType type, ChestItem item)
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
		else if (adDistribute == AdDistribute.Unity)
		{
			rewardType = type;
			chestItem = item;
			Advertisement.Show(myPlacementId);
		}
	}
	#endregion

	#region Reward
    void OnVideoRewarded()
    {
		// Reward the user for watching the ad to completion.
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdRewarded, rewardType.ToString());
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
	#endregion

	#region Admob
	private void RequestAdmobRewardBasedVideo()
	{
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();

		// Load the rewarded video ad with the request.
		this.rewardBasedVideo.LoadAd(request, adUnitId);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoLoaded event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.VideoAdLoaded, rewardType.ToString());
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
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
		RequestAdmobRewardBasedVideo();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		OnVideoRewarded();
	}

	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
	{
		Debug.Log("HandleRewardBasedVideoLeftApplication event received");
	}
	#endregion

	#region Unity Ad

	// Implement IUnityAdsListener interface methods:
	public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
	{
		// Define conditional logic for each ad completion status:
		if (showResult == ShowResult.Finished)
		{
			OnVideoRewarded();
		}
		else if (showResult == ShowResult.Skipped)
		{
			// Do not reward the user for skipping the ad.
		}
		else if (showResult == ShowResult.Failed)
		{
			Debug.LogWarning("The ad did not finish due to an error.");
		}
	}

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
    
}

