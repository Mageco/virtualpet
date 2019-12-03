using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ActionData 
{
    public AchivementType achivementType = AchivementType.Do_Action;
    public ActionType actionType;
    public int itemId;
    public AnimalType animalType;
    
    public DateTime startTime;
    public DateTime endTime;
    
}
