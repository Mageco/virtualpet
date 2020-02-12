using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCollider : MonoBehaviour
{
    public float height = 5;
    public float depth = 2;
    public float width = 4;
    public float edge = 3;

	protected Animator animator;
	public Transform anchorPoint;
	public Transform startPoint;
	public Transform endPoint;
	[HideInInspector]
	public ItemObject item;
	protected Vector3 dragOffset;
	public EquipmentState state = EquipmentState.Idle;
	protected Vector3 originalPosition;
	protected Vector3 originalScale;
    [HideInInspector]
	public Vector3 lastPosition;
	protected float dragTime = 0;
	public Vector2 boundX = new Vector2(-40, 60);
	public Vector2 boundY = new Vector2(0, 0);
	public List<CharController> pets = new List<CharController>();

	
	ItemCollider itemCollide;

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
		if (state == EquipmentState.Drag)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
			pos.z = this.transform.position.z;

			if (pos.y > boundY.y)
				pos.y = boundY.y;
			else if (pos.y < boundY.x)
				pos.y = boundY.x;

			if (pos.x > boundX.y)
				pos.x = boundX.y;
			else if (pos.x < boundX.x)
				pos.x = boundX.x;

            this.transform.position = pos;
			dragTime += Time.deltaTime;
		}
	}


	protected virtual void OnMouseUp()
	{
		if (state == EquipmentState.Drag)
		{
			if (dragTime < 0.1f)
			{
				OnClick();
			}
			else
			{
				if (itemCollide != null)
					StartCoroutine(ReturnPosition(lastPosition));
                else
				    state = EquipmentState.Idle;
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

		if (state == EquipmentState.Active || state == EquipmentState.Busy)
		{
			return;
		}

		dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
		state = EquipmentState.Drag;
		lastPosition = this.transform.position;
		ItemManager.instance.SetCameraTarget(this.gameObject);
	}

	IEnumerator ReturnPosition(Vector3 pos)
	{
		state = EquipmentState.Busy;
		while (Vector2.Distance(this.transform.position, pos) > 0.1f)
		{
			this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame();
		}
		state = EquipmentState.Idle;
		dragOffset = Vector3.zero;
	}

	protected virtual void OnClick()
	{
		state = EquipmentState.Idle;
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

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<ItemCollider>() != null)
		    itemCollide = other.GetComponent<ItemCollider>();
		Debug.Log(other.name);
	}

	void OnTriggerExit2D(Collider2D other)
	{
        if(other.GetComponent<ItemCollider>() != null && other.GetComponent<ItemCollider>() == itemCollide)
        {
			itemCollide = null;
        }
	}
}

