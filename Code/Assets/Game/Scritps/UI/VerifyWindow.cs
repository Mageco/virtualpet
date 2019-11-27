using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class VerifyWindow : MonoBehaviour {

	[Serializable]
	public class VerifyEvent : UnityEvent{ };
	[SerializeField]
	public  VerifyEvent m_verify = new VerifyEvent();


	public Text question;
	public InputField answer;
	int n1 = 0;
	int n2 = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Load()
	{
		n1 = UnityEngine.Random.Range(2, 10);
		n2 = UnityEngine.Random.Range(2, 10);
		question.text = n1.ToString() + " X " + n2.ToString() + " = ";
		answer.text = "";
	}

	public void Check()
	{	
		if (answer.text != "" && int.Parse (answer.text) == n1 * n2) {
			m_verify.Invoke ();
			this.Close ();
		} else {
			//GameManager.Instance.OnNotificationPanel ("Warning", "Please input correct result");
		}
	}

	public void Close()
	{
		this.GetComponent<Popup> ().Close ();
	}
}
