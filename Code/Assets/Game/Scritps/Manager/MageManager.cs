using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mage.Models.Application;

public class MageManager : MonoBehaviour {

	public static MageManager instance;
	public GameObject soundPrefab;

	public AudioSource voice;
	public AudioSource music;

	public GameObject fadeScreen;
	public GameObject waitingScreen;
	public Mprogress loadingBar;
	public GameObject notificationPopupPrefab;
	public GameObject confirmationPopUpPrefab;
	public GameObject verificationPrefab;
	InformationPopup notificationPopup;
	ConfirmationPopup confirmationPopup;

	VerifyWindow verifyPopup;

	[HideInInspector]
	public bool isRate = false;
	string location = "";
	int currentLanguage = 0;

	[HideInInspector]
	public float soundVolume = 1;

	[HideInInspector]
	public int currentID = 1;

	[HideInInspector]
	public List<Sound> audioSounds = new List<Sound>();

	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		else
			GameObject.Destroy (this.gameObject);

		DontDestroyOnLoad (this.gameObject);

		Load ();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 50;

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Load()
	{
		if (ES2.Exists ("SoundVolume"))
			soundVolume = ES2.Load<float> ("SoundVolume");
		if (ES2.Exists ("MusicVolume"))
			music.volume = ES2.Load<float> ("MusicVolume");
		if (ES2.Exists ("VoiceVolume"))
			voice.volume = ES2.Load<float> ("VoiceVolume");

		if (ES2.Exists ("Language"))
			currentLanguage = ES2.Load<int> ("Language");
		else {
			if (Application.systemLanguage == SystemLanguage.Vietnamese) {
				currentLanguage = 0;
			} else
				currentLanguage = 1;
				
		}

		fadeScreen.SetActive (false);
		waitingScreen.SetActive (false);

		if (loadingBar != null)
			loadingBar.gameObject.SetActive (false);

		if (ES2.Exists ("IsRate"))
			isRate = ES2.Load<bool> ("IsRate");

		if (ES2.Exists ("Location")) {
			location = ES2.Load<string> ("Location");
		}

	}

	#region Voice

	public void PlayVoice(AudioClip c,ulong delay)
	{
		voice.Stop ();
		voice.clip = c;
        voice.PlayDelayed (delay);
	}

	public void StopVoice()
	{
		voice.Stop ();
	}

	public void SetVoiceVolume(float v)
	{
		voice.volume = v;
		ES2.Save (voice.volume, "VoiceVolume");
	}

	public float GetVoiceVolume()
	{
		return voice.volume;
	}
	#endregion

	#region Music
	public void PlayMusic(AudioClip c,ulong delay,bool isLoop)
	{
		music.Stop ();
		music.clip = c;
		music.loop = isLoop;
		music.PlayDelayed (delay);
	}

    public void PlayMusicName(string clipName, bool loop)
    {
        AudioClip clip = Resources.Load<AudioClip>("Music/" + clipName);
        music.clip = clip;
        music.loop = loop;
        music.Play();
    }

	public void StopMusic()
	{
		music.Stop ();
	}

	public void SetMusicVolume(float v)
	{
		music.volume = v;
		ES2.Save (music.volume, "MusicVolume");
	}

	public float GetMusicVolume()
	{
		return music.volume;
	}
	#endregion


	#region Sound
	public int PlaySoundName(string clipName,bool loop)
	{
		AudioClip clip = Resources.Load<AudioClip> ("Sound/" + clipName);
		if (clip != null) {
			GameObject go = GameObject.Instantiate (soundPrefab);
			go.transform.parent = this.transform;
			go.name = "Sound";
			Sound s = go.GetComponent<Sound> ();
			currentID++;
			s.Play (currentID, clip, loop, 0, soundVolume);
			audioSounds.Add (s);
			if (!loop) {
				StartCoroutine (StopSoundCouroutine (s, clip.length));
			}
			return currentID;
		}
		return -1;
	}

	public float PlaySoundClip(AudioClip clip)
	{
		GameObject go = GameObject.Instantiate (soundPrefab);
		go.transform.parent = this.transform;
		go.name = "Sound";
		Sound s = go.GetComponent<Sound> ();
		currentID++;
		s.Play (currentID, clip, false,0,soundVolume);
		audioSounds.Add (s);
		StartCoroutine (StopSoundCouroutine (s, clip.length));
		return clip.length;
	}

