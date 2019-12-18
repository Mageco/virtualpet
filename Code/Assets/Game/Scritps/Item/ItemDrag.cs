using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class ItemDrag : MonoBehaviour
{
	Vector3 dragOffset;
	protected Animator anim;
	protected bool isDrag = false;
	protected Vector3 originalPosition;
	protected Quaternion originalRotation;
	Vector3 lastPosition;
	protected bool isDragable = true;
	protected bool isBusy = false;
	public bool isReturn = false;
	public bool isObstruct = true;
	public bool isDragOffset = true;
	public bool isCameraControl = false;

	public bool isIgnoreUI = true;

	protected virtual void Awake()
	{
		anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		originalRotation = this.transform.rotation;
	}

	protected virtual void Start(){

	}

	// Update is called once per frame
	void Update()
	{		
		if (isDrag ) {
			if (isDragable) {
				Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
				pos.z = this.transform.position.z;
				if (!isObstruct)
					pos.z = -500;
				this.transform.position = pos;
			} 
		}

	}

	public void StartDrag(){
		OnMouseDown();
	}

	public void EndDrag(){
		OnMouseUp();
	}

	void OnMouseDown()
	{
		if (isIgnoreUI && IsPointerOverUIObject ()) {
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

		if (isCameraControl) {
			GameManager.instance.SetCameraTarget (this.gameObject);
		}
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
		if (isCameraControl) {
			GameManager.instance.ResetCameraTarget();
		}
	}

	protected IEnumerator ReturnPosition(Vector3 pos)
	{
		isBusy = true;
		while (Vector2.Distance (this.transform.position, pos) > 0.1f) {
			this.transform.position = Vector3.Lerp (this.transform.position, pos, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame ();
		}
		isBusy = false;
		
	}

	protected IEnumerator ReturnRotation(Quaternion rot)
	{
		isBusy = true;
		while (Quaternion.Angle (this.transform.rotation, rot) > 1f) {
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rot, Time.deltaTime * 40);
			yield return new WaitForEndOfFrame ();
		}
		isBusy = false;
	}


	void OnTriggerEnter2D(Collider2D other) {
		//if(isBusy)
		//	return;
		if (other.tag == "Floor" && isObstruct) {
		 	isDragable = true;
		}
		 if (other.GetComponent<PolyNavObstacle>() != null && isObstruct) {
		 	isDragable = false;
		 }

		 if(other.tag == "Player"){
			 OnActive(other.GetComponent<CharController>());
		 }

	}

	void OnTriggerExit2D(Collider2D other) {
		if(isBusy)
			return;
		if (other.GetComponent<PolyNavObstacle>() != null && isObstruct) {
		 	isDragable = true;
		}
		if (other.tag == "Floor" && isObstruct) {
			isDragable = false;
		}

	}

	protected virtual void OnActive(CharController pet){

	}


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
