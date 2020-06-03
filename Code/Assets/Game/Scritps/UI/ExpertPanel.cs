using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpertPanel : MonoBehaviour
{
    int price = 3;
    public Text priceText;
    // Start is called before the first frame update
    void Start()
    {
        priceText.text = price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Confirm()
    {
        if (GameManager.instance.GetDiamond() < price)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }
        else
        {
            GameManager.instance.AddDiamond(-price, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            OnService();
            this.Close();
        }
    }

    void OnService()
    {
        foreach(CharController c in GameManager.instance.GetPetObjects())
        {
            c.OnHealth(SickType.Sick, c.data.MaxHealth);
            c.OnHealth(SickType.Injured, c.data.MaxDamage);
            c.data.Food = c.data.MaxFood;
            c.data.Water = c.data.MaxWater;
            c.data.Dirty = 0;
            c.data.Pee = 0;
            c.data.Shit = 0;
            c.data.Sleep = c.data.MaxSleep;
        }
    }

    public void WatchAd()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        RewardVideoAdManager.instance.ShowVideoAd(RewardType.Service);
    }

    public void OnWatchedAd()
    {
        OnService();
        this.Close();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
