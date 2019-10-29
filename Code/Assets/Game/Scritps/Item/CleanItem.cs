using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CleanItem : MonoBehaviour
{
	public float clean = 1f;
	bool isTouch = false;
	ItemDirty dirtyItem;
	float targetAngle = 0;

	Animator anim;

	void Awake(){
		anim = this.GetComponent<Animator>();
	}

	void Update()
	{
		if (isTouch) {
			anim.Play("Active",0);
			targetAngle = Mathf.Lerp (targetAngle, 0, Time.deltaTime * 4);
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.Euler (new Vector3 (0, 0,-targetAngle)), Time.deltaTime * 2);
			if(dirtyItem != null){
				dirtyItem.OnClean(clean);
			}
		}else
		{
			anim.Play("Idle",0);
		}
	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}
		isTouch = true;
	}

	void OnMouseUp()
	{
		isTouch = false;
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() != null) {
			dirtyItem = other.GetComponent <ItemDirty>();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() == dirtyItem) {
			dirtyItem = null;
		}
	}

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
