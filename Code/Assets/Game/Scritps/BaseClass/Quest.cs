using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public int iD =0;
    public LanguageItem[] languageItem = new LanguageItem[0];
    //Requirement
    public int charLevel;
    public QuestRequirement[] requirements;
    //Rewards
    public int itemId = 0;
    public int coinValue = 0;
    public int diamondValue = 0;
    //Dialog
    public int dialogId = 0;
}

public class QuestRequirement{

     public QuestRequirementType requireType;
     public ActionType actionType;
     public InteractType interactType;
     public SkillType skillType;
     public int value = 0;

}


