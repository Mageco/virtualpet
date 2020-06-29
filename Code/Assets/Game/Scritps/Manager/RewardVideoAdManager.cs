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
		else if (MageEngine.instance.GetApplicationDataItem("CurrentAdDistrubution") == "IronSource")
		{
			adDistribute = AdDistribute.IronSource;
		}

        //Get quest max value
		if (MageEngine.instance.GetApplicationDataItem("QuestMax") != null)
		{
			GameManager.instance.questMax = int.Parse(MageEngine.instance.GetApplicationDataItem("QuestMax"));
		}

		// Get time lap for Interstitial
		if (MageEngine.instance.GetApplicationDataItem("TimeLapInterstitial") != null)
		{
			adDuration = float.Parse(MageEngine.instance.GetApplicationDataItem("TimeLapInterstitial"));
			Debug.Log("TimeLapInterstitial " + MageEngine.instance.GetApplicationDataItem("TimeLapInterstitial"));
		}

		if (adDistribute == AdDistribute.Admob)
		{ 
			AdmobAdaptor.GetInstance().Initialize(ProcessReward);
		}
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			YodoMASAdaptor.GetInstance().Initialize(ProcessReward);
		}
		else if (adDistribute == AdDistribute.IronSource) {
			IronSourceAdaptor.Initialize(ProcessReward);
		}
		else if (adDistribute == AdDistribute.Unity) {
			UnityAdsAdaptor.GetInstance().Initialize(ProcessReward);
		}

		if (MageEventHelper.GetInstance().GetEventCounter(MageEventType.ConfirmPaymentItem.ToString()) > 0)
			isRemoveAd = true;
	}

	public void ShowVideoAd(RewardType type)
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (AdmobAdaptor.GetInstance().rewardBasedVideo.IsLoaded())
			{
				rewardType = type;
				AdmobAdaptor.GetInstance().rewardBasedVideo.Show();
			}
		}
		#if YODO1MAS_ENABLED
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS VideoAds");
			rewardType = type;
			Yodo1U3dAds.ShowVideo();
		} 
		#endif
		else if(adDistribute == AdDistribute.IronSource) {
			ApiUtils.Log("Show Iron Source VideoAds");
			rewardType = type;
			IronSource.Agent.showRewardedVideo();
		}
		else if (adDistribute == AdDistribute.Unity)
		{
			ApiUtils.Log("Show Unity Ads VideoAds");
			rewardType = type;
			Advertisement.Show(UnityAdsAdaptor.rewardedVideoPlacementId);
		}
	}

	public void ShowVideoAd(RewardType type,int petId)
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (AdmobAdaptor.GetInstance().rewardBasedVideo.IsLoaded())
			{
				rewardType = type;
				AdmobAdaptor.GetInstance().rewardBasedVideo.Show();
				this.petId = petId;
			}
		}
		#if YODO1MAS_ENABLED
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS VideoAds - Pet");
			rewardType = type;
			this.petId = petId;
			Yodo1U3dAds.ShowVideo();
		} 
		#endif
		else if(adDistribute == AdDistribute.IronSource)
        {
			ApiUtils.Log("Show Ironsource VideoAds - Pet");
			rewardType = type;
			this.petId = petId;
			IronSource.Agent.showRewardedVideo();
		} 
		else if (adDistribute == AdDistribute.Unity)
		{
			rewardType = type;
			this.petId = petId;
			Advertisement.Show(UnityAdsAdaptor.rewardedVideoPlacementId);
		}
	}

	public void ShowVideoAd(RewardType type, ChestItem item)
	{
		if (adDistribute == AdDistribute.Admob)
		{
			if (AdmobAdaptor.GetInstance().rewardBasedVideo.IsLoaded())
			{
				rewardType = type;
				AdmobAdaptor.GetInstance().rewardBasedVideo.Show();
				chestItem = item;
			}
		}
		#if YODO1MAS_ENABLED
		else if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS VideoAds - Chest");
			rewardType = type;
			Yodo1U3dAds.ShowVideo();
			chestItem = item;
		} 
		#endif
		else if(adDistribute == AdDistribute.IronSource)
        {
			ApiUtils.Log("Show IronSource VideoAds - Chest");
			rewardType = type;
			IronSource.Agent.showRewardedVideo();
			chestItem = item;
		} 
		else if(adDistribute == AdDistribute.Unity)
        {
			ApiUtils.Log("Show Unity VideoAds - Chest");
			rewardType = type;
			Advertisement.Show(UnityAdsAdaptor.rewardedVideoPlacementId);
			chestItem = item;
		} 
	}

	public void ShowIntetestial(RewardType type = RewardType.Minigame)
	{
		if (isRemoveAd)
			return;

		Debug.Log(GameManager.instance.gameTime - timeAd + " " + adDuration);
		if (GameManager.instance.gameTime - timeAd < adDuration)
			return;

		#if YODO1MAS_ENABLED
		if(adDistribute == AdDistribute.Yodo1MAS)
        {
			ApiUtils.Log("Show Yodo1MAS Interstitial");
			rewardType = type;
			Yodo1U3dAds.ShowInterstitial();
		} 
		#endif
		if(adDistribute == AdDistribute.IronSource)
        {
			ApiUtils.Log("Show IronSource Interstitial");
			rewardType = type;
			IronSource.Agent.showInterstitial();
		} else if (adDistribute == AdDistribute.Unity)
        {
			ApiUtils.Log("Show UnityAds Interstitial");
			rewardType = type;
			Advertisement.Show(UnityAdsAdaptor.interstitialPlacementId);
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

	#region  Ads Processing Event
	public void ProcessReward(MageEventType adsType)
	{
		if (adsType == MageEventType.InterstitialAdShow) {
			timeAd = GameManager.instance.gameTime;
		}
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

