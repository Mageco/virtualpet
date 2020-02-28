using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : ToyItem
{
    public override void OnActive(){
        state = EquipmentState.Active;
        animator.Play("Active");
        MageManager.instance.PlaySound3D("Item_WaterJet", false,this.transform.position);
        GameManager.instance.LogAchivement(AchivementType.Use_Item,ActionType.None,item.itemID);
    }

    public override void DeActive()
    {
        state = EquipmentState.Idle;
    }

    protected override void Update()
    {
        base.Update();
        if(state == EquipmentState.Active && pets.Count == 0)
        {
            state = EquipmentState.Idle;
        }
    }
}
