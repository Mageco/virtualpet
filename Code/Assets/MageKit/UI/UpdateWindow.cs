using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Load()
	{

	}

	//On Update Button
	public void OnUpdateClick(string urlUpdate)
	{
		#if UNITY_ANDROID
		Application.OpenURL(urlUpdate);
		#endif

		#if UNITY_IOS
		Application.OpenURL(urlUpdate);
		#endif

		#if UNITY_STANDALONE_OSX
		Application.OpenURL(urlUpdate);
		#endif

		#if UNITY_STANDALONE_WIN
		Application.OpenURL(urlUpdate);
		#endif
	}
}
