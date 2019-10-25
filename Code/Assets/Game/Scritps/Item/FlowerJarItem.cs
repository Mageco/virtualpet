using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowerJarItem : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
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
				pos.z = -50;
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
		if (isBusy)
			return;
		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isDrag = true;
		lastPosition = this.transform.position;

		InputController.instance.cameraController.SetTarget (this.gameObject);
	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		isDrag = false;
		InputController.instance.ResetCameraTarget();
	}


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
