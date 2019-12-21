using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour {

	[HideInInspector]
	public int soundID;
	AudioSource audioSource;
	void Awake()
	{
		audioSource = this.GetComponent<AudioSource> ();
	}


	public void Play(int id,AudioClip clip,bool loop,ulong delay,float volume)
	{
		soundID = id;
		audioSource.clip = clip;
		audioSource.loop = loop;
		audioSource.volume = volume;
		audioSource.PlayDelayed (delay);
	}

	public void Pause()
	{
		audioSource.Pause ();
	}

	public void UnPause()
	{
		audioSource.UnPause ();
	}


	public void Stop()
	{
		if(audioSource != null)
			audioSource.Stop ();
	}
}
