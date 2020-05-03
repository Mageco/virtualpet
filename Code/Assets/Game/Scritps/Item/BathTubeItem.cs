using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathTubeItem : BaseFloorItem
{
	public override void OnActive()
	{
		state = EquipmentState.Active;
		if (animator != null)
			animator.Play("Bath_Start", 0);
	}

	public override void DeActive()
	{
		state = EquipmentState.Idle;
		if (animator != null)
			animator.Play("Bath_End", 0);
	}

}
