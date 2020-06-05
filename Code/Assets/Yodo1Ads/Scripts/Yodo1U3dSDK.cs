using UnityEngine;
using System.Collections.Generic;
using Yodo1U3dJSON;

public class Yodo1U3dSDK : MonoBehaviour
{
    //ResultCode
    public const int RESULT_CODE_FAILED = 0;
    public const int RESULT_CODE_SUCCESS = 1;
    public const int RESULT_CODE_CANCEL = 2;

    public static Yodo1U3dSDK Instance { get; private set; }

    public string SdkMethodName
    {
        get
        {
            return "Yodo1U3dSDKCallBackResult";
        }
    }

    public string SdkObjectName
    {
        get
        {
            return gameObject.name;
        }
    }

    #region Ad Delegate

    //ShowInterstitialAd of delegate
    public delegate void InterstitialAdDelegate(Yodo1U3dConstants.AdEvent adEvent, string error);

    private static InterstitialAdDelegate _interstitialAdDelegate;

    public static void setInterstitialAdDelegate(InterstitialAdDelegate interstitialAdDelegate)
    {
        _interstitialAdDelegate = interstitialAdDelegate;
    }

    //ShowBanner of delegate
    public delegate void BannerdDelegate(Yodo1U3dConstants.AdEvent adEvent, string error);

    private static BannerdDelegate _bannerDelegate;

    public static void setBannerdDelegate(BannerdDelegate bannerDelegate)
    {
        _bannerDelegate = bannerDelegate;
    }

    //RewardVideo of delegate
    public delegate void RewardVideoDelegate(Yodo1U3dConstants.AdEvent adEvent, string error);

    private static RewardVideoDelegate _rewardVideoDelegate;

    public static void setRewardVideoDelegate(RewardVideoDelegate rewardVideoDelegate)
    {
        _rewardVideoDelegate = rewardVideoDelegate;
    }

    #endregion

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Yodo1U3dSDKCallBackResult(string result)
    {
        Debug.Log("[Yodo1 Ads] The SDK callback result:" + result + "\n");
        Yodo1U3dConstants.Yodo1AdsType flag = Yodo1U3dConstants.Yodo1AdsType.Yodo1AdsTypeNone;
        int resultCode = 0;
        string error = "";
        Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(result);
        if (obj != null)
        {
            if (obj.ContainsKey("resulType"))
            {
                flag = (Yodo1U3dConstants.Yodo1AdsType)int.Parse(obj["resulType"].ToString());  //判定来自哪个回调的标记
            }
            if (obj.ContainsKey("code"))
            {
                resultCode = int.Parse(obj["code"].ToString()); //结果码
            }
            //if (obj.ContainsKey("error"))
            //{
            //    error = obj["error"].ToString(); //msg
            //}
        }

        switch (flag)
        {
            case Yodo1U3dConstants.Yodo1AdsType.Yodo1AdsTypeBanner:  //adview of banner
                {
                    if (_bannerDelegate != null)
                    {
                        _bannerDelegate(getAdEvent(resultCode), error);
                    }
                }
                break;
            case Yodo1U3dConstants.Yodo1AdsType.Yodo1AdsTypeInterstitial: //Interstitial
                {
                    if (_interstitialAdDelegate != null)
                    {
                        _interstitialAdDelegate(getAdEvent(resultCode), error);
                    }
                }
                break;

            case Yodo1U3dConstants.Yodo1AdsType.Yodo1AdsTypeVideo:
                {
                    if (_rewardVideoDelegate != null)
                    {
                        _rewardVideoDelegate(getAdEvent(resultCode), error);
                    }
                }
                break;

        }
    }

    /// <summary>
    /// 获取广告事件
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Yodo1U3dConstants.AdEvent getAdEvent(int value)
    {
        switch (value)
        {
            case 0:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventClose;
                }
            case 1:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventFinish;
                }
            case 2:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventClick;
                }
            case 3:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventLoaded;
                }
            case 4:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventShowSuccess;
                }
            case 5:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventShowFail;
                }
            case 6:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventPurchase;
                }
            case -1:
                {
                    return Yodo1U3dConstants.AdEvent.AdEventLoadFail;
                }
        }
        return Yodo1U3dConstants.AdEvent.AdEventLoadFail;
    }
}
