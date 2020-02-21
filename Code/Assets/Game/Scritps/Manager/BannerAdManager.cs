using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using MageSDK.Client;

public class BannerAdManager : MonoBehaviour {

	public static BannerAdManager instance;

	private BannerView bannerView;
	#if UNITY_ANDROID
	string appId = "ca-app-pub-6818802678275174~9883498495";
	#elif UNITY_IOS
	string appId = "ca-app-pub-6818802678275174~9883498495";
	#else
	string appId = "unexpected_platform";
	#endif

	#if UNITY_ANDROID
	string adUnitId = "ca-app-pub-6818802678275174/8219754968";
	#elif UNITY_IOS
	string adUnitId = "ca-app-pub-6818802678275174/9616597112";
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

	public void Start()
	{
		// Initialize the Google Mobile Ads SDK.
		MobileAds.Initialize(appId);
		RequestBanner ();
	}

	public void RequestBanner()
	{
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

		// Called when an ad request has successfully loaded.
		bannerView.OnAdLoaded += HandleOnAdLoaded;
		// Called when an ad request failed to load.
		bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
		// Called when an ad is clicked.
		bannerView.OnAdOpening += HandleOnAdOpened;
		// Called when the user returned from the app after an ad click.
		bannerView.OnAdClosed += HandleOnAdClosed;
		// Called when the ad click caused the user to leave the application.
		bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;


		// Create an empty ad request.
		//AdRequest request = new AdRequest.Builder().Build();

		AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();

		// Load the banner with the request.
		bannerView.LoadAd(request);
	}

	public void ShowAd()
	{
		Debug.Log ("Show Banner Ad");

	if (bannerView != null && !PurchaseManager.instance.IsRemoveAd) {
			bannerView.Show ();
		    MageEngine.instance.OnEvent (Mage.Models.Application.MageEventType.BannerAdOpened);
		}
	}

	public void HideAd()
	{
		Debug.Log ("Hide Banner Ad");
		if(bannerView != null)
			bannerView.Hide ();
	}

	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		Debug.Log("HandleAdLoaded event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.BannerAdLoaded);
		HideAd ();
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("HandleFailedToReceiveAd event received with message: "
			+ args.Message);
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		Debug.Log("HandleAdOpened event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.BannerAdOpened);
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		Debug.Log("HandleAdClosed event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.BannerAdClosed);
	}

	public void HandleOnAdLeavingApplication(object sender, EventArgs args)
	{
		Debug.Log("HandleAdLeavingApplication event received");
	}
}
