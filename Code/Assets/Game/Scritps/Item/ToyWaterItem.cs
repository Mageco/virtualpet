﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : ToyItem
{
   

    protected override void Start(){
        base.Start();
        toyType = ToyType.WaterJet;
    }


    public override void OnActive(){
        animator.Play("Active");
        GameManager.instance.LogAchivement(AchivementType.Use_Item,ActionType.None,item.itemID);
    }
}
