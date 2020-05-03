using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWheelItem : ToyItem
{
    public override void OnActive()
    {
        state = EquipmentState.Active;
        animator.Play("Active");
        MageManager.instance.PlaySound3D("Item_WaterJet", false,this.transform.position);
    }
}
