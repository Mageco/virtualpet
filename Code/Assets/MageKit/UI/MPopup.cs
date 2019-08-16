using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MPopup : MonoBehaviour {

	public Text[] texts;
	public AnimatedButton[] buttons;
	public TintedButton[] tintedbuttons;

	// Use this for initialization
	void Start () {
	}

	public void Close()
	{
		this.GetComponent<Popup> ().Close ();
	}

}
