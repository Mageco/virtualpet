using System;
using UnityEngine;
using UnityEditor;

namespace Yodo1Ads
{
    public class Yodo1AssetsMenuEditor : Editor
    {
        [MenuItem("Assets/Yodo1 MAS Settings/Android Settings")]
        public static void AndroidSettings()
        {
            Yodo1AdWindows.Initialize(Yodo1AdWindows.PlatfromTab.Android);
        }

        [MenuItem("Assets/Yodo1 MAS Settings/iOS Settings")]
        public static void IOSSettings()
        {
            Yodo1AdWindows.Initialize(Yodo1AdWindows.PlatfromTab.iOS);
        }

        //[MenuItem("Assets/Yodo1 MAS Settings/Documentation")]
        //public static void Documentation()
        //{
        //    string languageStr = Application.systemLanguage.ToString();
        //    if (languageStr.CompareTo("ChineseSimplified") == 0)
        //    {
        //        Application.OpenURL("https://docs.yodo1.com/#/zh-cn/unity/Unity3dGuide");
        //    }
        //    else
        //    {
        //        Application.OpenURL("https://docs.yodo1.com/#/unity3d/Unity3dGuide");
        //    }
        //}
    }
}
