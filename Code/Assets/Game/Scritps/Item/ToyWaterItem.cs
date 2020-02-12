using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : ToyItem
{
    public override void OnActive(){
        state = EquipmentState.Active;
        animator.Play("Active");
        MageManager.instance.PlaySoundName("Item_WaterJet", false);
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
