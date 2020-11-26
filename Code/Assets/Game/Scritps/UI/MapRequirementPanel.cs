using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRequirementPanel : MonoBehaviour
{
    public Sprite[] icons;
    public MapType mapType = MapType.Forest;
    int price = 30;
    public Text priceText;
    public GameObject description;
    public Text mapName;
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
        //int price = GameManager.instance.myPlayer.petCount * 10;
        price = 0;
        mapType = type;
        icon.sprite = icons[(int)mapType];
        priceText.text = price.ToString();

        if (mapType == MapType.Forest)
        {
            mapName.text = DataHolder.Dialog(72).GetName(MageManager.instance.GetLanguage());
            levelRequire = 4;
        }
        else if (mapType == MapType.Lake)
        {
            mapName.text = DataHolder.Dialog(97).GetName(MageManager.instance.GetLanguage());
            levelRequire = 6;
        }
        else
            levelRequire = 99;

        if (GameManager.instance.myPlayer.level >= levelRequire)
        {
            description.SetActive(true);
            requirement.gameObject.SetActive(false);
            //buttonAd.SetActive(true);
            buttonGo.SetActive(true);
        }
        else
        {
            description.SetActive(false);
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
            GameManager.instance.AddCoin(-price,Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            UIManager.instance.OnMap(mapType);
            if (UIManager.instance.mapPanel != null)
                UIManager.instance.mapPanel.Close();
            this.Close();
        }
    }

    public void WatchAd()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        RewardVideoAdManager.instance.ShowVideoAd(RewardType.Map);
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
