using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestSalePanel : MonoBehaviour
{

    public Text priceText;
    RareType rareType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load(RareType rareType)
    {
        this.rareType = rareType;
        
        if(rareType == RareType.Common)
        {
            Item item = DataHolder.GetItem(128);
            priceText.text = (item.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00");
        }
        else if (rareType == RareType.Rare)
        {
            Item item = DataHolder.GetItem(129);
            priceText.text = (item.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00");
        }
        if (rareType == RareType.Epic)
        {
            Item item = DataHolder.GetItem(130);
            priceText.text = (item.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00");
        }

    }

    public void OnBuy()
    {
        if (rareType == RareType.Common)
        {
            PurchaseManager.instance.BuyConsumable(4);
        }else if(rareType == RareType.Rare)
        {
            PurchaseManager.instance.BuyConsumable(5);
        }
        else if (rareType == RareType.Epic)
        {
            PurchaseManager.instance.BuyConsumable(6);
        }
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
