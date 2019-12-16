using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollItem : MonoBehaviour
{
    float activeTime = 0;
    bool isActive = false;
    Animator animator;

    void Awake(){
        animator = this.GetComponent<Animator>();
    }

    void OnMouseUp(){
        if(!isActive){
            isActive = true;
            StartCoroutine(OnActive());
        }
        
    }

    IEnumerator OnActive(){
        animator.Play("Active");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isActive = false;
        animator.Play("Idle");
    }

	
}
