using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemPanel : MonoBehaviour
{
    int itemId = 0;
    public Text priceText;
    RewardType rewardType = RewardType.Chest;
    ChestItem chestItem;
    int price = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(RewardType type,ChestItem item)
    {
        rewardType = type;
        chestItem = item;
        //priceText.text = "??";
    }

    public void ShowAd()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        if (chestItem != null)
            RewardVideoAdManager.instance.ShowVideoAd(rewardType,chestItem);
        this.Close();
        //Test
        //chestItem.OnActive();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }

    public void Unlock()
    {
        if (GameManager.instance.GetHappy() < price)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
        }
        else
        {
            GameManager.instance.AddHappy(-price, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            if(chestItem != null)
            {
                chestItem.OnActive();
            }
            this.GetComponent<Popup>().Close();
        }
    }
}
