using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour {

	public Text title;
	public Text description;
	[HideInInspector]
	public string variable;
	public TintedButton okButton;
	public TintedButton cancelButton;

	public void Load(string t,string d,string v = "")
	{
		title.text = t;
		description.text = d;
		variable = v;
	}

	public void Close()
	{
		this.GetComponent<Popup> ().Close ();
	}
}
