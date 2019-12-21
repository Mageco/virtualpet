using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
     public ScrollRect scroll;
     int price = 10;
     public Text priceText;
     public Text levelText;
    // Start is called before the first frame update
    void Start()
    {
        price = 10; 
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
             MageManager.instance.OnNotificationPopup ("Trò chơi này chưa ra mắt");
        }else{
            if(GameManager.instance.GetHappy() >= price){
                this.Close();
                MageManager.instance.PlaySoundName("BubbleButton",false);
                GameManager.instance.AddHappy(-price);
                UIManager.instance.OnMinigame(1);
                GameManager.instance.LogAchivement(AchivementType.Play_MiniGame);
            }else
            {
                MageManager.instance.OnNotificationPopup ("Bạn không đủ tim để chơi trò chơi này hãy chăm sóc thú cưng để thu thập thêm tim nhé.");
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
