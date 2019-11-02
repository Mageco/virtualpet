using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrinkBowlItem : MonoBehaviour
{
	public float initZ = -6;
	public float scaleFactor = 0.1f;
	Vector3 originalScale;
	Vector3 dragOffset;
	public bool isDrag = false;
	public bool isDragable = true;
	Vector3 originalPosition;
	Vector3 lastPosition;
	public bool isBusy = false;
	float foodAmount;
	public float maxfoodAmount = 200;
	public SpriteRenderer image;
	public Sprite[] foodSprites;
	public Transform anchor;

	float time;
	float maxDoubleClickTime = 0.4f;
	bool isClick = false;

	void Awake(){
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
	}
    // Start is called before the first frame update
    void Start()
    {
		foodAmount = maxfoodAmount - 1;   
    }

    // Update is called once per frame
    void Update()
    {
		int id = (int)(foodAmount/(maxfoodAmount/foodSprites.Length));
		image.sprite = foodSprites [id];
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
		pos.z = this.transform.position.y;
		this.transform.position = pos;
	}

	public void Eat(float f)
	{
		foodAmount -= f;
		if (foodAmount < 0)
			foodAmount = 0;
	}

	public bool CanEat()
	{
		if (foodAmount <= 0)
			return false;
		else
			return true;
	}

	void OnMouseUp()
	{

		dragOffset = Vector3.zero;

		if (isDrag && !isDragable)
			StartCoroutine (ReturnPosition (lastPosition));
		isDrag = false;
		//InputController.instance.ResetCameraTarget();

		if (isClick) {
			if (time > maxDoubleClickTime) {
				time = 0;
			} else {
				Fill ();
				time = 0;
				isClick = false;
				return;
			}
		} else {
			time = 0;
			isClick = true;
		}
	}

	void Fill()
	{
		foodAmount = maxfoodAmount-1;
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



		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isDrag = true;
		lastPosition = this.transform.position;

		//InputController.instance.cameraController.SetTarget (this.gameObject);
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


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
