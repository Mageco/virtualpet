using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowerJarItem : BaseDragItem
{
	public GameObject breakObject;

	protected override void Start(){
		base.Start();
		breakObject.SetActive(false);
	}
	protected override void OnHit()
    {
        StartCoroutine(OnHitCoroutine());
    }


	IEnumerator OnHitCoroutine(){
		state = ItemDragState.Hited;
		
		Vector3 pos = this.transform.position;
		pos.z += 2;
		this.transform.position = pos;

		float l = Vector2.Distance(GameManager.instance.GetPetObject(0).transform.position,this.transform.position);
		breakObject.SetActive(true);
		yield return StartCoroutine(DoAnim("Break"));
		GameManager.instance.ResetCameraTarget();
		yield return new WaitForSeconds(2);
		breakObject.SetActive(false);
		this.transform.position = originalPosition;
			//CharController char = ;
		
		this.transform.rotation = originalRotation;
		height = originalHeight;
		this.scalePosition = this.transform.position + new Vector3(0,-height,0);
		
		fallSpeed = 0;
		anim.Play("Idle",0);
		state = ItemDragState.None;
		
	}
}

