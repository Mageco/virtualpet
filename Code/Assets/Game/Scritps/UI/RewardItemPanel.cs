using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemPanel : MonoBehaviour
{
    public GameObject happyIcon;
    public GameObject coinIcon;
    public GameObject diamondIcon;
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
        coinIcon.SetActive(false);
        happyIcon.SetActive(false);
        diamondIcon.SetActive(false);
        rewardType = type;
        chestItem = item;
        if(item.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
        }else if(item.priceType == PriceType.Happy)
        {
            happyIcon.SetActive(true);
        }
        else if (item.priceType == PriceType.Diamond)
        {
            diamondIcon.SetActive(true);
        }
        priceText.text = chestItem.value.ToString();
    }

    public void ShowAd()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
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
            GameManager.instance.AddHappy(-price, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            if(chestItem != null)
            {
                chestItem.OnActive();
            }
            this.GetComponent<Popup>().Close();
        }
    }
}
