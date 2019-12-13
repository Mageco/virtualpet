using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToyCarItem : BaseDragItem
{
	float time = 0;
	protected override void OnHit(){
		state = ItemDragState.Hited;
		StartCoroutine(OnHitCoroutine());	
	}

	IEnumerator OnHitCoroutine(){
				
		yield return StartCoroutine(DoAnim("Hit"));
		GameManager.instance.ResetCameraTarget();
		yield return new WaitForSeconds(0.5f);
		fallSpeed = 0;
		anim.Play("Idle",0);
		state = ItemDragState.None;	
	}

	
	
}

