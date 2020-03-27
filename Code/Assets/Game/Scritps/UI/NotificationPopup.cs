using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour {

	public Text title;
	public Text description;
	public float delay = 4f;
	float time = 0;

	// Use this for initialization
	void Start () {
        this.gameObject.AddComponent<Canvas>();
        this.GetComponent<Canvas>().overrideSorting = true;
        this.GetComponent<Canvas>().sortingOrder = 4;

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
		//if(UIManager.instance != null && UIManager.instance.notificationText.Contains(this.description.text))
		//	UIManager.instance.notificationText.Remove(this.description.text);
		GameObject.Destroy(this.gameObject);
	}
}
