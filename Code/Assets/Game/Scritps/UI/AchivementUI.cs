﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchivementUI : MonoBehaviour
{
    int iD = 0;
    public Image icon;
    public Text achivementlName;
    public Text achivementDescription;
    public Text progress;
    public Image slider;
    public Image rewardIcon;
    public Sprite coinIcon;
    public Sprite diamonIcon;
    public Text rewardValue;
    public Button collect;
    void Awake(){

    }

    // Start is called before the first frame update
    public void Load(PlayerAchivement a)
    {
        Achivement d = DataHolder.Achivements().GetAchivement(a.achivementId);
        int level = a.level;
        iD = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        achivementlName.text = d.GetName(0);
        achivementDescription.text = d.levelDescription[level];
        progress.text = a.amount.ToString() + "/" + d.maxProgress[level].ToString();
        slider.fillAmount = a.amount * 1f/d.maxProgress[level];
        if(d.coinValue[level] != 0){
            rewardIcon.sprite = coinIcon;
            rewardValue.text = d.coinValue[level].ToString();
        }else if(d.diamondValue[level] != 0){
            rewardIcon.sprite = diamonIcon;
            rewardValue.text = d.diamondValue[level].ToString();           
        }

        collect.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
