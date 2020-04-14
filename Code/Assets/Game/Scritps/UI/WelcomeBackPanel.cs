using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeBackPanel : MonoBehaviour
{
    public Text coinText;
    public Text happyText;
    int coin = 1;
    int happy = 1;
    public Button AdButton;
    // Start is called before the first frame update

    public void Load(int c,int h)
    {
        foreach(PlayerItem item in GameManager.instance.myPlayer.items)
        {
            if(item.itemType == ItemType.Fruit)
            {
                coin += c * DataHolder.GetItem(item.itemId).buyPrice / 50;
            }
        }

        foreach (PlayerPet p in GameManager.instance.myPlayer.petDatas)
        {
            happy += h * DataHolder.GetPet(p.iD).RateHappy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = coin.ToString();
        happyText.text = happy.ToString();
        if (RewardVideoAdManager.instance.isUnityVideoLoaded)
            AdButton.interactable = true;
        else
            AdButton.interactable = false;
    }


    public void WatchAd()
    {
        AdButton.gameObject.SetActive(false);
        MageManager.instance.PlaySound("BubbleButton", false);
        RewardVideoAdManager.instance.ShowAd(RewardType.Welcome);
    }

    public void WatchedAd()
    {
        coin = coin * 2;
        happy = happy * 2;
        
    }

    public void Close()
    {
        MageManager.instance.PlaySound("Collect_Achivement", false);
        GameManager.instance.AddCoin(coin);
        GameManager.instance.AddHappy(happy);
        this.GetComponent<Popup>().Close();
    }
}
