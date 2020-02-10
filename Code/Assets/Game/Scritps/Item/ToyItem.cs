using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToyItem : MonoBehaviour
{

	public ToyType toyType = ToyType.Ball;
	protected Animator animator;
	public Transform anchorPoint;
	protected ItemObject item;
	public float initZ = -6;
	public float scaleFactor = 0.05f;
	Vector3 dragOffset;
	public bool isDrag = false;
	public bool isDragable = true;
	protected Vector3 originalPosition;
	protected Vector3 lastPosition;
	protected Vector3 originalScale;

	public bool isBusy = false;

	protected float dragTime = 0;

	protected virtual void Awake()
	{
		animator = this.GetComponent<Animator>();
		item = this.transform.parent.GetComponent<ItemObject>();
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
		if (isDrag && isDragable)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
			pos.z = this.transform.position.z;
			if (pos.y > -3)
				pos.y = -3;
			if (pos.y < -24)
				pos.y = -24;

			if (pos.x > 52)
				pos.x = 52;
			else if (pos.x < -270)
				pos.x = -270;
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
		isDrag = false;
		ItemManager.instance.ResetCameraTarget();
		dragTime = 0;
	}


	protected virtual void OnMouseDown()
	{
		if (IsPointerOverUIObject())
		{
			return;
		}

		if (isBusy)
		{
			return;
		}

		dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
		isDrag = true;
		lastPosition = this.transform.position;

		ItemManager.instance.SetCameraTarget(this.gameObject);
	}

	IEnumerator ReturnPosition(Vector3 pos)
	{
		isBusy = true;
		while (Vector2.Distance(this.transform.position, pos) > 0.1f)
		{
			this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame();
		}
		isBusy = false;
		isDragable = true;
		isDrag = false;
		dragOffset = Vector3.zero;

	}


	public virtual void OnActive()
	{

	}



	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
