using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnviromentManager : MonoBehaviour
{
    public static EnviromentManager instance;
    public GameObject dayBG;
    public GameObject nightBG;
    public GameObject rainEffect;

    WeatherType weatherType = WeatherType.None;

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
        if(today.DayOfYear % 7 == 1){
            weatherType = WeatherType.Rain;
        }else{
            weatherType = WeatherType.None;
        }

        LoadWeather();
        CheckDayNight();
        LoadMusic();
    }

    void LoadWeather(){
        if(weatherType == WeatherType.Rain){
            rainEffect.SetActive(true);
        }else
            rainEffect.SetActive(false);
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
            dayBG.SetActive(false);
            nightBG.SetActive(true);
        } else{
            dayBG.SetActive(true);
            nightBG.SetActive(false);
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
