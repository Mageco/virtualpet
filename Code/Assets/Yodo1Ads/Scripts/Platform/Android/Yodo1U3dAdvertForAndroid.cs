using UnityEngine;
using System.Collections;

public class Yodo1U3dAdvertForAndroid
{
#if UNITY_ANDROID
    static AndroidJavaClass jc = null;
    static Yodo1U3dAdvertForAndroid()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            jc = new AndroidJavaClass("com.yodo1.advert.unity.UnityYodo1Advertising");
        }
    }

    /// <summary>
    /// Initialize the with app key.
    /// </summary>
    /// <param name="appKey">App key.</param>
    public static void InitWithAppKey(string appKey)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activityContext = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (jc != null)
                {
                    jc.CallStatic("initSDK", activityContext, appKey);
                }
            }

        }
    }

    /// <summary>
    /// 设置log是否有效
    /// </summary>
    /// <param name="enable"></param>
    /// <returns></returns>
    public static bool SetLogEnable(bool enable)
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            if (jc != null)
            {
                jc.CallStatic("setLogEnable", enable);
            }
        }

        return false;
    }

    public static void SetUserConsent(bool consent)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                jc.CallStatic("setUserConsent", consent);
            }
        }
    }

    public static void SetTagForUnderAgeOfConsent(bool underAgeOfConsent)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                jc.CallStatic("setTagForUnderAgeOfConsent", underAgeOfConsent);
            }
        }
    }

    public static void SetDoNotSell(bool doNotSell)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                jc.CallStatic("setDoNotSell", doNotSell);
            }
        }
    }

    //显示插屏广告
    public static void showInterstitial(string gameObjectName, string callbackName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            if (jc != null)
            {
                jc.CallStatic("showInterstitial", gameObjectName, callbackName);
            }
        }
    }

    //是否已经缓存好插屏广告
    public static bool interstitialIsReady()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                bool value = jc.CallStatic<bool>("interstitialIsReady");
                return value;
            }
        }
        return false;
    }

    //播放视频广告
    public static void showVideo(string gameObjectName, string callbackName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            if (jc != null)
            {
                jc.CallStatic("showVideo", gameObjectName, callbackName);
            }
        }
    }

    //检查视频广告是否缓冲完成
    public static bool videoIsReady()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                bool value = jc.CallStatic<bool>("videoIsReady");
                return value;
            }
        }
        return false;
    }

    //显示Banner
    public static void ShowBanner(string gameObjectName, string callbackName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {

            if (jc != null)
            {
                jc.CallStatic("ShowBanner", gameObjectName, callbackName);
            }
        }
    }

    //设置Banner
    public static void SetBannerAlign(Yodo1U3dConstants.BannerAdAlign align)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                jc.CallStatic("SetBannerAlign", (int)align);
            }
        }
    }

    //关闭Banner
    public static void RemoveBanner()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                jc.CallStatic("RemoveBanner");
            }
        }
    }
    //隐藏Banner
    public static void HideBanner()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (jc != null)
            {
                jc.CallStatic("HideBanner");
            }
        }
    }
#endif
}
