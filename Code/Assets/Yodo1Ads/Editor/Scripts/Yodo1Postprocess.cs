using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Linq;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.Xml;

namespace Yodo1Ads
{
    public class Yodo1PostProcess
    {
        [PostProcessBuild()]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
#if UNITY_IOS
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (CheckConfiguration_iOS(settings))
                {
                    UpdateIOSPlist(pathToBuiltProject, settings);
                    UpdateIOSProject(pathToBuiltProject);
                }
#endif
            }
            if (buildTarget == BuildTarget.Android)
            {
#if UNITY_ANDROID
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (CheckConfiguration_Android(settings))
                {
#if UNITY_2019_1_OR_NEWER
#else
                    ValidateManifest(settings);
#endif
                }
#endif
            }
        }

        #region iOS Content

        public static bool CheckConfiguration_iOS(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                string message = "MAS iOS settings is null, please check the configuration.";
                Debug.LogError("[Yodo1 Ads] " + message);
                Yodo1Utils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(settings.iOSSettings.AppKey))
            {
                string message = "MAS iOS AppKey is null, please check the configuration.";
                Debug.LogError("[Yodo1 Ads] " + message);
                Yodo1Utils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (settings.iOSSettings.GlobalRegion && string.IsNullOrEmpty(settings.iOSSettings.AdmobAppID))
            {
                string message = "MAS iOS AdMob App ID is null, please check the configuration.";
                Debug.LogError("[Yodo1 Ads] " + message);
                Yodo1Utils.ShowAlert("Error", message, "Ok");
                return false;
            }
            return true;
        }

