using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public int iD =0;
    public LanguageItem[] languageItem = new LanguageItem[0];
    //Requirement
   



    //Rewards


    //Dialog
}

public class QuestRequirement{
     public int charLevel;
     public int diaLogId;
     public QuestRequirementType requireType;

     public ActionType actionType;
     public InteractType interactType;
     public SkillType skillType;
     public int value = 0;

}


