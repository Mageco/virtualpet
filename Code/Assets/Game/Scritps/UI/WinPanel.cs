﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    public Text exp;
    public Text coin;
    public Text diamon;
    public GameObject replayButton;
    public GameObject adButton;
    int playCount;
    System.DateTime startTime;
    public Text timeText;
    int gameId = 0;

    // Start is called before the first frame update
    void Start()
    {
        MageManager.instance.PlaySoundName("Win", false);

    }

    Animator animator;

    void Awake(){

    }

    // Update is called once per frame
    void Update()
    {
        int t = (int)(600 - (System.DateTime.Now - startTime).TotalSeconds);
        int m = t / 60;
        timeText.text = m.ToString("00") + ":" + (t - m * 60).ToString("00");
    }

    public void Load(int d, int c,int minigameId){
        gameId = minigameId;
        animator = this.GetComponent<Animator>();
        animator.Play("Win",0);
        if(minigameId == 0)
        {
            if ((GameManager.instance.myPlayer.minigameLevels[0] + 1) % 5 == 0 || GameManager.instance.myPlayer.minigameLevels[0] == 0)
            {
                exp.transform.parent.gameObject.SetActive(true);
                GameManager.instance.AddItem(72);
                GameManager.instance.EquipItem(72);
            }
            else
                exp.transform.parent.gameObject.SetActive(false);
        }else if(minigameId == 1)
        {
            exp.transform.parent.gameObject.SetActive(false);
        }

        
        if(c > 0){
            coin.text = c.ToString();
            GameManager.instance.AddCoin(c);
        }
        else
            coin.transform.parent.gameObject.SetActive(false);

        if(d > 0){
            diamon.text = d.ToString();
            GameManager.instance.AddDiamond(d);
        }
        else
            diamon.transform.parent.gameObject.SetActive(false);

        if (ES2.Exists("MinigameWait" + gameId.ToString()))
        {
            startTime = ES2.Load<System.DateTime>("MinigameWait" + gameId.ToString());
        }
        else
            startTime = System.DateTime.Now;

        if (ES2.Exists("MinigamePlayCount" + gameId.ToString()))
        {
            playCount = ES2.Load<int>("MinigamePlayCount" + gameId.ToString());
        }

        if (playCount <= 0)
        {
            replayButton.SetActive(false);
            adButton.SetActive(true);
        }
        else
        {
            replayButton.SetActive(true);
            adButton.SetActive(false);
        }
    }

    public void OnHome(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }

    public void Replay(){
        playCount--;
        startTime = startTime = System.DateTime.Now;
        MageManager.instance.PlaySoundName("BubbleButton", false);
        MageManager.instance.LoadScene(SceneManager.GetActiveScene().name,0.5f);
        this.GetComponent<Popup>().Close();

        ES2.Save(startTime, "MinigameWait" + gameId.ToString());
        ES2.Save<int>(playCount, "MinigamePlayCount" + gameId.ToString());
    }

    public void Close(){
        Minigame.instance.OnHome();
        this.GetComponent<Popup>().Close();
    }


}
