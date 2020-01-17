using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour {

	public Text title;
	public Text description;
	public Button okButton;
	public Button cancelButton;

	public void Load(string t,string d)
	{
		title.text = t;
		description.text = d;
	}

	public void Close()
	{
		this.GetComponent<Popup> ().Close ();
	}
}
