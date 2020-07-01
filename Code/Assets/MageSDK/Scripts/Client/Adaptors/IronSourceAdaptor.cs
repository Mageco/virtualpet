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
		
        private static IronSourceAdaptor _instance;
        public static Action<MageEventType> processMageEventType;

        public IronSourceAdaptor() {
		}

		public static IronSourceAdaptor GetInstance() {
			if (null == _instance) {
				_instance = new IronSourceAdaptor ();
			} 
			return _instance;
		}

		///<summary>Initialize IronSource Ads</summary>
		public void Initialize(Action<MageEventType> processMageEventTypeCallback) {
			ApiUtils.Log ("unity-script: Initialize Iron source called");

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
            // start loading Interstitial for 1st time
            IronSource.Agent.loadInterstitial();

            // asign call back
            processMageEventType = processMageEventTypeCallback;

            // SDK init
            ApiUtils.Log ("unity-script: IronSource.Agent.init");
            IronSource.Agent.init (appKey);
		}

        /************* Video Delegates *************/ 
        public void RewardedVideoAdOpenedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdOpenedEvent");
        }

        public void RewardedVideoAdRewardedEvent (IronSourcePlacement ssp)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount () + " name = " + ssp.getRewardName ());
            processMageEventType(MageEventType.VideoAdRewarded);
        }
        
        public void RewardedVideoAdClosedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdClosedEvent");
        }

        public void RewardedVideoAdStartedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdStartedEvent");
        }

        public void RewardedVideoAdEndedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdEndedEvent");
        }
        
        public void RewardedVideoAdShowFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
        }

        public void RewardedVideoAdClickedEvent (IronSourcePlacement ssp)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName ());
        }

        /************* Interstitial Delegates *************/ 
        public void InterstitialAdReadyEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdReadyEvent");
        }
        
        public void InterstitialAdLoadFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode () + ", description : " + error.getDescription ());
            // prepare for next time
            IronSource.Agent.loadInterstitial();
        }
        
        public void InterstitialAdShowSucceededEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdShowSucceededEvent");
        }
        
        public static  void InterstitialAdShowFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
            // prepare for next time
            IronSource.Agent.loadInterstitial();
        }
        
        public void InterstitialAdClickedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdClickedEvent");
        }
        
        public void InterstitialAdOpenedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdOpenedEvent");
        }

        public void InterstitialAdClosedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdClosedEvent");
            processMageEventType(MageEventType.InterstitialAdShow);
            
            // prepare for next time
            IronSource.Agent.loadInterstitial();
        }

	}
	
		
}

