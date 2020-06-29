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

#if UNITY_ADS_ENABLED
namespace MageSDK.Client.Adaptors 
{
	public class UnityAdsAdaptor : IUnityAdsListener 
	{
        //Unity ID
        #if UNITY_IOS
            private static string gameId = "3508454";
        #elif UNITY_ANDROID
            private static string gameId = "3508455";
        #else
            private static string gameId = "3508454";
        #endif

        private static bool testMode = false;

        public static Action<MageEventType> processMageEventType;

		///<summary>Initialize Unity Ads</summary>
		public static void Initialize(Action<MageEventType> processMageEventTypeCallback) {
			//Advertisement.AddListener(this);
			//Advertisement.Initialize(gameId, testMode);
		}

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
        
        /************* Video Delegates *************/ 
        static void RewardedVideoAdOpenedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdOpenedEvent");
        }

        static void RewardedVideoAdRewardedEvent (IronSourcePlacement ssp)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount () + " name = " + ssp.getRewardName ());
            processMageEventType(MageEventType.VideoAdRewarded);
        }
        
        static void RewardedVideoAdClosedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdClosedEvent");
        }

        static void RewardedVideoAdStartedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdStartedEvent");
        }

        static void RewardedVideoAdEndedEvent ()
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdEndedEvent");
        }
        
        static void RewardedVideoAdShowFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
        }

        static void RewardedVideoAdClickedEvent (IronSourcePlacement ssp)
        {
            ApiUtils.Log ("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName ());
        }

        /************* Interstitial Delegates *************/ 
        static void InterstitialAdReadyEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdReadyEvent");
        }
        
        static void InterstitialAdLoadFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode () + ", description : " + error.getDescription ());
        }
        
        static void InterstitialAdShowSucceededEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdShowSucceededEvent");
            processMageEventType(MageEventType.InterstitialAdShow);
        }
        
       static  void InterstitialAdShowFailedEvent (IronSourceError error)
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode () + ", description : " + error.getDescription ());
        }
        
       static void InterstitialAdClickedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdClickedEvent");
        }
        
        static void InterstitialAdOpenedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdOpenedEvent");
        }

        static void InterstitialAdClosedEvent ()
        {
            ApiUtils.Log ("unity-script: I got InterstitialAdClosedEvent");
        }

	}
	
		
}

#endif