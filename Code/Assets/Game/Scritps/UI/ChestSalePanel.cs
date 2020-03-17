using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestSalePanel : MonoBehaviour
{
    float price;
    public Text priceText;
    public Text diamondText;
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
    }

    public void OnBuy()
    {
        if(rareType == RareType.Common)
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
}
