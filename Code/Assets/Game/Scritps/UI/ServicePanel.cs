using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServicePanel : MonoBehaviour
{
    public Text charName;
    public Text charDescription;
    ServiceType serviceType;
    int price = 2;
    public Text priceText;
    public Image icon;



    public void Load(ServiceType type)
    {
        
        this.serviceType = type;
        icon.sprite = Resources.Load<Sprite>("Icons/NPC_Icon/" + serviceType.ToString());
        if (serviceType == ServiceType.Chef)
        {
            charName.text = DataHolder.Dialog(69).GetName(MageManager.instance.GetLanguage());
            charDescription.text = DataHolder.Dialog(87).GetName(MageManager.instance.GetLanguage());
        }
        else if (serviceType == ServiceType.Doctor)
        {
            charName.text = DataHolder.Dialog(71).GetName(MageManager.instance.GetLanguage());
            charDescription.text = DataHolder.Dialog(84).GetName(MageManager.instance.GetLanguage());
        }
        else if (serviceType == ServiceType.HouseKeeper)
        {
            charName.text = DataHolder.Dialog(68).GetName(MageManager.instance.GetLanguage());
            charDescription.text = DataHolder.Dialog(85).GetName(MageManager.instance.GetLanguage());
        }
        else if (serviceType == ServiceType.PetSitter)
        {
            charName.text = DataHolder.Dialog(70).GetName(MageManager.instance.GetLanguage());
            charDescription.text = DataHolder.Dialog(86).GetName(MageManager.instance.GetLanguage());
        }
        else if (serviceType == ServiceType.Instructor)
        {
            charName.text = DataHolder.Dialog(91).GetName(MageManager.instance.GetLanguage());
            charDescription.text = DataHolder.Dialog(96).GetName(MageManager.instance.GetLanguage());
        }

        priceText.text = price.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
            ServiceManager.instance.StartService(serviceType);
            this.Close();
        }
    }

    public void WatchAd()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        RewardVideoAdManager.instance.ShowAd(RewardType.Service);
    }

    public void OnWatchedAd()
    {
        ServiceManager.instance.StartService(serviceType);
        this.Close();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }

}
