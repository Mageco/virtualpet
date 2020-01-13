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

    
    void Start(){
        fishNumber.text = "";
        levelText.text = "Stage " + (gameLevel + 1).ToString();
        MageManager.instance.PlayMusicName("Minigame1",true);
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
            this.maxLive = fishs.Length;
            this.live = this.maxLive;
            UpdateLive();
        }
    }


    void Load(){
        
       
    } 

    public override void UpdateLive(){
        live = 0;
        for(int i=0;i<fishs.Length;i++){
            if(fishs[i].state == FishState.Cached){
                live ++;
            }
        }

        fishNumber.text = live.ToString() + "/" + maxLive.ToString();
       // Debug.Log("Update Live " + live.ToString());
        if(live == maxLive){
            OnWin();
            EndGame();
        }
    }

    protected override void Update(){
        time += Time.deltaTime;
        if (time >= maxTime && state == GameState.Run)
        {

            EndGame();
            if (live == maxLive)
            {
                OnWin();
                GameManager.instance.GetPlayer().minigameLevels[0]++;
                GameManager.instance.LogAchivement(AchivementType.Minigame_Level);
            }
            else
            {
                OnLose();
            }
        }

        if (state == GameState.Run)
        {
            float t = maxTime - time;
            timeText.text  = t.ToString("F0");
        }
    }

    public override void EndGame(){
        GameManager.instance.LogAchivement(AchivementType.Play_MiniGame);
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

