﻿using System.Collections;
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
		}
	}

	public virtual void OnActive(){
		if(anim != null)
			anim.Play("Active",0);
	}

	public virtual void Deactive(){
		if(anim != null)
			anim.Play("Idle",0);
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() != null) {
			dirtyItem = other.GetComponent <ItemDirty>();
			OnActive();
		}
	}


	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() == dirtyItem) {
			dirtyItem = null;
			Deactive();
		}
	}

}
