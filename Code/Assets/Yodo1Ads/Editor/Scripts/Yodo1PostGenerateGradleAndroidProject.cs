#if UNITY_ANDROID
using UnityEngine;
using UnityEditor.Android;
using System.IO;
using System.Xml;

namespace Yodo1Ads
{
#if UNITY_2018_1_OR_NEWER
    public class Yodo1PostGenerateGradleAndroidProject : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder
        {
            get { return 100; }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Yodo1ValidateGradle(path);
            Yodo1ValidateManifest(path);
        }

        static void Yodo1ValidateManifest(string path)
        {
            var mainfestPath = Path.Combine(path, "src/main/AndroidManifest.xml");

            if (mainfestPath.Contains("unityLibrary"))
            {
                mainfestPath = mainfestPath.Replace("unityLibrary", "launcher");
            }

            if (File.Exists(mainfestPath))
            {
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (Yodo1PostProcess.CheckConfiguration_Android(settings))
                {
                    ValidateManifest(mainfestPath, settings);
                }
            }
        }

        static void ValidateManifest(string manifestFile, Yodo1AdSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            if (doc == null)
            {
                Debug.LogError("[Yodo1 Ads] Couldn't load " + manifestFile);
                return;
            }

            XmlNode manNode = Yodo1PostProcess.FindChildNode(doc, "manifest");
            string ns = manNode.GetNamespaceOfPrefix("android");

            XmlNode app = Yodo1PostProcess.FindChildNode(manNode, "application");

            if (app == null)
            {
                Debug.LogError("[Yodo1 Ads] Error parsing " + manifestFile + ", tag for application not found.");
                return;
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
                    return;
                }
                string admobAppIdName = "com.google.android.gms.ads.APPLICATION_ID";
                XmlNode metaNode = Yodo1PostProcess.FindChildNodeWithAttribute(app, "meta-data", "android:name", admobAppIdName);
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
        }

        static void Yodo1ValidateGradle(string path)
        {
            var gradlePath = Path.Combine(path, "build.gradle");
            if (gradlePath.Contains("unityLibrary"))
            {
                gradlePath = gradlePath.Replace("unityLibrary", "launcher");
            }
            Debug.LogFormat("[Yodo1 Ads] Updating gradle for Play Instant: {0}", gradlePath);
            WriteBelow(gradlePath, "defaultConfig {", "\t\tmultiDexEnabled true");
        }

        static bool WriteBelow(string filePath, string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.LastIndexOf(below);
            if (beginIndex == -1)
            {
                Debug.LogError("[Yodo1 Ads] Error parsing " + filePath + ", tag for " + below + " not found.");
                return false;
            }

            if (text_all.IndexOf(text) == -1)
            {
                int endIndex = beginIndex + below.Length;

                text_all = text_all.Substring(0, endIndex) + "\n" + text + /*"\n" +*/ text_all.Substring(endIndex);

                StreamWriter streamWriter = new StreamWriter(filePath);
                streamWriter.Write(text_all);
                streamWriter.Close();
                return true;
            }
            return false;
        }
    }
#endif
}
#endif