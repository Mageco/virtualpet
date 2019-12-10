using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : BaseFloorItem
{
    public Transform jumpPoint;
    ToyType toyType = ToyType.WaterJet;

    protected override void OnCollidePet(CharController pet){
		    pet.OnToy(toyType);
	  }
}
