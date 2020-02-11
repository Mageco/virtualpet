using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : ToyItem
{
    public override void OnActive(){
        state = ToyState.Active;
        animator.Play("Active");
        MageManager.instance.PlaySoundName("Item_WaterJet", false);
        GameManager.instance.LogAchivement(AchivementType.Use_Item,ActionType.None,item.itemID);
    }

    public override void DeActive()
    {
        state = ToyState.Idle;
    }

    protected override void Update()
    {
        base.Update();
        if(state == ToyState.Active && pets.Count == 0)
        {
            state = ToyState.Idle;
        }
    }
}
