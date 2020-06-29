using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.RemoteConfig;
using Mage.Models;
using Mage.Models.Application;
using Mage.Models.Users;
using MageApi;
using MageSDK.Client;
using UnityEngine;

namespace MageSDK.Client.Adaptors 
{
	public class IronSourceAdaptor 
	{
         #if UNITY_IOS
            private static string appKey = "";
        #elif UNITY_ANDROID
            private static string appKey = "cab81bed";
        #endif
		
        public static Action<MageEventType> processMageEventType;

		///<summary>Initialize IronSource Ads</summary>
		public static void Initialize(Action<MageEventType> processMageEventTypeCallback) {
			ApiUtils.Log ("unity-script: MyAppStart Start called");

            //Dynamic config example
            IronSourceConfig.Instance.setClientSideCallbacks (true);
            IronSource.Agent.setConsent(false);
            IronSource.Agent.setMetaData("do_not_sell","true");

            string id = IronSource.Agent.getAdvertiserId ();
            ApiUtils.Log ("unity-script: IronSource.Agent.getAdvertiserId : " + id);
            
            ApiUtils.Log ("unity-script: IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration ();

            ApiUtils.Log ("unity-script: unity version" + IronSource.unityVersion ());

            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent; 
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent; 

            //Add Interstitial Events
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
            
            processMageEventType = processMageEventTypeCallback;

            // SDK init
            ApiUtils.Log ("unity-script: IronSource.Agent.init");
            IronSource.Agent.init (appKey);

		}

        /************* Video Delegates *************/ 
        public static void RewardedVideoAdOpenedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdOpenedEvent");
        }

        public static void RewardedVideoAdRewardedEvent (IronSourcePlacement ssp)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount () + " name = " + ssp.getRewardName ());
            processMageEventType(MageEventType.VideoAdRewarded);
        }
        
        public static void RewardedVideoAdClosedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdClosedEvent");
        }

        public static void RewardedVideoAdStartedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdStartedEvent");
        }

        public static void RewardedVideoAdEndedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdEndedEvent");
        }
        
        public static void RewardedVideoAdShowFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
        }

        public static void RewardedVideoAdClickedEvent (IronSourcePlacement ssp)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName ());
        }

        /************* Interstitial Delegates *************/ 
        public static void InterstitialAdReadyEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdReadyEvent");
        }
        
        public static void InterstitialAdLoadFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode () + ", description : " + error.getDescription ());
        }
        
        public static void InterstitialAdShowSucceededEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdShowSucceededEvent");
            processMageEventType(MageEventType.InterstitialAdShow);
        }
        
        public static  void InterstitialAdShowFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
        }
        
        public static void InterstitialAdClickedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdClickedEvent");
        }
        
        public static void InterstitialAdOpenedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdOpenedEvent");
        }

        public static void InterstitialAdClosedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdClosedEvent");
        }

	}
	
		
}

