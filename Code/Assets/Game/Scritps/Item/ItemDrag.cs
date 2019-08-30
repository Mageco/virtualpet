using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour
{
	Vector3 dragOffset;
	Animator anim;
	bool isDrag = false;
	Vector3 originalPosition;
	Quaternion originalRotation;
	Vector3 lastPosition;
	bool isDragable = true;
	bool isBusy = false;
	public bool isReturn = false;
	public bool isObstruct = true;
	public bool isDragOffset = true;

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		originalRotation = this.transform.rotation;
	}

	// Update is called once per frame
	void Update()
	{		
		if (isDrag ) {
			if (isDragable) {
				Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
				pos.z = this.transform.position.z;
				if (!isObstruct)
					pos.z = -50;
				this.transform.position = pos;
			} 
		}

	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

		if (isBusy)
			return;

		if (!isDragable)
			StartCoroutine (ReturnPosition (originalPosition));

		if(isDragOffset)
			dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isDrag = true;
		lastPosition = this.transform.position;
	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;

		if (isDrag && isReturn) {
			StartCoroutine (ReturnPosition (originalPosition));
			StartCoroutine (ReturnRotation (originalRotation));
		}
		else if (isDrag && !isDragable)
			StartCoroutine (ReturnPosition (lastPosition));
		isDrag = false;
	}

	IEnumerator ReturnPosition(Vector3 pos)
	{
		isBusy = true;
		while (Vector2.Distance (this.transform.position, pos) > 0.1f) {
			this.transform.position = Vector3.Lerp (this.transform.position, pos, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame ();
		}
		isBusy = false;
	}

	IEnumerator ReturnRotation(Quaternion rot)
	{
		isBusy = true;
		while (Quaternion.Angle (this.transform.rotation, rot) > 1f) {
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rot, Time.deltaTime * 40);
			yield return new WaitForEndOfFrame ();
		}
		isBusy = false;
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent<PolyNavObstacle>() != null && isObstruct) {
			isDragable = false;
			Debug.Log ("Collide");
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent<PolyNavObstacle>() != null && isObstruct) {
			isDragable = true;
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
