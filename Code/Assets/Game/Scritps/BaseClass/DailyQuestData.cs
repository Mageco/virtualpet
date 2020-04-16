using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyQuestData
{
    public int achivementId = 0;
    public int startValue = 0;
    public int requireValue = 1;
    public bool isCollected = false;
    public string timeCollected = "";
}
