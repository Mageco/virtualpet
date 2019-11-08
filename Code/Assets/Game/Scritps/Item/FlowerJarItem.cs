using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowerJarItem : BaseDragItem
{
	protected override void OnHit()
    {
        StartCoroutine(OnHitCoroutine());
    }

	IEnumerator OnHitCoroutine(){
		state = ItemDragState.Hited;
		if(this.transform.position.y < originalPosition.y - 2){
			Vector3 pos = this.transform.position;
			pos.z += 2;
			this.transform.position = pos;

			float l = Vector2.Distance(InputController.instance.Character.transform.position,this.transform.position);
			InputController.instance.Character.OnListening(9 + 30f/l);
			yield return StartCoroutine(DoAnim("Break"));
			InputController.instance.ResetCameraTarget();
			yield return new WaitForSeconds(2);
			this.transform.position = originalPosition;
			//CharController char = ;
		}else {
			yield return StartCoroutine(DoAnim("Shake"));
			InputController.instance.ResetCameraTarget();
			Vector3 pos = originalPosition;
			pos.x = this.transform.position.x;
			pos.y = this.transform.position.y;
			this.transform.position = pos;
		}

		
		
		this.transform.rotation = originalRotation;
		fallSpeed = 0;
		anim.Play("Idle",0);
		state = ItemDragState.None;
		
	}
}

