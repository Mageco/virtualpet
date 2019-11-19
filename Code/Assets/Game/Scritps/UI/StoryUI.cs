using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryUI : MonoBehaviour
{
    public PlayableDirector playTimeLine;
    bool isLoad = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playTimeLine.time >= playTimeLine.duration - 0.5f && !isLoad){
            isLoad = true;
            MageManager.instance.LoadSceneWithLoading("House");
        }   
    }
}
