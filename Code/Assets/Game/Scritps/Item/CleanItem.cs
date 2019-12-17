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
			dirtyItem.OnClean(clean*Time.deltaTime);
			anim.Play("Active");
		}else
			anim.Play("Idle");
	}


	void OnTriggerStay2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() != null) {
			dirtyItem = other.GetComponent <ItemDirty>();
		}
	}


	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() == dirtyItem) {
			dirtyItem = null;
		}
	}

}
