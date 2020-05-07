using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CleanItem : BaseFloorItem
{
	public float clean = 1f;
	//bool isTouch = false;
	protected ItemDirty dirtyItem;
	//float targetAngle = 0;
	int soundId = 0;
	float time = 1;
	float maxTime = 0.5f;



	protected override void Start(){

		base.Start();
		this.clean = DataHolder.GetItem(this.itemID).value;
	}

	protected override void Update()
	{
		base.Update();
		if(dirtyItem != null){
			if(this.itemType == ItemType.Clean && dirtyItem.dirty <= clean*Time.deltaTime){
				ItemManager.instance.SpawnHeart(1,this.transform.position);
				GameManager.instance.LogAchivement(AchivementType.Clean);
            }
			
			if(this.itemType == ItemType.Clean && time > maxTime){
				MageManager.instance.PlaySound3D("Item_Broom", false,this.transform.position);
				time  = 0;
			}else{
				time += Time.deltaTime;
			}
			dirtyItem.OnClean(clean*Time.deltaTime);
			if(animator != null)
				animator.Play("Active");
        }
        else
        {
            if(this.itemType == ItemType.Clean)
				animator.Play("Idle");
        }
			
	}


    void OnTriggerStay2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() != null) {
			dirtyItem = other.GetComponent <ItemDirty>();
		}
	}


	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() == dirtyItem) {
			dirtyItem = null;
			time = maxTime;
		}
	}

}
