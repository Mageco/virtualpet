using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemPanel : MonoBehaviour
{
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
    }

    public void ShowAd()
    {
        MageManager.instance.PlaySoundName("BubbleButton", false);
        if (chestItem != null)
            RewardVideoAdManager.instance.ShowAd(rewardType,chestItem);
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
            GameManager.instance.AddHappy(-price);
            if(chestItem != null)
            {
                chestItem.OnActive();
            }
            this.GetComponent<Popup>().Close();
        }
    }
}
