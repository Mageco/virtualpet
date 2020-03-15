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
    public GameObject buttonGo;
    public GameObject buttonAd;
    public Text requirement;

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
        int levelRequire = 1;
        mapType = type;
        icon.sprite = icons[(int)mapType];
        priceText.text = price.ToString();

        if (mapType == MapType.Forest)
        {
            levelRequire = 3;
        }
        else if (mapType == MapType.Lake)
        {
            levelRequire = 8;
        }
        else
            levelRequire = 99;

        if (GameManager.instance.myPlayer.level >= levelRequire)
        {
            requirement.gameObject.SetActive(false);
            buttonAd.SetActive(true);
            buttonGo.SetActive(true);
        }
        else
        {
            requirement.gameObject.SetActive(true);
            requirement.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + levelRequire.ToString() + " " + DataHolder.Dialog(52).GetName(MageManager.instance.GetLanguage());
            buttonAd.SetActive(false);
            buttonGo.SetActive(false);
        }
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
