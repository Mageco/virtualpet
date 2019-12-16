using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyWaterItem : MonoBehaviour
{
    public Transform jumpPoint;
    ToyType toyType = ToyType.WaterJet;
    Animator animator;

    void Awake(){
        animator = this.GetComponent<Animator>();
    }


    public void OnActive(){
        animator.Play("Active");
    }
}
