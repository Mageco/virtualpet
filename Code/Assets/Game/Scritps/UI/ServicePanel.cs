using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServicePanel : MonoBehaviour
{
    public Text charName;
    public Text charDescription;
    ServiceType serviceType;
    int price = 1;
    public Text priceText;

    public void Load(ServiceType type)
    {
        this.serviceType = type;
        if(serviceType == ServiceType.Chef)
        {

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
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
            return;
        }
        else
        {
            GameManager.instance.AddDiamond(-price);
            ServiceManager.instance.StartService(serviceType);
            this.Close();
        }
    }

    public void WatchAd()
    {
        MageManager.instance.PlaySoundName("BubbleButton", false);
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
