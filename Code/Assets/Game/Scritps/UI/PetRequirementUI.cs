﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetRequirementUI : MonoBehaviour
{
    public Image icon;
    public Text number;
    public GameObject done;
    int tabId = 0;
    int petId = 0;

    public void Load(int petId,string url,int num,int maxNum,int id)
    {
        this.petId = petId;
        tabId = id;
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        number.text = num.ToString() + "/" + maxNum.ToString();
        if (num >= maxNum)
        {
            done.SetActive(true);
        }
        else
            done.SetActive(false);
        
    }

    public void OnClick()
    {
        UIManager.instance.OnShopPanel(tabId);
        UIManager.instance.shopPanel.ScrollToItem(petId);
    }
}
