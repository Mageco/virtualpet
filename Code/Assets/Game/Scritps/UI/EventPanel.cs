﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
     public ScrollRect scroll;
     int price = 10;
     public Text priceText;
     public Text levelText;
    public GameObject playButton;
    // Start is called before the first frame update
    void Start()
    {
        price = Mathf.Min(GameManager.instance.myPlayer.minigameLevels[0],10); 
        priceText.text = price.ToString();
        levelText.text = "Level " + (GameManager.instance.myPlayer.minigameLevels[0] + 1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEvent(int id){
        if(id > 0)
        {
             MageManager.instance.OnNotificationPopup ("The game is comming soon.");
        }else{
            if(GameManager.instance.GetHappy() >= price){
                this.Close();
                MageManager.instance.PlaySoundName("BubbleButton",false);
                GameManager.instance.AddHappy(-price);
                UIManager.instance.OnMinigame(1);
            }else
            {
                MageManager.instance.OnNotificationPopup ("You have not enough happy point to play this level, come back and earn more.");
            }
        }
       
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }

     public void OnLeft(){
        if(scroll.horizontalNormalizedPosition > 0)
            scroll.horizontalNormalizedPosition -= 0.333f; 
    }

    public void OnRight(){
        if(scroll.horizontalNormalizedPosition < 1)
            scroll.horizontalNormalizedPosition += 0.333f; 
    }
}