	public int PlayAnimationSound(AudioClip clip,float volume,Transform anim)
	{
		//if(clip == null)
			return -1;
/* 		GameObject go = GameObject.Instantiate (soundPrefab);
		go.transform.parent = anim;
		go.transform.localPosition = Vector3.zero;
		go.name = "Sound";
		Sound s = go.GetComponent<Sound> ();
		currentID++;
		s.Play (currentID, clip, false,0,volume*soundVolume);
		audioSounds.Add (s);
		//Debug.Log(clip.name);
		StartCoroutine (StopSoundCouroutine (s, clip.length));
		return currentID; */
	}

	public void StopSound(int id)
	{
		foreach (Sound s in audioSounds) {
			if (s.soundID == id) {
				s.Stop ();
				audioSounds.Remove (s);
				if(s != null)
					GameObject.Destroy (s.gameObject);
				return;
			}
		}

		Sound[] sounds = this.transform.GetComponentsInChildren<Sound>();
		foreach (Sound s in sounds) {
			GameObject.Destroy (s.gameObject);
		}
	}

	public void PauseAllSound()
	{
		foreach (Sound s in audioSounds) {
			if (s != null)
				s.Pause ();
		}

		music.Pause ();
		voice.Pause ();
	}

	public void UnPauseAllSound()
	{
		foreach (Sound s in audioSounds) {
			if (s != null)
				s.UnPause ();
		}

		music.UnPause ();
		voice.UnPause ();
	}

	public void StopAllSound()
	{
		foreach (Sound s in audioSounds) {
			if(s != null)
				GameObject.Destroy (s.gameObject);
		}

		audioSounds.Clear ();
		music.Stop ();
		voice.Stop ();
	}

	IEnumerator StopSoundCouroutine(Sound s, float duration)
	{
		yield return new WaitForSeconds (duration);
		s.Stop ();
		audioSounds.Remove (s);
		if(s != null)
			GameObject.Destroy (s.gameObject);
	}

	public void SetSoundVolume(float v)
	{
		soundVolume = v;
		ES2.Save (soundVolume, "SoundVolume");
	}

	public float GetSoundVolume()
	{
		return soundVolume;
	}
	#endregion

	#region Language
	public void SetLanguage(int lan)
	{
		currentLanguage = lan;
		ES2.Save (currentLanguage,"Language");
	}

	public int GetLanguage()
	{
		return currentLanguage;
	}

	public string GetLanguageName()
	{
		return "";
	}

	#endregion

	#region LoadScene

	public void LoadScene(string sceneName,float time)
	{
		StartCoroutine(LoadSceneCouroutine(sceneName,time));
	}

	public void LoadScene(string sceneName,float time1,float time2)
	{
		StartCoroutine(LoadSceneCouroutine(sceneName,time1,time2));
	}

	public void LoadSceneWithLoading(string sceneName)
	{
		StartCoroutine (LoadSceneWithLoadingCouroutine (sceneName));
	}


	IEnumerator LoadSceneWithLoadingCouroutine(string sceneName)
	{
        StopAllSound();
		loadingBar.gameObject.SetActive (true);
		yield return null;

		//Begin to load the Scene you specify
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
		//Don't let the Scene activate until you allow it to
		asyncOperation.allowSceneActivation = false;
		float progress = 0;
		//When the load is still in progress, output the Text and progress bar
		while (!asyncOperation.isDone)
		{
			
			// Check if the load has finished
			if (asyncOperation.progress >= 0.9f) {
				progress += 0.001f;
				asyncOperation.allowSceneActivation = true;
			} else {
				progress = asyncOperation.progress;
			}
			loadingBar.UpdateProgress (progress);
			yield return null;
		}
		loadingBar.gameObject.SetActive (false);
		Resources.UnloadUnusedAssets ();
	}

	IEnumerator LoadSceneCouroutine(string sceneName,float time)
	{
		StopAllSound ();
		yield return StartCoroutine (FadeOut (time));
		SceneManager.LoadScene (sceneName);
		yield return StartCoroutine (FadeIn (time));
		Resources.UnloadUnusedAssets ();
	}

