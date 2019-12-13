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
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
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
        float s = -time.Second * 6f;
        float m = -time.Minute * 6f - time.Second/60;
        float h = -time.Hour * 30f - time.Minute/2;
        
        
        hour.transform.rotation = Quaternion.Euler(new Vector3(0,0,h));
        minutes.transform.rotation = Quaternion.Euler(new Vector3(0,0,m));
        second.transform.rotation = Quaternion.Euler(new Vector3(0,0,s));
        if(m == 0 && s == 0){
            Active();
        }
    }

    void Active(){
        if(animator != null){
            animator.Play("Active",0);
        }
    }
}
