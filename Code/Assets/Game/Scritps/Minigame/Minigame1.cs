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

    public GameObject guideUIPrefab;
    GuideUI guildUI;

    public AudioClip music;
    
    void Start(){
        chickenNumber.text = "";
        levelText.text = "Stage " + (gameLevel + 1).ToString();
        GameManager.instance.UnEquipPets();
        MageManager.instance.PlayMusic(music,0,true);
//        Debug.Log(live);
        if (gameLevel == 0)
            OnGuildPanel();
        else 
            StartGame();
    }

    public override void StartGame(){
        if(state == GameState.Ready){
            state = GameState.Run;
            Load();
            chickens = GameObject.FindObjectsOfType<ChickenController>();
            this.maxLive = chickens.Length;
            this.live = this.maxLive;
            UpdateLive();
        }

    }

    void LoadPet(int id){
        /*
        GameManager.instance.GetPet(id).energy = GameManager.instance.GetActivePet().maxEnergy;
        GameManager.instance.GetPet(id).Food = 0;
        GameManager.instance.GetPet(id).Water = 0;
        GameManager.instance.GetPetObject(id).actionType = ActionType.None;*/
    }

    void Load(){
        int initNumber = 5;
        float initSpeed = 20;
        maxTime = 30 + gameLevel/5*5;
        chickenSpawner.maxNumber = 5;
        chickenSpawner.speed = 5;

        float addSpeed = gameLevel/2f;
        if(addSpeed > 20)
            addSpeed = 20;

        foxSpawner.maxNumber = initNumber + gameLevel/5;
        foxSpawner.speed = initSpeed + addSpeed/3f;

        if(gameLevel > 5){
            int n = gameLevel - 5;
            snakeSpawner.maxNumber = 1 + n/5;
            snakeSpawner.speed = initSpeed + addSpeed/2f;
        }else
            snakeSpawner.maxNumber = 0;

        if(gameLevel > 10){
            int n = gameLevel - 10;
            eagleSpawner.maxNumber = 1 + n/5;
            eagleSpawner.speed = initSpeed + addSpeed;
        }else
            eagleSpawner.maxNumber = 0;

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
       // Debug.Log("Update Live " + live.ToString());
        if(live <= 0){
            OnLose();
            EndGame();
        }
    }

    protected override void Update(){
        base.Update();
        if(state == GameState.Run)
        {
            float t = maxTime - time;
            float m = (int)(t/60);
            float s = (int)(t - m*60);
            timeText.text  = m.ToString("00") + ":" + s.ToString("00");
        }
    }

    public override void EndGame(){
        state = GameState.End;
        AnimalSpawner[] animals = GameObject.FindObjectsOfType<AnimalSpawner>();
        for(int i=0;i<animals.Length;i++){
            animals[i].gameObject.SetActive(false);
        }
        MageManager.instance.StopMusic();
    }

    public void OnGuildPanel()
    {
        if (guildUI == null)
        {
            var popup = Instantiate(guideUIPrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            guildUI = popup.GetComponent<GuideUI>();
        }
     }
    
}

