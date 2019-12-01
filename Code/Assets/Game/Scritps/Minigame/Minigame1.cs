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
        chickenSpawner.maxNumber = 5 + gameLevel/5;
        chickenSpawner.speed = 5;

        float addSpeed = gameLevel/2f;
        if(addSpeed > 10)
            addSpeed = 10;

        foxSpawner.maxNumber = 2 + gameLevel/5;
        foxSpawner.speed = 12 + addSpeed/3f;

        if(gameLevel > 5){
            int n = gameLevel - 5;
            snakeSpawner.maxNumber = 2 + n/7;
            snakeSpawner.speed = 12 + addSpeed/2f;
        }else
            snakeSpawner.maxNumber = 0;

        if(gameLevel > 10){
            int n = gameLevel - 10;
            eagleSpawner.maxNumber = 2 + n/10;
            eagleSpawner.speed = 15 + addSpeed;
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
        if(!isEnd)
        {
            float t = maxTime - time;
            float m = (int)(t/60);
            float s = (int)(t - m*60);
            timeText.text  = m.ToString("00") + ":" + s.ToString("00");
        }
    }

    public override void EndGame(){
        isEnd = true;
        AnimalController[] animals = GameObject.FindObjectsOfType<FoxController>();
        for(int i=0;i<animals.Length;i++){
            animals[i].InActive();
        }
    }
    
}

