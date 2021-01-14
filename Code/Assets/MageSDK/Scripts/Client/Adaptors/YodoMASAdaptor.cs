using System;
using Mage.Models.Application;

namespace MageSDK.Client.Adaptors 
{
	public class YodoMASAdaptor 
	{
        private static YodoMASAdaptor _instance;

        public YodoMASAdaptor() {
		}

		public static YodoMASAdaptor GetInstance() {
			if (null == _instance) {
				_instance = new YodoMASAdaptor ();
			} 
			return _instance;
		}
        private static bool testMode = false;

        public Action<MageEventType> processMageEventType;

		///<summary>Initialize YodoMAS Ads</summary>
		public void Initialize(Action<MageEventType> processMageEventTypeCallback) {
            processMageEventType = processMageEventTypeCallback;
            #if YODO1MAS_ENABLED
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
            #endif
		}

        #region Yodo1MAS
        #if YODO1MAS_ENABLED
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
                    this.processMageEventType(MageEventType.InterstitialAdShow);
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
                    this.processMageEventType(MageEventType.VideoAdRewarded);
                break;
            }
        }
        #endif
        #endregion
        
	}
		
}
