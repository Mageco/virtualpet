using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathTubeItem : BaseFloorItem
{
	public override void OnActive()
	{
		state = EquipmentState.Active;
		if (animator != null)
			animator.Play("Active", 0);
	}

	public override void DeActive()
	{
		state = EquipmentState.Idle;
		if (animator != null)
			animator.Play("Deactive", 0);
	}

}
