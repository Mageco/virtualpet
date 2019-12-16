using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : ToyItem
{
   

    protected override void Awake(){
        animator = this.GetComponent<Animator>();
        toyType = ToyType.WaterJet;
    }


    public override void OnActive(){
        animator.Play("Active");
    }
}
