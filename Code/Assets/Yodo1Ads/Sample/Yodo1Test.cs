using UnityEngine;

public class Yodo1Test : MonoBehaviour
{
    bool isTimes;

    void Start()
    {
        isTimes = true;

        Yodo1U3dAds.SetLogEnable(true);

        Yodo1U3dAds.SetUserConsent(true);
        Yodo1U3dAds.SetTagForUnderAgeOfConsent(false);
        Yodo1U3dAds.SetDoNotSell(false);
        Yodo1U3dAds.InitializeSdk();
        Yodo1U3dSDK.setBannerdDelegate((Yodo1U3dConstants.AdEvent adEvent, string error) =>
        {
            Debug.Log("[Yodo1 Ads] BannerdDelegate:" + adEvent + "\n" + error);
            switch (adEvent)
            {
                case Yodo1U3dConstants.AdEvent.AdEventClick:
                    Debug.Log("[Yodo1 Ads] Banner advertising has been clicked.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventClose:
                    Debug.Log("[Yodo1 Ads] Banner advertising has been closed.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
                    Debug.Log("[Yodo1 Ads] Banner advertising has been shown.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowFail:
                    Debug.Log("[Yodo1 Ads] Banner advertising show failed, the error message:" + error);
                    break;
            }
        });

        Yodo1U3dSDK.setInterstitialAdDelegate((Yodo1U3dConstants.AdEvent adEvent, string error) =>
        {
            Debug.Log("[Yodo1 Ads] InterstitialAdDelegate:" + adEvent + "\n" + error);
            switch (adEvent)
            {
                case Yodo1U3dConstants.AdEvent.AdEventClick:
                    Debug.Log("[Yodo1 Ads] Interstital advertising has been clicked.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventClose:
                    Debug.Log("[Yodo1 Ads] Interstital advertising has been closed.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
                    Debug.Log("[Yodo1 Ads] Interstital advertising has been shown.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowFail:
                    Debug.Log("[Yodo1 Ads] Interstital advertising show failed, the error message:" + error);
                    break;
            }

        });

        Yodo1U3dSDK.setRewardVideoDelegate((Yodo1U3dConstants.AdEvent adEvent, string error) =>
        {
            Debug.Log("[Yodo1 Ads] RewardVideoDelegate:" + adEvent + "\n" + error);
            switch (adEvent)
            {
                case Yodo1U3dConstants.AdEvent.AdEventClick:
                    Debug.Log("[Yodo1 Ads] Reward video advertising has been clicked.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventClose:
                    Debug.Log("[Yodo1 Ads] Reward video advertising has been closed.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
                    Debug.Log("[Yodo1 Ads] Reward video advertising has shown successful.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowFail:
                    Debug.Log("[Yodo1 Ads] Reward video advertising show failed, the error message:" + error);
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventFinish:
                    Debug.Log("[Yodo1 Ads] Reward video advertising has been played finish, give rewards to the player.");
                    break;
            }

        });
    }

    void Update()
    {

    }


    void OnGUI()
    {
        int buttonHeight = Screen.height / 13;
        int buttonWidth = Screen.width / 2;
        int buttonSpace = buttonHeight / 2;
        int startHeight = buttonHeight / 2;

        if (GUI.Button(new Rect(Screen.width / 4, startHeight, buttonWidth, buttonHeight), "show banner ad"))
        {
            if (isTimes)
            {
                isTimes = false;
                Yodo1U3dAds.SetBannerAlign(Yodo1U3dConstants.BannerAdAlign.BannerAdAlignBotton | Yodo1U3dConstants.BannerAdAlign.BannerAdAlignHorizontalCenter);
            }
            //Show banner ad
            Yodo1U3dAds.ShowBanner();
        }

        if (GUI.Button(new Rect(Screen.width / 4, startHeight + buttonSpace + buttonHeight, buttonWidth, buttonHeight), "hide banner ad"))
        {
            //Hide banner ad
            Yodo1U3dAds.HideBanner();

        }
        if (GUI.Button(new Rect(Screen.width / 4, startHeight + buttonHeight * 2 + buttonSpace * 2, buttonWidth, buttonHeight), "show interstitial ad"))
        {
            //Show interstitial ad
            if (Yodo1U3dAds.InterstitialIsReady())
            {
                Yodo1U3dAds.ShowInterstitial();
            }
            else
            {
                Debug.Log("[Yodo1 Ads] Interstitial ad has not been cached.");
            }

        }

        if (GUI.Button(new Rect(Screen.width / 4, startHeight + buttonHeight * 3 + buttonSpace * 3, buttonWidth, buttonHeight), "show reward video ad"))
        {
            //Show reward video ad
            if (Yodo1U3dAds.VideoIsReady())
            {
                Yodo1U3dAds.ShowVideo();
            }
            else
            {
                Debug.Log("[Yodo1 Ads] Reward video ad has not been cached.");
            }
        }

    }
}
