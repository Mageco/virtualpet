using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWheelItem : ToyItem
{
    public GameObject wheel;

    public override void OnActive()
    {
        state = EquipmentState.Active;
        animator.Play("Active");
        MageManager.instance.PlaySound3D("Item_WaterJet", false,this.transform.position);
        GameManager.instance.LogAchivement(AchivementType.Use_Item, ActionType.None, item.itemID);
    }

    public override void DeActive()
    {
        state = EquipmentState.Idle;
        animator.Play("Idle");
    }
}
