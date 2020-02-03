using Mage.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetColor : BaseModel
{
    public int iD = 0;
    public int category = 0;
    public string iconUrl = "";
    public LanguageItem[] languageItem = new LanguageItem[0];
    public PriceType priceType = PriceType.Coin;
    public int levelRequire = 0;
    public string petPrefab = "";
    public int buyPrice = 0;
    public ItemState itemState = ItemState.OnShop;
    public bool isAvailable = true;
    public int shopOrder = 0;
}
