using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPopup : MonoBehaviour {

	public Text title;
	public Text description;
	public Button okButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Load(string t,string d)
	{
		if(title != null)
			title.text = t;
		if(description != null)
			description.text = d;
	}

	public void Close(){
		this.GetComponent<Popup>().Close();
	}
}
