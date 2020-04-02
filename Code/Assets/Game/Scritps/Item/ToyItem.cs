 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToyItem : BaseFloorItem
{
	public ToyType toyType = ToyType.Ball;
	public Transform startPoint;
	public Transform endPoint;
    public Transform[] anchorPoints;
    [HideInInspector]
    public float maxTime = 1;
    [HideInInspector]
    public int count = 0;

    public bool IsActive()
    {
        if (state == EquipmentState.Active)
            return true;
        else
            return false;
    }

    public int GetPetIndex(CharController pet)
    {
        for(int i=0;i< pets.Count;i++)
        {
            if (pets[i] == pet)
                return i;
        }
        return -1;
    }

    public override void DeActive()
    {
        state = EquipmentState.Idle;
        if (animator != null)
            animator.Play("Idle", 0);
        //count = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (count > pets.Count)
            count = pets.Count;
    }

}


