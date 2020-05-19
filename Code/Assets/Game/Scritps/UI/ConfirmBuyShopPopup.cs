﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBuyShopPopup : MonoBehaviour
{
    int realId = 0;
    int itemId = 0;
    bool isCharacter = false;
    public Image icon;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public GameObject happyIcon;
    public Text priceText;
    public Text question;
    public GameObject replacePanel;
    public GameObject quatityPanel;
    public Image replaceIcon;
    ItemState state = ItemState.OnShop;
    public GameObject okButton;
    bool isBuy = false;
    int quantity = 1;
    public InputField quantityEdit;

    public void LoadItem(int id, bool isBuy)
    {
        if (isBuy)
            itemId = id;
        else
        {
            realId = id;
            Debug.Log(realId);
            itemId = GameManager.instance.GetItem(realId).itemId;
        }
            
        this.isBuy = isBuy;
        Item d = DataHolder.GetItem(itemId);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        replacePanel.SetActive(false);
        quatityPanel.SetActive(false);

        if (d.itemType == ItemType.Room || d.itemType == ItemType.Gate || d.itemType == ItemType.Board)
        {
            foreach(PlayerItem item in GameManager.instance.myPlayer.items)
            {
                if(item.itemType == d.itemType && item.state == ItemState.Equiped)
                {
                    replacePanel.SetActive(true);
                    string url1 = DataHolder.GetItem(item.itemId).iconUrl.Replace("Assets/Game/Resources/", "");
                    url1 = url1.Replace(".png", "");
                    replaceIcon.sprite = Resources.Load<Sprite>(url1) as Sprite;
                }
            }
            
        }

        quantityEdit.text = quantity.ToString();
        if (d.consume)
        {
            quatityPanel.SetActive(true);
        }

        if (isBuy)
        {
            priceText.text = d.buyPrice.ToString();
            question.text = DataHolder.Dialog(3).GetDescription(MageManager.instance.GetLanguage()) + " ";
        }
        else
        {
            question.text = DataHolder.Dialog(4).GetDescription(MageManager.instance.GetLanguage()) + " ";
            priceText.text = (d.buyPrice / 2 * quantity).ToString();
        }

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(true);
        }
    }


    public void LoadPet(int id, bool isBuy)
    {
        if (isBuy)
            itemId = id;
        else
        {
            realId = id;
            itemId = GameManager.instance.GetPet(realId).iD;
        }

        quatityPanel.SetActive(false);
        isCharacter = true;
        this.isBuy = isBuy;
        Pet d = DataHolder.GetPet(itemId);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
 
        if (isBuy)
        {
            priceText.text = d.buyPrice.ToString();
            replacePanel.SetActive(false);
            question.text = DataHolder.Dialog(3).GetDescription(MageManager.instance.GetLanguage()) + " ";
        }
        else
        {
            question.text = DataHolder.Dialog(4).GetDescription(MageManager.instance.GetLanguage()) + " ";
            replacePanel.SetActive(false);
            if(d.priceType == PriceType.Coin)
                priceText.text = (d.buyPrice / 2).ToString();
            else if(d.priceType == PriceType.Diamond)
                priceText.text = (100 * d.buyPrice / 2).ToString();
        }

        if (d.priceType == PriceType.Coin || !isBuy)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(true);
        }
    }

    public void Confirm()
    {
        if (isCharacter)
        {
            if (isBuy)
            {
                UIManager.instance.BuyPet(itemId);
            }
            else
                UIManager.instance.SellPet(realId);
        }
        else
        {
            if (isBuy)
            {
                UIManager.instance.BuyItem(itemId);
            }
            else
            {
                if(quantity <= 1)
                    UIManager.instance.SellItem(realId);
                else
                    UIManager.instance.SellItem(realId,quantity);
            }
                
        }
        this.Close();
    }

    public void OnEdit()
    {
        Item d = DataHolder.GetItem(itemId);
        PlayerItem item = GameManager.instance.GetItem(realId);
        
        quantity = int.Parse(quantityEdit.text);
        if (quantity < 1)
        {
            quantity = 1;
            quantityEdit.text = quantity.ToString();
        }     
        else if (quantity > item.number)
        {
            quantity = item.number;
            quantityEdit.text = quantity.ToString();
        }
            
        priceText.text = (d.buyPrice / 2 * quantity).ToString();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
