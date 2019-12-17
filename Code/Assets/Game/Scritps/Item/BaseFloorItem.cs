using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class BaseFloorItem : MonoBehaviour
{
	public float initZ = -6;
	public float scaleFactor = 0.05f;
	protected Vector3 originalScale;
	Vector3 dragOffset;
	public bool isDrag = false;
	public bool isDragable = true;
	protected Vector3 originalPosition;
	protected Vector3 lastPosition;
	public bool isBusy = false;
	public ItemObject item;

	protected float dragTime = 0;

	protected virtual void Awake(){
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
			if(pos.y > -3)
				pos.y = -3;
			if(pos.y < -24)
				pos.y = -24;
			
            if (pos.x > 52)
                pos.x = 52;
            else if (pos.x < -49)
                pos.x = -49;
			this.transform.position = pos;	
			dragTime += Time.deltaTime;		 
		}
		
    }

	protected virtual void LateUpdate()
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
		dragTime = 0;
	}


	protected virtual void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

		if (isBusy)
        {
			return;
            //StopAllCoroutines();
            //isBusy = false;
        }
		if (!isDragable)
			StartCoroutine (ReturnPosition (lastPosition));



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
		isDrag = false;
		dragOffset = Vector3.zero;

	}




	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
