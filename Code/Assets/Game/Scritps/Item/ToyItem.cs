using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToyItem : BaseFloorItem
{
	public ToyType toyType = ToyType.Ball;
	public Transform anchorPoint;
	public Transform startPoint;
	public Transform endPoint;

    public bool IsActive()
    {
        if (state == EquipmentState.Active)
            return true;
        else
            return false;
    }
}