	IEnumerator LoadSceneCouroutine(string sceneName,float time1,float time2)
	{
		StopAllSound ();
		yield return StartCoroutine (FadeOut (time1));
		SceneManager.LoadScene (sceneName);
		yield return StartCoroutine (FadeIn (time2));
		Resources.UnloadUnusedAssets ();
	}

	public void ScreenFadeIn(float time)
	{
		StartCoroutine (FadeIn (time));
	}

	public void ScreenFadeOut(float time)
	{
		StartCoroutine (FadeOut (time));
	}

	public void Fade(float time)
	{
		StartCoroutine(FadeCouroutine(time,time));
	}

	IEnumerator FadeCouroutine(float time1,float time2)
	{
		fadeScreen.SetActive (true);
		fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		Debug.Log ("StartFade");
		float t = 0;
		while (t < time1) {

			float a = fadeScreen.GetComponent<Image> ().color.a;
			a += Time.fixedDeltaTime / time1;
			fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, a);
			t += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		Debug.Log ("EndFade");

		Resources.UnloadUnusedAssets ();
		yield return new WaitForSeconds (0.5f);
		t = 0;
		fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, 1);
		while (t < time2) {
			float a = fadeScreen.GetComponent<Image> ().color.a;

			a -= Time.fixedDeltaTime / time2;
			fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, a);
			t += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate ();
		}


		this.fadeScreen.gameObject.SetActive (false);
	}

	IEnumerator FadeOut(float time1)
	{
		fadeScreen.SetActive (true);
		fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		float t = 0;
		while (t < time1) {

			float a = fadeScreen.GetComponent<Image> ().color.a;
			a += Time.fixedDeltaTime / time1;
			fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, a);
			t += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator FadeIn(float time2)
	{
		this.fadeScreen.SetActive (true);
		float t = 0;
		fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, 1);
		while (t < time2) {
			float a = fadeScreen.GetComponent<Image> ().color.a;

			a -= Time.fixedDeltaTime / time2;
			fadeScreen.GetComponent<Image> ().color = new Color (0, 0, 0, a);
			t += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate ();
		}

		Debug.Log ("Fade In");
		this.fadeScreen.SetActive (false);
	}
	#endregion


	#region Location
	public string GetLocation()
	{
		return location;
	}

	public void SetLocation(string l)
	{
		location = l;
		ES2.Save (location,"Location");
	}

	#endregion


	#region UI
	public InformationPopup OnNotificationPopup(string description)
	{
		if (notificationPopup == null) {
			var popup = Instantiate (notificationPopupPrefab) as GameObject;
			popup.SetActive (true);
			popup.transform.localScale = Vector3.zero;
			popup.transform.SetParent (GameObject.Find ("Canvas").transform, false);
			popup.GetComponent<Popup> ().Open ();
			notificationPopup = popup.GetComponent<InformationPopup>();
			notificationPopup.Load("",description);
		}
		return notificationPopup;
	}

	public ConfirmationPopup OnConfirmationPopup(string title,string description)
	{
		if (confirmationPopup == null) {
			var popup = Instantiate (confirmationPopUpPrefab) as GameObject;
			popup.SetActive (true);
			popup.transform.localScale = Vector3.zero;
			popup.transform.SetParent (GameObject.Find ("Canvas").transform, false);
			popup.GetComponent<Popup> ().Open ();
			confirmationPopup = popup.GetComponent<ConfirmationPopup> ();
			confirmationPopup.Load(title,description);
		}
		return confirmationPopup;
	}

	public void OnWaiting()
	{
		waitingScreen.SetActive (true);
	}

	public void OffWaiting()
	{
		waitingScreen.SetActive (false);
	}

	public VerifyWindow OnVerficationPopUp()
	{
		if (verifyPopup == null) {
			GameObject popup = GameObject.Instantiate (verificationPrefab);
			popup.SetActive (true);
			popup.transform.localScale = Vector3.zero;
			popup.transform.SetParent (GameObject.Find ("Canvas").transform, false);
			popup.GetComponent<Popup> ().Open ();
			verifyPopup = popup.GetComponent<VerifyWindow> ();
			verifyPopup.Load ();
		}
		return verifyPopup;
	}
	#endregion
}

