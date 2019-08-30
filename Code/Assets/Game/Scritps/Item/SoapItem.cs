using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class SoapItem : MonoBehaviour
{

	bool isTouch = false;
	CharController character;

	void Update()
	{
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
			if (character != null) {
				character.OnSoap ();
				ItemController.instance.bathTubeItem.OnSoap ();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent <CharController>() != null) {
			character = other.GetComponent <CharController>();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <CharController>() == character) {
			character = null;
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
