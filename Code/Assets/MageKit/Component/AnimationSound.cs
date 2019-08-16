using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour {

	[HideInInspector]
	Hashtable soundList = new Hashtable();
	int id = 0;

	void Awake()
	{
		StopAllSound ();
	}

	public void PlaySound(AnimationEvent myEvent)
	{
		if(myEvent != null)
		{
			int soundID = MageManager.instance.PlayAnimationSound ((AudioClip)myEvent.objectReferenceParameter,myEvent.floatParameter,this.transform);
			soundList.Add (id.ToString(), soundID);
			id++;
		}
	}


	void StopAllSound()
	{
		soundList.Clear ();
		Sound[] sounds = this.transform.GetComponentsInChildren<Sound> ();
		for (int i = 0; i < sounds.Length; i++) {
			GameObject.Destroy (sounds [i].gameObject);
		}
	}
}
