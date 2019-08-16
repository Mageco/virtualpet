using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpWindow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Close()
	{
		this.GetComponent<Popup> ().Close ();
	}


	public void OnWeb()
	{
		Application.OpenURL ("https://kynangchobe.vn");
		this.Close ();
	}

	public void OnFanpage()
	{
		Application.OpenURL ("https://www.facebook.com/kynangchobe/");
	}

	public void OnPhoneCall()
	{

	}



	public void OnFacebook()
	{
		Application.OpenURL ("https://m.me/kynangchobe");
	}

	public void OnZalo()
	{
		Application.OpenURL ("https://zalo.me/982189637475616649");
	}
}
