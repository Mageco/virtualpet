﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class BaseFloorItem : MonoBehaviour
{
	public float initZ = -6;
	public float scaleFactor = 0.05f;
	Vector3 originalScale;
	Vector3 dragOffset;
	public bool isDrag = false;
	public bool isDragable = true;
	Vector3 originalPosition;
	Vector3 lastPosition;
	public bool isBusy = false;
	protected ItemObject item;

	void Awake(){
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
	}
    // Start is called before the first frame update
    protected virtual void Start()
    {
		item = this.transform.parent.GetComponent<ItemObject>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
		if (isDrag &&isDragable) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
			pos.z = this.transform.position.z;
			this.transform.position = pos;			 
		}
		
    }

	void LateUpdate()
	{
		float offset = initZ;

		if (transform.position.y < offset)
			transform.localScale = originalScale * (1 + (-transform.position.y + offset) * scaleFactor);
		else
			transform.localScale = originalScale;

		Vector3 pos = this.transform.position;
		pos.z = this.transform.position.y * 10;
		this.transform.position = pos;
	}

	protected virtual void OnMouseUp()
	{

		dragOffset = Vector3.zero;

		if (isDrag && !isDragable)
			StartCoroutine (ReturnPosition (lastPosition));
		isDrag = false;
		GameManager.instance.ResetCameraTarget();
	}


	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

		if (isBusy)
        {
            StopAllCoroutines();
            isBusy = false;
        }
		if (!isDragable)
			StartCoroutine (ReturnPosition (originalPosition));



		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isDrag = true;
		lastPosition = this.transform.position;

		GameManager.instance.SetCameraTarget(this.gameObject);
	}

	IEnumerator ReturnPosition(Vector3 pos)
	{
		isBusy = true;
		while (Vector2.Distance (this.transform.position, pos) > 0.1f) {
			this.transform.position = Vector3.Lerp (this.transform.position, pos, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame ();
		}
		isBusy = false;
		isDragable = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(isBusy)
			return;
		
		if (other.tag == "Floor") {
		 	isDragable = true;
		}
		 if (other.GetComponent<PolyNavObstacle>() != null) {
		 	isDragable = false;
		 }
	}

	void OnTriggerStay2D(Collider2D other) {
		if(isBusy)
			return;

		if(other.tag == "Pet"){
            OnCollidePet(other.GetComponent<CharController>());
		}
	}
	void OnTriggerExit2D(Collider2D other) {
		if(isBusy)
			return;

		if (other.GetComponent<PolyNavObstacle>() != null) {
		 	isDragable = true;
		}
		if (other.tag == "Floor") {
			isDragable = false;
		}
	}

    protected virtual void OnCollidePet(CharController pet){

    }


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
