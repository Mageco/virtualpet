using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CleanItem : MonoBehaviour
{

	bool isTouch = false;
	bool isCollide = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
			float angle = finger.ScreenDelta.x;
			if (angle > 30)
				angle = 30;
			if (angle < -30)
				angle = -30;

			this.transform.rotation = Quaternion.Euler (0, 0, -angle);

			if (isCollide) {

			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() != null) {
			isCollide = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <ItemDirty>() != null) {
			isCollide = false;
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
