using System;
using SimpleJSON;
using UnityEngine;
using MageApi.Models;
using System.Security.Cryptography;
using MageSDK.Client;

namespace MageApi
{
    public class ApiUtils
    {
        private static ApiUtils _instance;

        public ApiUtils()
        {
        }

        public static ApiUtils GetInstance()
        {
            if (null == _instance)
            {
                _instance = new ApiUtils();
            }
            return _instance;
        }

        /*
		 * get md5 string
		 */
        public string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        /*
		 * encode plain text
		 */
        public string EncodeXor(string text)
        {
            return encrypt(text, MageEngine.instance.ApplicationSecretKey);
        }

        /*
		 * decode encrypted text
		 */
        public string DecodeXor(string cipherText)
        {
            return decrypt(cipherText, MageEngine.instance.ApplicationSecretKey);
        }

        /*
		 * encrypt text based on secret key
		 */
        private string encrypt(string plainText, string key)
        {
            byte[] plainTextbytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            for (int i = 0, j = 0; i < plainTextbytes.Length; i++, j = (j + 1) % keyBytes.Length)
            {
                plainTextbytes[i] = (byte)(plainTextbytes[i] ^ keyBytes[j]);
            }
            return System.Convert.ToBase64String(plainTextbytes);
        }

        /*
		 * decrypt text based on secret key
		 */
        private string decrypt(string plainTextString, string secretKey)
        {
            byte[] cipheredBytes = System.Convert.FromBase64String(plainTextString);
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            for (int i = 0, j = 0; i < cipheredBytes.Length; i++, j = (j + 1) % keyBytes.Length)
            {
                cipheredBytes[i] = (byte)(cipheredBytes[i] ^ keyBytes[j]);
            }
            return System.Text.Encoding.UTF8.GetString(cipheredBytes);

        }

        public string EncryptStringWithKey(string plainTextString, string secretKey)
        {
            return encrypt(plainTextString, secretKey);
        }

        public string DecryptStringWithKey(string cipherText, string secretKey)
        {
            return decrypt(cipherText, secretKey);
        }

        /*
		 * Get device type
		 */
        public string GetDeviceType()
        {
#if UNITY_ANDROID
            return "Android";
#endif

#if UNITY_IOS
			return "IOS";
#endif

#if UNITY_STANDALONE_OSX
			return "OSX";
#endif

#if UNITY_STANDALONE_WIN
			return "Window";
#endif

#if UNITY_WEBGL
			return "WebGL";
#endif
        }

        /*
		 * Generate api key
		 */
        public string GenerateApiKey(string uuid, string apiVersion)
        {
            return Md5Sum(EncodeXor(uuid) + apiVersion).Substring(0, 8);
            //return Md5Sum (uuid + apiVersion).Substring (0, 8);
        }

        /*
		 * Parse from string to json 
		 */
        public JSONNode ParseJson(string data)
        {
            if (data != null)
            {
                try
                {
                    var jNode = JSON.Parse(data);
                    return jNode;
                }
                catch (Exception e)
                {
                    Debug.LogError(this + "ParseJson: " + e);
                }
            }
            else
            {
                Debug.LogError(this + "ParseJson: data null");
            }
            return null;
        }



        /*
		 * Util function to generate guid
		 */
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string RSADecrypt(string input, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

            try
            {
                //rsa.
            }
            catch (Exception e)
            {
            }

            return "";
        }

        public string RSAEncrypt(string input, string privateKey)
        {
            return "";
        }

        public string GetDeviceID()
        {
#if UNITY_IOS
			string applicationKey = "";
			applicationKey = FSG.iOSKeychain.Keychain.GetValue("deviceId");
			if (applicationKey == "")
			{
				//Debug.Log("not Exists");
				FSG.iOSKeychain.Keychain.SetValue("deviceId", SystemInfo.deviceUniqueIdentifier);
				return SystemInfo.deviceUniqueIdentifier;
			}
			else
			{
				//Debug.Log("Exists");
				return applicationKey;
			}
#else
            return SystemInfo.deviceUniqueIdentifier;
#endif
        }

        public static void Log(string s)
        {
#if MAGE_DEBUG
            Debug.Log(s);
#endif
        }

        public static void LogError(string s)
        {
#if UNITY_EDITOR
            //Debug.LogError(s);
#endif
        }
    }
}

