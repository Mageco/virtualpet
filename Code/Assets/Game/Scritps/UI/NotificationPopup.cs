using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour {

	public Text title;
	public Text description;
	public float delay = 10f;
	float time = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(time > delay){
			Close();
		}else
		{
			time += Time.deltaTime;
		}
	}

	public void Load(string t,string d)
	{
		if(title != null)
			title.text = t;
		if(description != null)
			description.text = d;
	}

	public void Close(){
		GameObject.Destroy(this.gameObject);
	}
}
