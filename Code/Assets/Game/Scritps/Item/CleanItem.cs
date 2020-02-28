using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CleanItem : MonoBehaviour
{
	public float clean = 1f;
	//bool isTouch = false;
	protected ItemDirty dirtyItem;
	//float targetAngle = 0;

	protected Animator anim;

	protected ItemObject item;
	int soundId = 0;
	float time = 1;
	float maxTime = 0.5f;

	protected virtual void Awake(){
		anim = this.GetComponent<Animator>();
	}

	protected virtual void Start(){

		item = this.transform.parent.GetComponent<ItemObject>();
		this.clean = DataHolder.GetItem(item.itemID).value;
	}

	protected virtual void Update()
	{
		if(dirtyItem != null){
			if(item.itemType == ItemType.Clean && dirtyItem.dirty <= clean*Time.deltaTime){
				ItemManager.instance.SpawnHeart(1,this.transform.position);
				GameManager.instance.LogAchivement(AchivementType.Clean);
            }
			
			if(item.itemType == ItemType.Clean && time > maxTime){
				MageManager.instance.PlaySound3D("Item_Broom", false,this.transform.position);
				time  = 0;
			}else{
				time += Time.deltaTime;
			}
			dirtyItem.OnClean(clean*Time.deltaTime);
			if(anim != null)
				anim.Play("Active");
        }
        else
        {
            if(item.itemType == ItemType.Clean)
                anim.Play("Idle");
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
