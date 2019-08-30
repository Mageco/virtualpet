using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CleanItem : MonoBehaviour
{

	bool isTouch = false;
	ItemDirty dirtyItem;
	float targetAngle = 0;

	void Update()
	{
		if (isTouch) {
			targetAngle = Mathf.Lerp (targetAngle, 0, Time.deltaTime * 4);
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.Euler (new Vector3 (0, 0,-targetAngle)), Time.deltaTime * 2);
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



	public void OnFingerSwipe(LeanFinger finger)
	{
		if (isTouch) {
			targetAngle = finger.ScreenDelta.x;
			if (targetAngle > 45)
				targetAngle = 45;
			if (targetAngle < -45)
				targetAngle = -45;

			if (dirtyItem != null) {
				dirtyItem.OnClean ();
			}
		}
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
