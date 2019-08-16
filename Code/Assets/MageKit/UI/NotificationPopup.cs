﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour {

	public Text title;
	public Text description;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Load(string t,string d)
	{
		title.text = t;
		description.text = d;
	}
}
