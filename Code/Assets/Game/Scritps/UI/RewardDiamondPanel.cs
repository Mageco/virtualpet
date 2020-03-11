using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDiamondPanel : MonoBehaviour
{
    RewardType rewardType = RewardType.Chest;
    ChestItem chestItem;
    ForestDiamondItem item;
    bool isWatchAd = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Load(ForestDiamondItem d)
    {
        item = d;
    }


    public void Close()
    {
        if (!isWatchAd && item != null)
            item.DeActive();
        this.GetComponent<Popup>().Close();
    }

    public void WatchAd()
    {
        MageManager.instance.PlaySoundName("BubbleButton", false);
        RewardVideoAdManager.instance.ShowAd(RewardType.ForestDiamond);
    }

    public void WatchedAd()
    {
        if (item != null)
            item.OnActive();
        isWatchAd = true;
        this.Close();
    }
}
