using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenItem : BaseFloorItem
{
    ChickenController chicken;
    protected override void Awake()
    {
        base.Awake();
        chicken = this.GetComponent<ChickenController>();
    }

    protected override void Update()
    {
        base.Update();
        if(state == EquipmentState.Drag)
        {
            chicken.agent.transform.position = this.transform.position;
        }
    }
}
