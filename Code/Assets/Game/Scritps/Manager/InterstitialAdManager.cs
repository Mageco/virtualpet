using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using MageSDK.Client;

public class InterstitialAdManager : MonoBehaviour {

	public static InterstitialAdManager instance;

	InterstitialAd interstitial;
	[HideInInspector]
	public double duration = 15;
	DateTime lastTime = DateTime.Now;

	public GameObject fade;

	#if UNITY_ANDROID
	string adUnitId = "ca-app-pub-6818802678275174/1406934608";
	#elif UNITY_IPHONE
	string adUnitId = "ca-app-pub-6818802678275174/4474939312";
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

		fade.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		RequestInterstitial ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	#region Interstitial
	void RequestInterstitial()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd (adUnitId);

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

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true").Build();

		// Load the interstitial with the request.
		interstitial.LoadAd (request);

	}

	public void HandleOnAdLoaded(object sender, EventArgs args)
	{
		Debug.Log("HandleAdLoaded event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdLoaded);
	}

	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		Debug.Log("HandleFailedToReceiveAd event received with message: "+ args.Message);
		fade.SetActive (false);
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdFailtoLoaded);
	}

	public void HandleOnAdOpened(object sender, EventArgs args)
	{
		Debug.Log("HandleAdOpened event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdOpened);
		Time.timeScale = 1;
	}

	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		this.RequestInterstitial ();
		Debug.Log("HandleAdClosed event received");
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdClosed);
		fade.SetActive (false);
		Time.timeScale = 1;
	}

	public void HandleOnAdLeavingApplication(object sender, EventArgs args)
	{
		Debug.Log("HandleAdLeavingApplication event received");
		Time.timeScale = 1;
	}

	public void ShowAd()
	{
	if (interstitial != null && interstitial.IsLoaded ()) {
			#if !UNITY_EDITOR
			fade.SetActive (true);
			#endif
			StartCoroutine(ShowAdCoroutine());

		}
	}

	IEnumerator ShowAdCoroutine()
	{
		yield return new WaitForSeconds (0.1f);
		#if !UNITY_EDITOR
		Time.timeScale = 0;
		#endif
		interstitial.Show ();
		MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.InterstitialAdShow);
	}

	public void CleanUp()
	{
		fade.SetActive (false);
		interstitial.Destroy();
	}

	#endregion
}
