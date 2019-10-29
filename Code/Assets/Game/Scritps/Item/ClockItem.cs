using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockItem : MonoBehaviour
{

    public GameObject hour;
    public GameObject minutes;
    public GameObject second;
    float time = 0;


    // Start is called before the first frame update
    void Start()
    {
        LoadTime();
    }

    // Update is called once per frame
    void Update()
    {
        if(time > 1){
            LoadTime();
            time = 0;
        }else
        {
            time  += Time.deltaTime;
        }
    }

    void LoadTime(){        
        DateTime time = DateTime.Now;
        float h = -time.Hour * 30f;
        float m = -time.Minute * 6f;
        float s = -time.Second * 6f;
        hour.transform.rotation = Quaternion.Euler(new Vector3(0,0,h));
        minutes.transform.rotation = Quaternion.Euler(new Vector3(0,0,m));
        second.transform.rotation = Quaternion.Euler(new Vector3(0,0,s));
    }
}