#if UNITY_IOS

        private static void UpdateIOSPlist(string path, Yodo1AdSettings settings)
        {
            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            //Get Root
            PlistElementDict rootDict = plist.root;
            PlistElementDict transportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
            transportSecurity.SetBoolean("NSAllowsArbitraryLoads", true);

            //Set AppLovinSdkKey
            rootDict.SetString("AppLovinSdkKey", Yodo1EditorConstants.DEFAULT_APPLOVIN_SDK_KEY);

            //Set AdMob APP Id
            if(settings.iOSSettings.GlobalRegion)
            {
                rootDict.SetString("GADApplicationIdentifier", settings.iOSSettings.AdmobAppID);
            }

            PlistElementString privacy = (PlistElementString)rootDict["NSLocationAlwaysUsageDescription"];
            if (privacy == null)
            {
                rootDict.SetString("NSLocationAlwaysUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            PlistElementString privacy1 = (PlistElementString)rootDict["NSLocationWhenInUseUsageDescription"];
            if (privacy1 == null)
            {
                rootDict.SetString("NSLocationWhenInUseUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void UpdateIOSProject(string path)
        {
            PBXProject proj = new PBXProject();
            string projPath = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(projPath);

            string mainTargetGuid = string.Empty;
            string unityFrameworkTargetGuid = string.Empty;

#if UNITY_2019_3_OR_NEWER
            mainTargetGuid = proj.GetUnityMainTargetGuid();
            unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
            string frameworksPath = path + "/Frameworks/Plugins/iOS/Yodo1Ads/";
            string[] directories = Directory.GetDirectories(frameworksPath, "*.bundle", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                var dirPath = directories[i];
                var suffixPath = dirPath.Substring(path.Length + 1);
                var fileGuid = proj.AddFile(suffixPath, suffixPath);
                proj.AddFileToBuild(mainTargetGuid, fileGuid);

                fileGuid = proj.FindFileGuidByProjectPath(suffixPath);
                if (fileGuid != null)
                {
                    proj.RemoveFileFromBuild(unityFrameworkTargetGuid, fileGuid);
                }
            }
#else
            mainTargetGuid = proj.TargetGuidByName("Unity-iPhone");
            unityFrameworkTargetGuid = mainTargetGuid;
#endif

#if UNITY_2019_3_OR_NEWER
            var unityFrameworkGuid = proj.FindFileGuidByProjectPath("UnityFramework.framework");
            if (unityFrameworkGuid == null)
            {
                unityFrameworkGuid = proj.AddFile("UnityFramework.framework", "UnityFramework.framework");
                proj.AddFileToBuild(mainTargetGuid, unityFrameworkGuid);
            }
            proj.AddFrameworkToProject(mainTargetGuid, "UnityFramework.framework", false);
            proj.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
#endif

            proj.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
            proj.AddBuildProperty(unityFrameworkTargetGuid, "OTHER_LDFLAGS", "-ObjC -lxml2");


            //Add Frameworks
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreFoundation.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "AdSupport.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "EventKit.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "EventKitUI.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreData.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "Photos.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "OpenGLES.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "UIKit.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "QuartzCore.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "MobileCoreServices.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "ImageIO.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "Foundation.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreGraphics.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CFNetwork.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "AudioToolbox.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "AssetsLibrary.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "StoreKit.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreTelephony.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreText.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "MessageUI.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreLocation.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "AddressBook.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "Accounts.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "Social.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "MediaPlayer.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "AVFoundation.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreMedia.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "CoreMotion.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "WebKit.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "GameController.framework", false);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "WatchConnectivity.framework", true);
            proj.AddFrameworkToProject(unityFrameworkTargetGuid, "GLKit.framework", true);

            string tbdPath = "/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk/usr/lib/";

            if (!Directory.Exists(tbdPath))
            {
                //Add dylibs
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libsqlite3.dylib", true);
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libz.dylib", true);
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libz.1.2.5.dylib", true);
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libresolv.dylib", true);
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libicucore.dylib", true);
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libresolv.9.dylib", true);
                proj.AddFrameworkToProject(unityFrameworkTargetGuid, "libc++.dylib", true);
            }
            else
            {
                //add tbds
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libsqlite3.tbd", "Frameworks/libsqlite3.tbd", PBXSourceTree.Sdk));
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libz.1.2.5.tbd", "Frameworks/libz.1.2.5.tbd", PBXSourceTree.Sdk));
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libresolv.tbd", "Frameworks/libresolv.tbd", PBXSourceTree.Sdk));
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libicucore.tbd", "Frameworks/libicucore.tbd", PBXSourceTree.Sdk));
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libresolv.9.tbd", "Frameworks/libresolv.9.tbd", PBXSourceTree.Sdk));
                proj.AddFileToBuild(unityFrameworkTargetGuid, proj.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));
            }

            // rewrite to file
            File.WriteAllText(projPath, proj.WriteToString());
        }

#endif

        #endregion


        public static void CopyDirectory(string srcPath, string dstPath, string[] excludeExtensions, bool overwrite = true)
        {
            if (!Directory.Exists(dstPath))
                Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath, "*.*", SearchOption.TopDirectoryOnly).Where(path => excludeExtensions == null || !excludeExtensions.Contains(Path.GetExtension(path))))
            {
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)), overwrite);
            }

            foreach (var dir in Directory.GetDirectories(srcPath))
                CopyDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)), excludeExtensions, overwrite);
        }

        #region Android Content

        public static bool CheckConfiguration_Android(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                string message = "MAS Android settings is null, please check the configuration.";
                Debug.LogError("[Yodo1 Ads] " + message);
                Yodo1Utils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(settings.androidSettings.AppKey))
            {
                string message = "MAS Android AppKey is null, please check the configuration.";
                Debug.LogError("[Yodo1 Ads] " + message);
                Yodo1Utils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (settings.androidSettings.GooglePlayStore && string.IsNullOrEmpty(settings.androidSettings.AdmobAppID))
            {
                string message = "MAS Android AdMob App ID is null, please check the configuration.";
                Debug.LogError("[Yodo1 Ads] " + message);
                Yodo1Utils.ShowAlert("Error", message, "Ok");
                return false;
            }
            return true;
        }

        static void GenerateManifest(Yodo1AdSettings settings)
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (!File.Exists(outputFile))
            {
                var inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/AndroidManifest.xml");
                if (!File.Exists(inputFile))
                {
                    inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");
                }
                if (!File.Exists(inputFile))
                {
                    string s = EditorApplication.applicationPath;
                    int index = s.LastIndexOf("/");
                    s = s.Substring(0, index + 1);
                    inputFile = Path.Combine(s, "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");
                }
                if (!File.Exists(inputFile))
                {
                    string s = EditorApplication.applicationPath;
                    int index = s.LastIndexOf("/");
                    s = s.Substring(0, index + 1);
                    inputFile = Path.Combine(s, "PlaybackEngines/AndroidPlayer/Apk/LauncherManifest.xml");
                }
                File.Copy(inputFile, outputFile);
            }
            ValidateManifest(settings);
        }

        public static bool ValidateManifest(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                Debug.LogError("[Yodo1 Ads] Validate manifest failed. Yodo1 ad settings is not exsit.");
                return false;
            }

            var androidPluginPath = Path.Combine(Application.dataPath, "Plugins/Android/");
            var manifestFile = androidPluginPath + "AndroidManifest.xml";
            if (!File.Exists(manifestFile))
            {
                GenerateManifest(settings);
                return true;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            if (doc == null)
            {
                Debug.LogError("[Yodo1 Ads] Couldn't load " + manifestFile);
                return false;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");
            string ns = manNode.GetNamespaceOfPrefix("android");

            XmlNode app = FindChildNode(manNode, "application");

            if (app == null)
            {
                Debug.LogError("[Yodo1 Ads] Error parsing " + manifestFile + ", tag for application not found.");
                return false;
            }

            ////Enable hardware acceleration for video play
            //XmlElement elem = (XmlElement)app;

            //Add AdMob App ID
            if (settings.androidSettings.GooglePlayStore)
            {
                string admobAppIdValue = settings.androidSettings.AdmobAppID;
                if (string.IsNullOrEmpty(admobAppIdValue))
                {
                    Debug.LogError("[Yodo1 Ads] MAS Android AdMob App ID is null, please check the configuration.");
                    return false;
                }
                string admobAppIdName = "com.google.android.gms.ads.APPLICATION_ID";
                XmlNode metaNode = FindChildNodeWithAttribute(app, "meta-data", "android:name", admobAppIdName);
                if (metaNode == null)
                {
                    metaNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "meta-data", null);
                    app.AppendChild(metaNode);
                }

                XmlElement metaElement = (XmlElement)metaNode;
                metaElement.SetAttribute("name", ns, admobAppIdName);
                metaElement.SetAttribute("value", ns, admobAppIdValue);
                metaElement.GetNamespaceOfPrefix("android");
            }

            doc.Save(manifestFile);
            return true;
        }

        public static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        public static XmlNode FindChildNodeWithAttribute(XmlNode parent, string name, string attribute, string value)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name) && curr.Attributes.GetNamedItem(attribute) != null && curr.Attributes[attribute].Value.Equals(value))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        #endregion

    }
}