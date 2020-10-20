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
using MageSDK.Client.Adaptors;

public class RewardVideoAdManager : MonoBehaviour
{

	public static RewardVideoAdManager instance;
	public RewardType rewardType = RewardType.Minigame;
	ChestItem chestItem;
	int petId = 0;
	[HideInInspector]
	public bool isRemoveAd = false;
	float timeAd = 0;
	float adDuration = 120;
	public AdDistribute adDistribute = AdDistribute.Unity;

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

		// select ads distribution network
		MageAdsHelper.GetInstance().Initialize(ProcessReward);

        //Get quest max value
		if (MageEngine.instance.GetApplicationDataItem("QuestMax") != null)
		{
			GameManager.instance.questMax = int.Parse(MageEngine.instance.GetApplicationDataItem("QuestMax"));
		}

		// Get time lap for Interstitial
	 	if (MageEngine.instance.GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_TIME_LAP_INTERSTITIAL) != null && MageEngine.instance.GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_TIME_LAP_INTERSTITIAL) != "")
		{
            adDuration = float.Parse(MageEngine.instance.GetApplicationDataItem(MageEngineSettings.GAME_ENGINE_TIME_LAP_INTERSTITIAL));
		}

		
		// remove ad for paid user
		if (MageEventHelper.GetInstance().GetEventCounter(MageEventType.ConfirmPaymentItem.ToString()) > 0)
			isRemoveAd = true;
		// start calculate time for Interstitial
		timeAd = GameManager.instance.gameTime;
	}
#if IRON_SOURCE_ENABLED
	void OnApplicationPause(bool isPaused) {      
		if (adDistribute == AdDistribute.IronSource) {
			IronSource.Agent.onApplicationPause(isPaused);
		}           
	}
#endif
	public void ShowVideoAd(RewardType type)
	{
		Debug.Log("Video ads is called: " + MageAdsHelper.GetInstance().GetVideoDistributor());
        rewardType = type;
        MageAdsHelper.GetInstance().ShowVideoAd();
	}

	public void ShowVideoAd(RewardType type,int petId)
	{
		Debug.Log("Video ads is called: " + MageAdsHelper.GetInstance().GetVideoDistributor());
        rewardType = type;
		this.petId = petId;
        MageAdsHelper.GetInstance().ShowVideoAd();

		
	}

	public void ShowVideoAd(RewardType type, ChestItem item)
	{
		Debug.Log("Video ads is called: " + MageAdsHelper.GetInstance().GetVideoDistributor());
        rewardType = type;
		this.chestItem = item;
        MageAdsHelper.GetInstance().ShowVideoAd();
	}

	public void ShowIntetestial(RewardType type = RewardType.Minigame)
	{
		Debug.Log(GameManager.instance.gameTime - timeAd + " " + adDuration);
		if (GameManager.instance.gameTime - timeAd < adDuration)
			return;

        rewardType = type;
        MageAdsHelper.GetInstance().ShowInterstitialAd();

		timeAd = GameManager.instance.myPlayer.playTime;

	}

    public void ShowBanner()
    {
	}

    public void HideBanner()
    {
    }

	#region  Ads Processing Event
	public void ProcessReward(MageEventType adsType)
	{
		if (adsType == MageEventType.InterstitialAdShow) {
			timeAd = GameManager.instance.gameTime;
		}
		// Reward the user for watching the ad to completion.
		MageEngine.instance.OnEvent(adsType, rewardType.ToString(), adDistribute.ToString());
		
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
    
}

