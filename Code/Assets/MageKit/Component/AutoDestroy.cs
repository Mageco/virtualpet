using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

	public float liveTime = 1;

	// Use this for initialization
	void Start () {
		Invoke ("Destroy", liveTime);
	}

	void Destroy()
	{
		GameObject.Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
