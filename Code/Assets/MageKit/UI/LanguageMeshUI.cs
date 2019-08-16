using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageMeshUI : MonoBehaviour
{
	public TextMeshPro[] texts;
	public string[] vietnamese;
	public string[] english;

	// Use this for initialization
	void Start () {
		ChangeLanguage ();
	}

	public void ChangeLanguage()
	{
		if (MageManager.instance.GetLanguage()==0) {
			for (int i = 0; i < texts.Length; i++) {
				if (vietnamese [i] != null)
					texts [i].text = vietnamese [i];
			}
		} else {
			for (int i = 0; i < texts.Length; i++) {
				if (english [i] != null)
					texts [i].text = english [i];
			}
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
