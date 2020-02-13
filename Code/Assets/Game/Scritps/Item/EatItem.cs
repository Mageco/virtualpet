﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class EatItem : MonoBehaviour
{
	public ItemSaveDataType itemSaveDataType = ItemSaveDataType.None;
	public float foodAmount = 0;
	public float maxfoodAmount = 100;
	public SpriteRenderer image;
	public Sprite[] foodSprites;
    

	public Transform anchor;

	protected Animator animator;
	[HideInInspector]
	public ItemObject item;
	public float initZ = -6;
	public float scaleFactor = 0.05f;
	protected Vector3 dragOffset;
	public EquipmentState state = EquipmentState.Idle;
	protected Vector3 originalPosition;
	protected Vector3 originalScale;
	public Vector3 lastPosition;
	protected float dragTime = 0;
	public Vector2 boundX = new Vector2(-270, 52);
	public Vector2 boundY = new Vector2(-24, -3);
	public List<CharController> pets = new List<CharController>();
	Vector3 clickPosition;
	GameObject arrow;
	List<Color> colors = new List<Color>();
	public SpriteRenderer[] sprites;
	ObstructItem obstructItem;
	public MovementType movementType = MovementType.FourDirection;

	int foodId = 0;
	int lastId = -1;

	protected virtual void Awake()
	{
		animator = this.GetComponent<Animator>();
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
		lastPosition = this.transform.position;
		sprites = GetComponentsInChildren<SpriteRenderer>(true);
		for (int i = 0; i < sprites.Length; i++)
		{
			colors.Add(sprites[i].color);
		}
		obstructItem = this.GetComponentInChildren<ObstructItem>(true);
	}

	protected virtual void Start()
	{
		item = this.transform.parent.GetComponent<ItemObject>();
		maxfoodAmount = DataHolder.GetItem(item.itemID).value;
		
	}


	// Update is called once per frame
	protected virtual void Update()
	{
		foodId = (int)(foodAmount / (maxfoodAmount / foodSprites.Length));
        if(lastId != foodId)
        {
			image.sprite = foodSprites[foodId];
			lastId = foodId;
		}
		    

		if (state == EquipmentState.Hold)
		{
			dragTime += Time.deltaTime;
			if (dragTime > 0.1f)
			{
				if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 0.1f)
				{
					if (arrow == null)
					{
						if (movementType == MovementType.FourDirection)
							arrow = GameObject.Instantiate(Resources.Load("Prefabs/Effects/AllDirectionArrow")) as GameObject;
						else if (movementType == MovementType.TwoDirection)
						{
							arrow = GameObject.Instantiate(Resources.Load("Prefabs/Effects/TwoDirectionArrow")) as GameObject;
						}
						arrow.transform.parent = this.transform;
						Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						pos.z = -500;
						arrow.transform.position = pos;
						MageManager.instance.PlaySoundName("BubbleButton", false);
					}
				}
				else
				{
					if (arrow != null)
						Destroy(arrow);
				}
			}

			if (dragTime > 0.5f && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 0.1f)
			{
				OnDrag();
			}
			else if (dragTime > 0.5f)
				OffDrag();


		}
		else if (state == EquipmentState.Drag)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
			pos.z = this.transform.position.z;
			if (pos.y > boundY.y)
				pos.y = boundY.y;
			if (pos.y < boundY.x)
				pos.y = boundY.x;

			if (pos.x > boundX.y)
				pos.x = boundX.y;
			else if (pos.x < boundX.x)
				pos.x = boundX.x;
			this.transform.position = pos;
			dragTime += Time.deltaTime;
			if (obstructItem.itemCollides.Count > 0)
			{
				SetColor(new Color(1, 0, 0, 0.5f));
			}
			else
				SetColor(new Color(1, 1, 1, 0.5f));
		}
		else
		{
			if (arrow != null)
				Destroy(arrow);

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
		if (state == EquipmentState.Drag || state == EquipmentState.Hold)
		{
			if (dragTime < 0.1f)
			{
				OnClick();
			}
			else
			{
				if (obstructItem.itemCollides.Count > 0)
				{
					StartCoroutine(ReturnPosition(lastPosition));
					MageManager.instance.PlaySoundName("Drag", false);
				}

				else
				{
					MageManager.instance.PlaySoundName("BubbleButton", false);
					state = EquipmentState.Idle;
				}
			}
		}
		OffDrag();
	}

	void SetColor(Color c)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].color = c;
		}
	}

	void ResetColor()
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].color = colors[i];
		}
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

		if (pets.Count > 0)
			return;

		clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		state = EquipmentState.Hold;
	}

	private void OnDrag()
	{
		animator.enabled = false;
		dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
		state = EquipmentState.Drag;
		lastPosition = this.transform.position;
		ItemManager.instance.SetCameraTarget(this.gameObject);
		if (arrow != null)
			Destroy(arrow);

	}

	void OffDrag()
	{
		animator.enabled = true;
		state = EquipmentState.Idle;
		dragOffset = Vector3.zero;
		dragTime = 0;
		ItemManager.instance.ResetCameraTarget();
		if (arrow != null)
			Destroy(arrow);
		ResetColor();
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
		Fill();
		state = EquipmentState.Idle;
	}


	protected bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}


	public void Eat(float f)
	{
		foodAmount -= f;
		if (foodAmount < 0){
			foodAmount = 0;
		}
			
	}

	public bool CanEat()
	{
		if (foodAmount <= 0)
			return false;
		else
			return true;
	}


	public void Fill()
	{
		if(foodAmount < maxfoodAmount-1)
			MageManager.instance.PlaySoundName("happy_collect_item_06",false);
		
		foodAmount = maxfoodAmount-1;
	}


}
