using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRequirementPanel : MonoBehaviour
{
    public Sprite[] icons;
    public MapType mapType = MapType.Forest;
    int price = 20;
    public Text priceText;
    public Image icon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(MapType type)
    {
        mapType = type;
        icon.sprite = icons[(int)mapType];
        priceText.text = price.ToString();
    }

    public void Confirm()
    {
        if(GameManager.instance.GetHappy() < price)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }
        else
        {
            GameManager.instance.AddCoin(-price);
            UIManager.instance.OnMap(mapType);
            if (UIManager.instance.mapPanel != null)
                UIManager.instance.mapPanel.Close();
            this.Close();
        }
    }

    public void WatchAd()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        RewardVideoAdManager.instance.ShowAd(RewardType.Map);
    }

    public void OnWatchedAd()
    {
        UIManager.instance.OnMap(mapType);
        if (UIManager.instance.mapPanel != null)
            UIManager.instance.mapPanel.Close();
        this.Close();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
