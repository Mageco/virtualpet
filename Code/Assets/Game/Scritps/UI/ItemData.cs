using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int itemID;
    public Sprite icon;
    public int price;
    public PriceType priceType = PriceType.Coin;
    public ItemCategory itemCategory = ItemCategory.Diamond;
    public ItemState itemState = ItemState.Buy;

    public void Copy(ItemData d){
        this.itemID = d.itemID;
        this.icon = d.icon;
        this.price = d.price;
        this.priceType = d.priceType;
        this.itemCategory = d.itemCategory;
        this.itemState = d.itemState;
    }
}

public enum PriceType{Coin,Diamond}
public enum ItemCategory{Diamond=0,Food=1,Toys=2,Dogs=3,Set=4,Room=5,Bed=6,Bath=7,Other=8}
public enum ItemState {Buy,Use,Used}
