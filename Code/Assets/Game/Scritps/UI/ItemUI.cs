﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    int itemId = 0;
    public Image icon;
    public Text price;
    public GameObject buyButton;
    public GameObject useButton;
    public GameObject usedButton;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;

    // Start is called before the first frame update
    public void Load(Item d)
    {
        
        itemId = d.iD;
        Debug.Log(d.iconUrl);
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        Debug.Log(url);
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();
        if(d.itemState == ItemState.Buy){
            buyButton.SetActive(true);
            useButton.SetActive(false);
            usedButton.SetActive(false);
        }else if(d.itemState == ItemState.Use){
            buyButton.SetActive(false);
            useButton.SetActive(true);
            usedButton.SetActive(false);
        }else if(d.itemState == ItemState.Used){
            buyButton.SetActive(false);
            useButton.SetActive(false);
            usedButton.SetActive(true);
        }

        if(d.priceType == PriceType.Coin){
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
        }else if(d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);      
            moneyIcon.SetActive(false);      
        }else if(d.priceType == PriceType.Money){
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);      
            moneyIcon.SetActive(true);            
        }

    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuy(){

    }

    public void OnUse(){

    }
}
