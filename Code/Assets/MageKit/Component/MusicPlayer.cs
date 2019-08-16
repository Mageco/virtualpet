using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

	public AudioClip clip;
	public float delay = 0;
	public bool isLoop = true;

	// Use this for initialization
	void Start () {
		MageManager.instance.PlayMusic (clip,(ulong)delay,isLoop);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
