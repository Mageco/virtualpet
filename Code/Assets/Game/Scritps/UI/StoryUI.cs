using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryUI : MonoBehaviour
{
    public PlayableDirector playTimeLine;
    bool isLoad = false;
    public GameObject skipButton;

    // Start is called before the first frame update
    void Start()
    {
        if (ES2.Exists("PlayTime"))
        {
            skipButton.SetActive(true);
        }
        else
        {
            skipButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playTimeLine.time >= playTimeLine.duration - 0.5f && !isLoad){
            isLoad = true;
            MageManager.instance.LoadSceneWithLoading("House");
        }   
    }

    public void Skip()
    {
         MageManager.instance.LoadSceneWithLoading("House");
    }
}
