using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minigame1 : Minigame
{ 
    public ChickenController[] chickens;
    public Text chickenNumber;
    
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
    }
}

