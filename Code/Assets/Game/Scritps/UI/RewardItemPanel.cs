using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemPanel : MonoBehaviour
{
    RewardType rewardType = RewardType.Chest;
    ChestItem chestItem;
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
}
