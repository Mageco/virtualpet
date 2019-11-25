﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    int iD = 0;
    public Image icon;
    public Text skillName;
    public Text skillDescription;
    public Text progress;
    public Image slider;
    public Image rewardIcon;
    public Sprite coinIcon;
    public Sprite diamonIcon;
    public Text rewardValue;
    public Image itemBG;


    void Awake(){

    }

    // Start is called before the first frame update
    public void Load(Skill d)
    {
        iD = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        skillName.text = d.GetName(0);
        skillDescription.text = d.GetDescription(0);
        progress.text = GameManager.instance.GetPetObject(0).data.GetSkillProgress(d.skillType).ToString() + "/" + d.maxProgress.ToString();
        slider.fillAmount = GameManager.instance.GetPetObject(0).data.GetSkillProgress(d.skillType) * 1f/d.maxProgress;
        if(d.coinValue != 0){
            rewardIcon.sprite = coinIcon;
            rewardValue.text = d.coinValue.ToString();
        }else if(d.diamondValue != 0){
            rewardIcon.sprite = diamonIcon;
            rewardValue.text = d.diamondValue.ToString();           
        }

        if(GameManager.instance.GetPetObject(0).data.SkillLearned(d.skillType)){
            itemBG.color = Color.grey;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
