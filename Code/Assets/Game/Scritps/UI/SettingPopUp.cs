using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopUp : MonoBehaviour {

	public Slider sound;
	public Slider music;
	public Dropdown language;
	Animator animator;

	void Awake(){
		animator = this.GetComponent<Animator>();
	}

	// Use this for initialization
	IEnumerator Start () {
		Load ();
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		Time.timeScale = 0;
	}

	void Load()
	{
		if(language != null)
			language.value = MageManager.instance.GetLanguage ();

		if (music != null)
			music.value = MageManager.instance.GetMusicVolume ();

		if(sound != null)
			sound.value = MageManager.instance.GetSoundVolume ();

	}

	public void OnChangeSound(Slider s)
	{
		if (MageManager.instance.GetSoundVolume() != s.value) {
			MageManager.instance.SetSoundVolume (s.value);
			MageManager.instance.SetVoiceVolume(s.value);
		}

		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos) {
			if (go && go.transform.parent == null) {
				go.gameObject.BroadcastMessage("ChangeVoice", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void OnChangeMusic(Slider s)
	{
		if(MageManager.instance.GetMusicVolume () != s.value)
			MageManager.instance.SetMusicVolume (s.value);

		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos) {
			if (go && go.transform.parent == null) {
				go.gameObject.BroadcastMessage("ChangeMusic", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void OnChangeLanguage()
	{
		MageManager.instance.SetLanguage (language.value);

		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos) {
			if (go && go.transform.parent == null) {
				go.gameObject.BroadcastMessage("ChangeLanguage", SendMessageOptions.DontRequireReceiver);
			}
		}
	}


	// Update is called once per frame
	void Update () {
		
	}

	public void Quit()
	{
		Application.Quit ();
	}

	public void Close()
	{
		Time.timeScale = 1;
		MageManager.instance.PlaySoundName ("BubbleButton", false);
		this.GetComponent<Popup> ().Close ();
	}

	public void OnHome(){
		Time.timeScale = 1;
		UIManager.instance.OnHome();
		Close();
	}


    public void OnRate()
    {
    #if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=vn.com.mage.virtualpet");
    #elif UNITY_IOS

    #endif
    }
}
