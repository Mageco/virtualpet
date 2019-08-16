using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatingWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Rate()
	{
		#if UNITY_IOS
		Application.OpenURL ("https://itunes.apple.com/us/app/ky-nang-thoat-hiem-cho-be/id1028955406?ls=1&mt=8");
		#endif
		#if UNITY_ANDROID
		Application.OpenURL ("https://play.google.com/store/apps/details?id=com.bluebirdaward.survivalskill");
		#endif
	}
}
