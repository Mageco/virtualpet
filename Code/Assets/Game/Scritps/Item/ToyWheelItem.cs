using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWheelItem : ToyItem
{
    protected override void Start()
    {
        base.Start();
        toyType = ToyType.Wheel;
    }


    public override void OnActive()
    {
        animator.Play("Active");
        MageManager.instance.PlaySoundName("Item_WaterJet", false);
        GameManager.instance.LogAchivement(AchivementType.Use_Item, ActionType.None, item.itemID);
    }
}
