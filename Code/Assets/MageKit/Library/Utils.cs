using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using System.IO;
using UnityEngine.UI;
using System.Net;
using UnityEngine.Networking;
using FSG.iOSKeychain;


public class Utils : MonoBehaviour {

	public static Utils instance;
	public delegate void OnLoadedImage(Texture2D tex);
	public event OnLoadedImage onLoadedImage;
	[HideInInspector]
	public string[] languageName = new string[] {"Afrikaans","Arabic","Armenian","Belarusian","Bulgarian","Catalan"," Chinese (Simplified)","Chinese (Traditional)","Croatian"
		,"Czech","Danish","Dutch","English","Esperanto","Estonian","Filipino","Finnish","French","German","Greek","Hebrew","Hindi","Hungarian","Icelandic"
		,"Indonesian","Italian","Japanese","Korean","Latvian","Lithuanian","Norwegian","Persian","Polish","Portuguese","Romanian","Russian","Serbian"
		,"Slovak","Slovenian","Spanish","Swahili","Swedish","Thai","Turkish","Ukrainian","Vietnamese"};
	[HideInInspector]
	public string[] languageCode = new string[] {"af","ar","hy","be","bg","ca","zh-CN","zh-TW","hr","cs","da","nl","en","eo","et","tl","fi","fr"
		,"de","el","iw","hi","hu","is","id","it","ja","ko","lv","lt","no","fa","pl","pt","ro","ru","sr","sk","sl","es","sw","sv","th","tr","uk","vi"};
	
	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			GameObject.Destroy (this.gameObject);
	}

	#region Image
	public Texture2D Resize(float scale,Texture2D sourceTex)
	{
		int w = (int)(sourceTex.width * scale);
		int h = (int)(sourceTex.height * scale);
		Texture2D b = new Texture2D ( w , h , TextureFormat.ARGB32, false);
		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				Color c = sourceTex.GetPixel((int)(x/scale),(int)(y/scale));
				b.SetPixel(x, y, c);
			}
		}
		b.Apply ();
		Texture2D.Destroy (sourceTex);
		return b;
	}

	public Texture2D Resize(int height,Texture2D sourceTex)
	{
		float scale = 1;
		if (sourceTex.width > sourceTex.height) {
			scale = height * 1.0f / sourceTex.height;
		} else {
			scale = height * 1.0f / sourceTex.width;
		}
		int w = (int)(sourceTex.width * scale);
		int h = (int)(sourceTex.height * scale);
		Texture2D b = new Texture2D ( w , h , TextureFormat.ARGB32, false);
		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				Color c = sourceTex.GetPixel((int)(x/scale),(int)(y/scale));
				b.SetPixel(x, y, c);
			}
		}
		b.Apply ();
		Texture2D.Destroy (sourceTex);
		return b;
	}

	public Texture2D CropSquare(Texture2D sourceTex)
	{
		Texture2D b;
		if (sourceTex.width > sourceTex.height) {
			int x1 = (sourceTex.width - sourceTex.height) / 2;
			int x2 = x1 + sourceTex.height;
			b= new Texture2D(sourceTex.height,sourceTex.height,TextureFormat.ARGB32,false);
			for (int x = x1; x < x2; x++) {
				for (int y = 0; y < sourceTex.height; y++) {
					Color c = sourceTex.GetPixel(x,y);
					b.SetPixel(x-x1, y, c);
				}
			}
		} else {
			int y1 = (sourceTex.height - sourceTex.width) / 2;
			int y2 = y1 + sourceTex.width;
			b= new Texture2D(sourceTex.width,sourceTex.width,TextureFormat.ARGB32,false);
			for (int x = 0; x < sourceTex.width; x++) {
				for (int y = y1; y < y2; y++) {
					Color c = sourceTex.GetPixel(x,y);
					b.SetPixel(x, y-y1, c);
				}
			}
		}

		b.Apply ();
		Texture2D.Destroy (sourceTex);
		return b;
	}

	public Texture2D CropSquare(Texture2D sourceTex,Vector2 pos,int size)
	{
		Texture2D b;
		b = new Texture2D (size, size, TextureFormat.ARGB32, false);
		int x1 = (int)pos.x - size / 2;
		int x2 = (int)pos.x + size / 2;
		int y1 = (int)pos.y - size / 2;
		int y2 = (int)pos.y + size / 2;
		for (int x = x1; x < x2; x++) {
			for (int y = y1; y < y2; y++) {
				Color c = sourceTex.GetPixel (x, y);
				b.SetPixel (x - x1, y - y1, c);
			}
		}
		b.Apply ();
		Texture2D.Destroy (sourceTex);
		return b;
	}

	public Texture2D CropCircle(Texture2D sourceTex)
	{
		float r = (sourceTex.width + sourceTex.height)/4;
		float cx = sourceTex.width/2;
		float cy = sourceTex.height/2;
		Texture2D b=new Texture2D(sourceTex.width,sourceTex.height,TextureFormat.ARGB32,false);
		Sprite s;
		for (int x = 0 ; x<sourceTex.width;x++)
		{
			for(int y = 0; y<sourceTex.height;y++)
			{
				Color c = sourceTex.GetPixel(x,y);
				if (r*r>=(x-cx)*(x-cx)+(y-cy)*(y-cy))
					b.SetPixel(x, y, c);
				else
					b.SetPixel(x, y, Color.clear);
			}
		}
		b.Apply ();
		Texture2D.Destroy (sourceTex);
		return b;
	}

	public Texture2D CropCircle(Texture2D sourceTex,Vector2 pos,int size)
	{
		Texture2D b;
		b = new Texture2D (size, size, TextureFormat.ARGB32, false);
		int x1 = (int)pos.x - size / 2;
		int x2 = (int)pos.x + size / 2;
		int y1 = (int)pos.y - size / 2;
		int y2 = (int)pos.y + size / 2;
		float r = size/2.0f;

		for (int x = x1; x <= x2; x++) {
			for (int y = y1; y <= y2; y++) {
				Color c = sourceTex.GetPixel(x,y);
				if (r*r>=(x-pos.x)*(x-pos.x)+(y-pos.y)*(y-pos.y))
					b.SetPixel(x-x1, y-y1, c);
				else
					b.SetPixel(x-x1, y-y1, Color.clear);
			}
		}
		b.Apply ();
		Texture2D.Destroy (sourceTex);
		return b;
	}



	public Sprite CreateSprite(Texture2D sourceTex)
	{
		Sprite s = Sprite.Create (sourceTex, new Rect (new Vector2 (0, 0), new Vector2 (sourceTex.width, sourceTex.height)), new Vector2 (0.5f, 0.5f));
		return s;
	}

	public Sprite CreateCircleSprite(int height,Texture2D tex)
	{
		float scale = 1;
		if (tex.width > tex.height) {
			scale = height * 1.0f / tex.height;
		} else {
			scale = height * 1.0f / tex.width;
		}

		Debug.Log (scale);

		tex = Utils.instance.Resize (scale, tex);
		tex = Utils.instance.CropSquare (tex);
		tex = Utils.instance.CropCircle (tex);
		Sprite s = Utils.instance.CreateSprite (tex);
		return s;	
	}

	public void LoadImage(int size,string avatarUrl)
	{
		StartCoroutine (LoadImageCoroutine (size,avatarUrl));
	}

	public void LoadImage(string avatarUrl,Image image)
	{
		StartCoroutine (LoadImageCoroutine (avatarUrl,image));
	}

	public void LoadImage(int size,string avatarUrl,Image image)
	{
		StartCoroutine (LoadImageCoroutine (size,avatarUrl,image));
	}

	IEnumerator LoadImageCoroutine(string avatarUrl,Image image)
	{
		string[] keys = avatarUrl.Split ('/');
		string path = keys [keys.Length - 1];
		Texture2D tex = new Texture2D(128, 128,TextureFormat.ARGB32,false);
		if (ES3.FileExists (path)) {
			tex = ES3.LoadImage (path);
			yield return null;
		} else {
			WWW url = new WWW (avatarUrl);
			Debug.Log ("start Download");
			yield return url;
			url.LoadImageIntoTexture (tex);
			Debug.Log ("downloaded");
			ES3.SaveImage (tex, path);
			Debug.Log ("saved");
		}
		image.sprite = Utils.instance.CreateSprite (tex);
		image.gameObject.SetActive (true);
		Resources.UnloadUnusedAssets ();
	}

	IEnumerator LoadImageCoroutine(int size, string avatarUrl)
	{
		string[] keys = avatarUrl.Split ('/');
		string path = keys [keys.Length - 1];
		Texture2D tex = new Texture2D(size, size,TextureFormat.ARGB32,false);
		if (ES3.FileExists (path)) {
			tex = ES2.LoadImage (path);
			yield return null;
		} else {
			WWW url = new WWW (avatarUrl);
			yield return url;
			url.LoadImageIntoTexture (tex);
			ES3.SaveImage (tex, path);
		}
		tex = Utils.instance.Resize (size, tex);
		onLoadedImage (tex);
	}

	IEnumerator LoadImageCoroutine(int size,string avatarUrl,Image image)
	{
		string[] keys = avatarUrl.Split ('/');
		string path = keys [keys.Length - 1];
		Texture2D tex = new Texture2D(size, size,TextureFormat.ARGB4444,false,true);
		if (ES3.FileExists (path)) {
			tex = ES2.LoadImage (path);
			yield return null;
		} else {
			WWW url = new WWW (avatarUrl);
			yield return url;
			url.LoadImageIntoTexture (tex);
			ES3.SaveImage (tex, path);
		}
		tex = Utils.instance.Resize (size, tex);
		image.sprite = Utils.instance.CreateSprite (tex);
		image.gameObject.SetActive (true);
		Resources.UnloadUnusedAssets ();
	}
	#endregion

	#region Encription

	public  string Md5Sum(string strToEncrypt)
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

	#endregion

	#region Device Info
	public string GetDeviceID()
	{
		#if UNITY_IOS
		string applicationKey;

		applicationKey =  Keychain.GetValue("deviceId");
		if (applicationKey == "")
		{
		Debug.Log("not Exists");
		Keychain.SetValue("deviceId", SystemInfo.deviceUniqueIdentifier);
		applicationKey = SystemInfo.deviceUniqueIdentifier;
		return applicationKey;
		}
		else
		{
		return applicationKey;
		}
		#else
		return SystemInfo.deviceUniqueIdentifier;
		#endif
	}

	//Get Device Type
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



	#endregion
	#region Audio
	public void LoadAudio(string url,AudioSource audio)
	{
		StartCoroutine (LoadAudioCouroutine (url, audio));
	}

	IEnumerator LoadAudioCouroutine(string url,AudioSource audio)
	{
		string[] keys = url.Split ('/');
		string path = keys [keys.Length - 1];
		if (ES2.Exists (path)) {
			audio.clip = ES2.Load<AudioClip> (path);

		}else
		{
			using (var www = new WWW(url))
			{
				yield return www;
				if(audio !=null)
					audio.clip = www.GetAudioClip();
				ES2.Save<AudioClip>(www.GetAudioClip(),path);
			}
		}
		//Debug.Log ("Complete Load Audio");
	}

	#endregion


	#region Check INternet
	//CheckInternet
	bool CheckConnection()
	{
		try
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.google.com");
			request.Timeout = 5000;
			request.Credentials = CredentialCache.DefaultNetworkCredentials;
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			if (response.StatusCode == HttpStatusCode.OK) return true;
			else return false;
		}
		catch
		{
			return false;
		}
	}


	#endregion
}
