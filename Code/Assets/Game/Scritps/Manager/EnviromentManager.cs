using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnviromentManager : MonoBehaviour
{
    public static EnviromentManager instance;
    //public GameObject dayBG;
    //public GameObject nightBG;
    //public GameObject rainEffect;
    public GameObject gardenDayBG;
    public GameObject gardenNightBG;

    WeatherType weatherType = WeatherType.None;

    float time = 0;
    public float maxTimeCheck = 20.3f;
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
        //if(today.DayOfYear % 7 == 1){
        //    weatherType = WeatherType.Rain;
        //}else{
        weatherType = WeatherType.None;
        //}

        //LoadWeather();
        CheckDayNight();
        LoadMusic();
    }

    /*
    void LoadWeather(){
        if(weatherType == WeatherType.Rain){
            rainEffect.SetActive(true);
        }else
            rainEffect.SetActive(false);
    }*/

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
            gardenDayBG.SetActive(false);
            //dayBG.SetActive(false);
            //nightBG.SetActive(true);
            gardenNightBG.SetActive(true);
        } else{
            gardenNightBG.SetActive(false);
            gardenDayBG.SetActive(true);
            //dayBG.SetActive(true);
            //nightBG.SetActive(false);
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
