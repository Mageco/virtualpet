using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Minigame1 : Minigame
{ 
    ChickenController[] chickens;
    
    protected override void Load(){
        chickens = GameObject.FindObjectsOfType<ChickenController>();
        this.maxLive = chickens.Length;
        this.live = this.maxLive;
    }
}

