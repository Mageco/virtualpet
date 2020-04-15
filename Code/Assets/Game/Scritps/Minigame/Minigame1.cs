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
    public GameObject hammerEffect;

    public GameObject[] bonuses;

    GuideUI guildUI;
    float timeHit = 0;
    
    
    void Start(){
        timeText.text = "";
        chickenNumber.text = "";
        levelText.text = DataHolder.Dialog(29).GetName(MageManager.instance.GetLanguage()) + " " + (gameLevel + 1).ToString();
        MageManager.instance.PlayMusicName("Minigame1",true);
        if (gameLevel == 0)
            OnGuildPanel();
        else 
            StartGame();

        hammerEffect.SetActive(false);
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


    void Load(){
        int initNumber = 5;
        float initSpeed = 30;
        maxTime = Mathf.Clamp(15 + gameLevel/2,15,45);
        chickenSpawner.maxNumber = 3 + gameLevel / 5 ;
        chickenSpawner.speed = 5;

        float addSpeed = Mathf.Sqrt(gameLevel);
        if(addSpeed > 25)
            addSpeed = 25;

        
        if(gameLevel < 5)
        {
            foxSpawner.maxNumber = initNumber + gameLevel;
        }else if(gameLevel < 20)
        {
            foxSpawner.maxNumber = initNumber + 5 + (gameLevel - 5) / 5;
        }else if(gameLevel < 30)
        {
            foxSpawner.maxNumber = initNumber + 8 + (gameLevel - 20) / 10;
        }
        else
        {
            foxSpawner.maxNumber = Random.Range(10,15);
        }


        foxSpawner.speed = initSpeed + addSpeed;

        if(gameLevel >= 5 && gameLevel % 5 == 0){
            int n = gameLevel - 5;
            snakeSpawner.maxNumber = 1 + n/5;
            snakeSpawner.speed = initSpeed + addSpeed;
            if(gameLevel > 30)
            {
                snakeSpawner.maxNumber = Random.Range(5,10);
            }
        }else
            snakeSpawner.maxNumber = 0;

        if(gameLevel >= 10 && gameLevel % 10 == 0){
            int n = gameLevel - 10;
            eagleSpawner.maxNumber = 1 + n/10;
            eagleSpawner.speed = initSpeed + addSpeed;
            if (gameLevel > 30)
            {
                snakeSpawner.maxNumber = Random.Range(1, 5);
            }
        }
        else
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
            OnEndGame(false);
            EndGame();
        }
    }

    protected override void Update(){

        base.Update();


        if (state == GameState.Run)
        {
            time += Time.deltaTime;
            float t = maxTime - time;
            float m = (int)(t/60);
            float s = (int)(t - m*60);
            timeText.text  = m.ToString("00") + ":" + s.ToString("00");
        }

        if (time >= maxTime && state == GameState.Run)
        {

            EndGame();
            if (live == maxLive)
            {
                OnEndGame(true);
                GameManager.instance.LogAchivement(AchivementType.Minigame_Level,ActionType.None,minigameId);
            }
            else
            {
                OnEndGame(false);
            }
        }

        if (timeHit > 0.2f)
        {
            hammerEffect.SetActive(false);
        }
        else
            timeHit += Time.deltaTime;
    }

    public override void EndGame(){
        Debug.Log("Minigame Id " + minigameId);
        
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

    public void OnFingerDown(Vector3 pos)
    {
        timeHit = 0;
        hammerEffect.SetActive(false);
        hammerEffect.SetActive(true);
        hammerEffect.GetComponentInChildren<Animator>().SetBool("tap", true);
        pos.z = -80;
        hammerEffect.transform.position = pos;

        int number = 0;
        RaycastHit2D[] hit = Physics2D.RaycastAll(pos, Vector3.forward);
        for(int i = 0; i < hit.Length; i++)
        {
            FoxController animal = hit[i].transform.GetComponent<FoxController>();
            if (animal != null)
            {
                if(animal.state == AnimalState.Seek || animal.state == AnimalState.Run)
                {
                    number++;
                }
                animal.OnTap();
            }
        }

        int value = 0;
        if (number == 1)
        {
            value = 1;
            GameObject bonus = Instantiate(bonuses[0]);
            bonus.transform.position = pos + new Vector3(0, 2, -100);
            bonus.transform.localScale = new Vector3(3, 3, 1);
        }
        else if (number == 2)
        {
            value = 3;
            GameObject bonus = Instantiate(bonuses[1]);
            bonus.transform.position = pos + new Vector3(0, 2, -100);
            bonus.transform.localScale = new Vector3(3, 3, 1);
        }
        else if (number == 3)
        {
            value = 7;
            GameObject bonus = Instantiate(bonuses[2]);
            bonus.transform.position = pos + new Vector3(0, 2, -100);
            bonus.transform.localScale = new Vector3(3, 3, 1);
        }
        else if (number > 3)
        {
            value = 12;
            GameObject bonus = Instantiate(bonuses[3]);
            bonus.transform.position = pos + new Vector3(0, 2, -100);
            bonus.transform.localScale = new Vector3(3, 3, 1);
        }

        if(value > 0)
        {
            Minigame.instance.SpawnCoin(pos + new Vector3(0, 2, -1), value);
            bonus += value;
            //GameManager.instance.AddCoin(value);
        }

    }
    
}

