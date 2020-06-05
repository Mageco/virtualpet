using UnityEngine;

public class Yodo1U3dAds
{
    static bool initialized = false;

    public static void InitializeSdk()
    {
        if (initialized)
        {
            Debug.LogWarning("[Yodo1 Ads] The SDK has been initialized, please do not initialize the SDK repeatedly.");
            return;
        }

        var type = typeof(Yodo1U3dSDK);
        var sdkObj = new GameObject("Yodo1U3dSDK", type).GetComponent<Yodo1U3dSDK>(); // Its Awake() method sets Instance.
        if (Yodo1U3dSDK.Instance != sdkObj)
        {
            Debug.LogError("[Yodo1 Ads] It looks like you have the " + type.Name + " on a GameObject in your scene. Please remove the script from your scene.");
            return;
        }

        Yodo1Ads.Yodo1AdSettings settings = Resources.Load("Yodo1Ads/Yodo1AdSettings", typeof(Yodo1Ads.Yodo1AdSettings)) as Yodo1Ads.Yodo1AdSettings;
        if (settings == null)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. The Yodo1AdSettings is missing.");
            return;
        }

        string appKey = string.Empty;
#if UNITY_ANDROID
        appKey = settings.androidSettings.AppKey;
#elif UNITY_IOS
        appKey = settings.iOSSettings.AppKey;
#endif
        Debug.Log("[Yodo1 Ads] The SDK has been initialized, the app key is " + appKey);
        Yodo1U3dAds.InitWithAppKey(appKey);

        initialized = true;
    }

    /// <summary>
    /// Initialize with app key.
    /// </summary>
    /// <param name="appKey">The app key obtained from MAS Developer Platform.</param>
    static void InitWithAppKey(string appKey)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.InitWithAppKey (appKey);
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.InitWithAppKey(appKey);
#endif
        }
    }

    /// <summary>
    /// Sets the log enable.
    /// </summary>
    /// <param name="enable">If set to <c>true</c> enable.</param>
    public static void SetLogEnable(bool enable)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.SetLogEnable(enable);
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.SetLogEnable(enable);
#endif
        }
    }

    /// <summary>
    /// MAS SDK requires that publishers set a flag indicating whether a user located in the European Economic Area (i.e., EEA/GDPR data subject) has provided opt-in consent for the collection and use of personal data. If the user has consented, please set the flag to true. If the user has not consented, please set the flag to false.
    /// </summary>
    /// <param name="consent"></param>
    public static void SetUserConsent(bool consent)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
            Yodo1U3dAdvertForIOS.SetUserConsent(consent);
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.SetUserConsent(consent);
#endif
        }
    }

    /// <summary>
    /// To ensure COPPA, GDPR, and Google Play policy compliance, you should indicate whether a user is a child. If the user is known to be in an age-restricted category (i.e., under the age of 16) please set the flag to true. If the user is known to not be in an age-restricted category (i.e., age 16 or older) please set the flag to false.
    /// </summary>
    /// <param name="underAgeOfConsent"></param>
    public static void SetTagForUnderAgeOfConsent(bool underAgeOfConsent)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
            Yodo1U3dAdvertForIOS.SetTagForUnderAgeOfConsent(underAgeOfConsent);
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.SetTagForUnderAgeOfConsent(underAgeOfConsent);
#endif
        }
    }

    /// <summary>
    /// Publishers may choose to display a "Do Not Sell My Personal Information" link. Such publishers may choose to set a flag indicating whether a user located in California, USA has opted to not have their personal data sold.
    /// </summary>
    /// <param name="doNotSell"></param>
    public static void SetDoNotSell(bool doNotSell)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
                Yodo1U3dAdvertForIOS.SetDoNotSell(doNotSell);
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
                Yodo1U3dAdvertForAndroid.SetTagForUnderAgeOfConsent(doNotSell);
#endif
        }
    }

    #region  Banner
    /// <summary>
    /// Sets the banner ad align.
    /// </summary>
    /// <param name="align">Align.</param>
    public static void SetBannerAlign(Yodo1U3dConstants.BannerAdAlign align)
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.SetBannerAlign(align);
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.SetBannerAlign(align);
#endif
        }
    }

    /// <summary>
    /// Sets the banner ad offset. Only works on iOS platform.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public static void SetBannerOffset(float x, float y)
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
#if UNITY_ANDROID

#elif UNITY_IPHONE
		Yodo1U3dAdvertForIOS.SetBannerOffset(x,y);
#endif
    }

    /// <summary>
    /// Sets the banner ad scale. Only works on iOS platform.
    /// </summary>
    /// <param name="sx">Sx.</param>
    /// <param name="sy">Sy.</param>
    public static void SetBannerScale(float sx, float sy)
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
#if UNITY_ANDROID

#elif UNITY_IPHONE
		Yodo1U3dAdvertForIOS.SetBannerScale(sx,sy);
#endif
    }

    /// <summary>
    /// Shows the banner ad.
    /// </summary>
    public static void ShowBanner()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.ShowBanner();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.ShowBanner(Yodo1U3dSDK.Instance.SdkObjectName, Yodo1U3dSDK.Instance.SdkMethodName);
#endif
        }
    }

    /// <summary>
    /// Hides the banner ad.
    /// </summary>
    public static void HideBanner()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.HideBanner();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.HideBanner();
#endif
        }
    }

    /// <summary>
    /// Removes the banner ad.
    /// </summary>
    public static void RemoveBanner()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.RemoveBanner();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.RemoveBanner();
#endif
        }
    }

    #endregion

    #region  Interstitial
    /// <summary>
    /// Whether the interstitial ads have been loaded.
    /// </summary>
    /// <returns><c>true</c>, if the interstitial ads have been loaded complete, <c>false</c> otherwise.</returns>
    public static bool InterstitialIsReady()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return false;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			return Yodo1U3dAdvertForIOS.InterstitialIsReady();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            return Yodo1U3dAdvertForAndroid.interstitialIsReady();
#endif
        }
        return false;
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public static void ShowInterstitial()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.ShowInterstitial();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.showInterstitial(Yodo1U3dSDK.Instance.SdkObjectName, Yodo1U3dSDK.Instance.SdkMethodName);
#endif
        }
    }

    #endregion

    #region  Video
    /// <summary>
    /// Whether the reward video ads have been loaded.
    /// </summary>
    /// <returns><c>true</c>, if the reward video ads have been loaded complete, <c>false</c> otherwise.</returns>
    public static bool VideoIsReady()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return false;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			return	Yodo1U3dAdvertForIOS.VideoIsReady();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            return Yodo1U3dAdvertForAndroid.videoIsReady();
#endif
        }
        return false;
    }

    /// <summary>
    /// Shows the reward video ad.
    /// </summary>
    public static void ShowVideo()
    {
        if (!initialized)
        {
            Debug.LogError("[Yodo1 Ads] The SDK has not been initialized yet. Please initialize the SDK first.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IPHONE
			Yodo1U3dAdvertForIOS.ShowVideo ();
#endif
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            Yodo1U3dAdvertForAndroid.showVideo(Yodo1U3dSDK.Instance.SdkObjectName, Yodo1U3dSDK.Instance.SdkMethodName);
#endif
        }
    }

    #endregion
}
