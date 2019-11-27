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
    public Text levelText;

    public AnimalSpawner chickenSpawner;
    public AnimalSpawner foxSpawner;
    public AnimalSpawner snakeSpawner;
    public AnimalSpawner eagleSpawner;
    
    void Start(){
        chickens = GameObject.FindObjectsOfType<ChickenController>();
        this.maxLive = chickens.Length;
        this.live = this.maxLive;
        levelText.text = "Level " + (gameLevel + 1).ToString();
        UpdateLive();
//        Debug.Log(live);
 
    }

    protected override void Load(){
        base.Load();
        maxTime = 55 + gameLevel * 5;
        chickenSpawner.maxNumber = 5 + gameLevel/2;
        chickenSpawner.speed = 10;

        foxSpawner.maxNumber = 2 + gameLevel/2;
        foxSpawner.speed = 10 + gameLevel;

        if(gameLevel > 5){
            int n = gameLevel - 5;
            snakeSpawner.maxNumber = 2 + n/2;
            snakeSpawner.speed = 10 + n;
        }else
            snakeSpawner.maxNumber = 0;

        if(gameLevel > 10){
            int n = gameLevel - 10;
            eagleSpawner.maxNumber = 2 + n/2;
            eagleSpawner.speed = 10 + n;
        }else
            eagleSpawner.maxNumber = 0;

        //foxSpawner.maxNumber = 0;
        //eagleSpawner.maxNumber = 0;
        //snakeSpawner.maxNumber = 0;

        chickenSpawner.Spawn();
        foxSpawner.Spawn();
        snakeSpawner.Spawn();
        eagleSpawner.Spawn();

    } 

    public override void UpdateLive(){
        live = 0;
        for(int i=0;i<chickens.Length;i++){
            if(chickens[i].state != AnimalState.Cached){
                live ++;
            }
        }

        chickenNumber.text = live.ToString() + "/" + maxLive.ToString();
        Debug.Log("Update Live " + live.ToString());
        if(live <= 0){
            OnLose();
            EndGame();
        }
    }

    protected override void Update(){
        base.Update();
        float t = maxTime - time;
        float m = (int)(t/60);
        float s = (int)(t - m*60);
        timeText.text  = m.ToString("00") + ":" + s.ToString("00");
    }

    public override void EndGame(){
        isEnd = true;
        AnimalController[] animals = GameObject.FindObjectsOfType<FoxController>();
        for(int i=0;i<animals.Length;i++){
            animals[i].InActive();
        }
    }
    
}

