using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeVoice();
    }


    public void ChangeVoice()
    {
        AudioSource[] audios = this.transform.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource a in audios)
        {
            if (MageManager.instance.GetSoundVolume() < 0.1f)
                a.enabled = false;
            else
                a.enabled = true;
        }
    }
}
