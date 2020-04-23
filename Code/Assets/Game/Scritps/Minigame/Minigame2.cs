using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Minigame2 : Minigame
{ 
    public FishController[] fishs;
    public Text fishNumber;
    public Text timeText;
    public Text levelText;
    public GameObject guideUIPrefab;
    GuideUI guildUI;
    public Image timeProgress;

    public FishSpawner fishSpawner;
    public FishSpawner squirtSpawner;
    public FishSpawner jellyFishSpawner;
    public FishSpawner yellowFishSpawner;
    public FishSpawner specialFishSpawner;
    public FishSpawner rusbishFishSpawner;



    protected override void Start(){
        base.Start();
        timeText.text = "";
        fishNumber.text = "";
        levelText.text = DataHolder.Dialog(29).GetName(MageManager.instance.GetLanguage()) + " " + (gameLevel + 1).ToString();
        MageManager.instance.PlayMusicName("Minigame2", true);
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
            fishs = GameObject.FindObjectsOfType<FishController>();
            this.maxLive = GetLive();
            this.live = 0;
            UpdateLive();
        }
    }

    int GetLive()
    {
        
        int count = 0;
        for (int i = 0; i < fishs.Length; i++)
        {
            if (fishs[i].state != FishState.DeActive && (fishs[i].fishType == FishType.Fish || fishs[i].fishType == FishType.YellowFish || fishs[i].fishType == FishType.SpecialFish))
            {
                count++;
            }
        }
        return count;
    }

    void Load(){

        int initNumber = 5;
        maxTime = Mathf.Clamp(60 + gameLevel / 5 * 5, 60, 90);
        fishSpawner.maxNumber = initNumber + gameLevel/2;
        if(gameLevel > 10)
        {
            fishSpawner.maxNumber = 10 + gameLevel / 5;
        }else if(gameLevel > 20)
        {
            fishSpawner.maxNumber = Random.Range(10,20);
        }
        squirtSpawner.maxNumber = 1 + gameLevel / 5;
        if (squirtSpawner.maxNumber > 7)
            squirtSpawner.maxNumber = 7;
        jellyFishSpawner.maxNumber = 1 + gameLevel / 10;
        if (jellyFishSpawner.maxNumber > 3)
            jellyFishSpawner.maxNumber = Random.Range(2,4);
        yellowFishSpawner.maxNumber = 0;
        specialFishSpawner.maxNumber = 0;
        rusbishFishSpawner.maxNumber = 1 + gameLevel / 10;

        if (rusbishFishSpawner.maxNumber > 3)
            rusbishFishSpawner.maxNumber = Random.Range(1, 4);


        if (gameLevel > 0 && gameLevel % 5  == 0)
        {
            yellowFishSpawner.maxNumber = gameLevel/5;
            if(gameLevel > 20)
            {
                yellowFishSpawner.maxNumber = Random.Range(1,3);
            }
        }

        if (gameLevel > 0 && gameLevel % 10 == 0)
        {
            specialFishSpawner.maxNumber = gameLevel/10;
            if (gameLevel > 20)
            {
                yellowFishSpawner.maxNumber = Random.Range(1, 3);
            }
        }


        fishSpawner.Spawn();
        squirtSpawner.Spawn();
        jellyFishSpawner.Spawn();
        yellowFishSpawner.Spawn();
        specialFishSpawner.Spawn();
        rusbishFishSpawner.Spawn();
    } 

    public override void UpdateLive(){
        live = 0;
        for(int i=0;i<fishs.Length;i++){
            if(fishs[i].state == FishState.DeActive && (fishs[i].fishType == FishType.Fish || fishs[i].fishType == FishType.YellowFish || fishs[i].fishType == FishType.SpecialFish))
            {
                live ++;
            }
        }

        fishNumber.text = live.ToString() + "/" + maxLive.ToString();
       // Debug.Log("Update Live " + live.ToString());
        if(live == maxLive){
            OnEndGame(true);
            GameManager.instance.LogAchivement(AchivementType.Minigame_Level, ActionType.None, minigameId);
            EndGame();
        }
    }

    protected override void Update(){

        base.Update();

        if (state == GameState.Run)
        {
            time += Time.deltaTime;
            float t = maxTime - time;
            timeText.text  = t.ToString("F0");
            timeProgress.fillAmount = t / maxTime;
        }

        if (time >= maxTime && state == GameState.Run)
        {

            EndGame();
            if (live == maxLive)
            {
                OnEndGame(true);
                GameManager.instance.LogAchivement(AchivementType.Minigame_Level, ActionType.None, minigameId);
            }
            else
            {
                OnEndGame(false);
            }
        }
    }

    public override void EndGame(){
        
        state = GameState.End;
        FishSpawner[] animals = GameObject.FindObjectsOfType<FishSpawner>();
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

