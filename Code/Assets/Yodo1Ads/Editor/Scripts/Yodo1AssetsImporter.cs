using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Yodo1Ads
{
    public class Yodo1AssetsImporter : AssetPostprocessor
    {
        public override int GetPostprocessOrder()
        {
            // Apply this postprocessor early
            return 0;
        }

        void OnPreprocessAsset()
        {
            if (assetPath.Contains("Version.md"))
            {
                //DeleteFiles_Deprecated();
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                Yodo1AdSettingsSave.UpdateDependencies(settings);
            }
        }

        //void DeleteFiles_Deprecated()
        //{
        //    string[] ignoreList =
        //    {
        //        "ApplovinMaxAdMob",
        //        "ApplovinMaxFacebook",
        //        "ApplovinMaxIronSource",
        //        "ApplovinMaxMintegral",
        //        "ApplovinMaxTapjoy",
        //        "ApplovinMaxToutiao",
        //        "ApplovinMaxUnityAds",
        //        "ApplovinMaxVungle",
        //        "Yodo1Admob",
        //        "Yodo1Applovin",
        //        "Yodo1Facebook",
        //        "Yodo1IronSource",
        //        "Yodo1Mintegral",
        //        "Yodo1SDWebImage",
        //        "Yodo1Tapjoy",
        //        "Yodo1Toutiao",
        //        "Yodo1UnityAds",
        //        "Yodo1UnityTool",
        //        "Yodo1Vungle",
        //    };

        //    string thirdSdkPath = Application.dataPath + "/Plugins/iOS/Yodo1Ads/thirdsdk";
        //    //Debug.Log("thirdSdkPath :" + thirdSdkPath);
        //    if (Directory.Exists(thirdSdkPath))
        //    {
        //        DirectoryInfo direction = new DirectoryInfo(thirdSdkPath);
        //        DirectoryInfo[] directions = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);
        //        for (int i = 0; i < directions.Length; i++)
        //        {
        //            DirectoryInfo directoryInfo = directions[i];
        //            string directionName = directoryInfo.Name;
        //            if (directionName.EndsWith(".meta"))
        //            {
        //                continue;
        //            }
        //            //Debug.Log("Name:" + directionName);
        //            bool ignore = false;
        //            foreach (string name in ignoreList)
        //            {
        //                if (name.Equals(directionName))
        //                {
        //                    ignore = true;
        //                    break;
        //                }
        //            }
        //            if (ignore == false)
        //            {
        //                //Debug.Log("FullName:" + directoryInfo.FullName);
        //                Directory.Delete(directoryInfo.FullName, true);
        //            }
        //        }
        //    }
        //}
    }
}
