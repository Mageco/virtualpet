﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TreatmentPopup : MonoBehaviour
{
    int coin = 200;
    public Text price;
    SickType sickType;
    public Sprite[] sickTypes;
    public Image sickTypeIcon;
    public Text description;
    Pet pet;

    public void Load(Pet p,SickType type)
    {
        pet = p;
        sickType = type;
        price.text = coin.ToString();
        if(sickType == SickType.Sick)
        {
            sickTypeIcon.sprite = sickTypes[0];
            description.text = DataHolder.Dialog(17).GetDescription(MageManager.instance.GetLanguage());
        }else if(sickType == SickType.Injured)
        {
            sickTypeIcon.sprite = sickTypes[1];
            description.text = DataHolder.Dialog(18).GetDescription(MageManager.instance.GetLanguage());
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConfirm()
    {
        if (GameManager.instance.GetCoin() >= coin)
        {
            GameManager.instance.OnTreatment(GameManager.instance.GetPetObject(pet.iD), sickType, coin);
            this.Close();
        }
        else
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
    }

    public void OnAd()
    {
        if (sickType == SickType.Sick)
            RewardVideoAdManager.instance.ShowVideoAd(RewardType.Sick, pet.iD);
        else if(sickType == SickType.Injured)
            RewardVideoAdManager.instance.ShowVideoAd(RewardType.Injured,pet.iD);

        Close();
    }

    public void Close() {
        this.GetComponent<Popup>().Close();
    }
}
