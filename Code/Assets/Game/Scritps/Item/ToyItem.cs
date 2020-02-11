using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToyItem : MonoBehaviour
{
	public ToyType toyType = ToyType.Ball;
	protected Animator animator;
	public Transform anchorPoint;
	public Transform startPoint;
	public Transform endPoint;
    [HideInInspector]
	public ItemObject item;
	public float initZ = -6;
	public float scaleFactor = 0.05f;
	protected Vector3 dragOffset;
	public ToyState state = ToyState.Idle;
	protected Vector3 originalPosition;
	protected Vector3 originalScale;
	public Vector3 lastPosition;
	protected float dragTime = 0;
	public Vector2 boundX = new Vector2(-270, 52);
	public List<CharController> pets = new List<CharController>();

	protected virtual void Awake()
	{
		animator = this.GetComponent<Animator>();
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
		lastPosition = this.transform.position;
		
	}

    protected virtual void Start()
    {
		item = this.transform.parent.GetComponent<ItemObject>();
	}


    // Update is called once per frame
    protected virtual void Update()
	{
		if (state == ToyState.Drag)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
			pos.z = this.transform.position.z;
			if (pos.y > -3)
				pos.y = -3;
			if (pos.y < -24)
				pos.y = -24;

			if (pos.x > boundX.y)
				pos.x = boundX.y;
			else if (pos.x < boundX.x)
				pos.x = boundX.x;
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
        if(state == ToyState.Drag)
        {
			if (dragTime < 0.1f)
			{
				OnClick();
			}
			else
			{
				state = ToyState.Idle;
			}
			dragOffset = Vector3.zero;
			
			dragTime = 0;
		}
		ItemManager.instance.ResetCameraTarget();
	}


	protected virtual void OnMouseDown()
	{
		if (IsPointerOverUIObject())
		{
			return;
		}

		if (state == ToyState.Active || state == ToyState.Busy)
		{
			return;
		}

		dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
		state = ToyState.Drag;
		lastPosition = this.transform.position;
		ItemManager.instance.SetCameraTarget(this.gameObject);
	}

	IEnumerator ReturnPosition(Vector3 pos)
	{
		state = ToyState.Busy;
		while (Vector2.Distance(this.transform.position, pos) > 0.1f)
		{
			this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame();
		}
		state = ToyState.Idle;
		dragOffset = Vector3.zero;
	}

    protected virtual void OnClick()
    {
		state = ToyState.Idle;
	}

	public virtual void OnActive()
	{

	}

	public virtual void DeActive()
	{

	}

	protected bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

public enum ToyState {Idle,Drag,Busy,Active}
