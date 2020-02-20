using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ForestManager : MonoBehaviour
{
    public static ForestManager instance;
    public GameObject dayBGG;
    public GameObject nightBG;
    float time = 0;
    public float maxTimeCheck = 10;
    DateTime today;
   

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            GameObject.Destroy(this.gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        today = DateTime.Today;
        CheckDayNight();
        LoadMusic();
    }

   

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeCheck){
            time = 0;
            CheckDayNight();
        }else
        {
            time += Time.deltaTime;
        }
    }

    void CheckDayNight(){
        if(DateTime.Now.Hour < 6 || DateTime.Now.Hour > 18)
        {
            dayBGG.SetActive(false);
            nightBG.SetActive(true);
        } else{
            nightBG.SetActive(false);
            dayBGG.SetActive(true);
        }
    }

    public void LoadMusic()
    {
        if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 18)
        {
            MageManager.instance.PlayMusicName("nightMusic", true);
        }
        else
        {
            MageManager.instance.PlayMusicName("dayMusic", true);
        }

    }
}
