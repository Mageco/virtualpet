using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minigame1 : Minigame
{ 
    public ChickenController[] chickens;
    public Text chickenNumber;
    public Text timeText;
    
    void Start(){
        chickens = GameObject.FindObjectsOfType<ChickenController>();
        this.maxLive = chickens.Length;
        this.live = this.maxLive;
        UpdateLive();
        Debug.Log(live);
    }

    public override void UpdateLive(){
        live = 0;
        for(int i=0;i<chickens.Length;i++){
            if(chickens[i].state != AnimalState.Cached){
                live ++;
            }
        }

        chickenNumber.text = live.ToString();
        Debug.Log("Update Live " + live.ToString());
        if(live <= 0){
            OnLose();
        }
    }

    protected override void Update(){
        base.Update();
        float t = maxTime - time;
        float m = (int)(t/60);
        float s = (int)(t - m*60);
        timeText.text  = m.ToString("00") + ":" + s.ToString("00");
    }
}

