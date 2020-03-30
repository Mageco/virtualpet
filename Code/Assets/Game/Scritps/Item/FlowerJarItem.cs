using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowerJarItem : BaseDragItem
{
	public GameObject breakObject;
	public Vector3 localPosition;
	public Transform parent;

	protected override void Start(){
		base.Start();
		state = ItemDragState.None;
		breakObject.SetActive(false);
		localPosition = this.transform.localPosition;
		parent = this.transform.parent;
		//Debug.Log(this.transform.localPosition);
		
	}
	protected override void OnHit()
    {
        StartCoroutine(OnHitCoroutine());
    }

    protected override void Update()
    {
        base.Update();
		//Debug.Log(this.transform.localPosition);
	}

    IEnumerator OnHitCoroutine(){
		state = ItemDragState.Hited;
		this.transform.parent = null;
		Vector3 pos = this.transform.position;
		pos.z = pos.y * 10 + 2;
		this.transform.position = pos;

		//float l = Vector2.Distance(GameManager.instance.GetPetObject(0).transform.position,this.transform.position);
		breakObject.SetActive(true);
        MageManager.instance.PlaySound("Item_Vase", false);
		yield return StartCoroutine(DoAnim("Break"));
        ItemManager.instance.ResetCameraTarget();
		yield return new WaitForSeconds(2);
		breakObject.SetActive(false);
		this.transform.parent = parent;
		this.transform.localPosition = localPosition;
        this.transform.rotation = originalRotation;
		height = originalHeight;
		this.scalePosition = this.transform.position + new Vector3(0,-height,0);
		
		fallSpeed = 0;
		anim.Play("Idle",0);
		state = ItemDragState.None;
		
	}
}

