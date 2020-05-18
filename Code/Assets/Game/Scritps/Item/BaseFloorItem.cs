using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseFloorItem : MonoBehaviour
{
	public int realID = 0;
	public int itemID = 0;
	protected Animator animator;
	public ItemType itemType = ItemType.All;
	public EquipmentState state = EquipmentState.Idle;
	protected float scaleFactor = 0.01f;
	protected Vector3 dragOffset;	
	protected Vector3 originalPosition;
	protected Vector3 originalScale;
	protected Vector3 lastPosition;
	protected float dragTime = 0;
	protected Vector2 boundX = new Vector2(-270, 52);
	protected Vector2 boundY = new Vector2(-24, -3);
    protected Animator[] animParts;
	public List<CharController> pets = new List<CharController>();
	protected Vector3 clickPosition;
	GameObject arrow;
	List<Color> colors = new List<Color>();
	SpriteRenderer[] sprites;
	public ObstructItem obstructItem;
	public Transform[] anchorPoints;
	public Transform startPoint;
	public Transform endPoint;
	public ToyType toyType = ToyType.None;
	public List<AnchorPoint> points = new List<AnchorPoint>();
	float highlight = 1;

	protected virtual void Awake()
	{
		animator = this.GetComponent<Animator>();
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
		lastPosition = this.transform.position;
		LoadSprite();
		obstructItem = this.GetComponentInChildren<ObstructItem>(true);
		animParts = this.GetComponentsInChildren<Animator>(true);
        if(anchorPoints != null)
        {
			for (int i = 0; i < anchorPoints.Length; i++)
			{
				points.Add(anchorPoints[i].gameObject.AddComponent<AnchorPoint>());
                
			}
		}
	}

	void LoadSprite()
	{
		sprites = GetComponentsInChildren<SpriteRenderer>(true);
		colors.Clear();
		for (int i = 0; i < sprites.Length; i++)
		{
			colors.Add(sprites[i].color);
		}
	}

	protected virtual void Start()
	{
		if (itemType == ItemType.Room || itemType == ItemType.Gate)
		{

		}
		else
		{
			if (itemType == ItemType.Picture || itemType == ItemType.Clock || itemType == ItemType.MedicineBox)
			{
				boundX = ItemManager.instance.roomBoundX;
				boundY = ItemManager.instance.roomWallBoundY;				
			}
			else
			{
				boundX = ItemManager.instance.gardenBoundX;
				boundY = ItemManager.instance.gardenBoundY;
				Vector3 pos = this.transform.position;
				pos.z = pos.y * 10;
				this.transform.position = pos;
			}
			for (int i = 0; i < sprites.Length; i++)
			{
				sprites[i].enabled = false;
			}
			StartCoroutine(CheckCollide());
		}


	}

    public virtual void Load(PlayerItem item)
    {
		this.itemType = item.itemType;
		this.itemID = item.itemId;
		this.realID = item.realId;
	}

	IEnumerator CheckCollide()
	{
		yield return new WaitForSeconds(0.5f);
		int n = 0;
		while (obstructItem != null && obstructItem.itemCollides.Count > 0 && n < 100)
		{
			if (itemType == ItemType.Bath || itemType == ItemType.Bed || itemType == ItemType.Toilet || itemType == ItemType.Food ||
				itemType == ItemType.Drink || itemType == ItemType.Table )
				this.transform.position = ItemManager.instance.GetRandomPoint(AreaType.Room);
            else if (itemType == ItemType.Fruit)
				this.transform.position = ItemManager.instance.GetRandomPoint(AreaType.Garden);
            else if (itemType == ItemType.MedicineBox || itemType == ItemType.Picture || itemType == ItemType.Clock)
				this.transform.position = ItemManager.instance.GetRandomPoint(AreaType.Wall);
            else
				this.transform.position = ItemManager.instance.GetRandomPoint(AreaType.All);

			//Debug.Log(ItemManager.instance.GetRandomPoint(AreaType.All));
			n++;
			yield return new WaitForEndOfFrame();
		}

		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].enabled = true;
		}

        if(n >= 100)
        {
			MageManager.instance.OnNotificationPopup(DataHolder.Dialog(158).GetName(MageManager.instance.GetLanguage()));
			GameManager.instance.UnEquipItem(this.realID);
        }
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (GameManager.instance.isGuest)
		{
			return;
		}

		if (state == EquipmentState.Hold)
		{
			dragTime += Time.deltaTime;
			if (dragTime > 0.2f)
			{
				if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 5f)
				{
					if (arrow == null)
					{
					    arrow = GameObject.Instantiate(Resources.Load("Prefabs/Effects/AllDirectionArrow")) as GameObject;
						arrow.transform.parent = this.transform;
						Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						pos.z = -500;
						pos.y += 5;
						arrow.transform.position = pos;
						MageManager.instance.PlaySound("BubbleButton", false);
						LoadSprite();
					}
				}
				else
				{
					if (arrow != null && state != EquipmentState.Drag)
						Destroy(arrow);
				}
			}

			if (dragTime > 0.3f && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) > 2f && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 5f)
			{
				OnDrag();
			}else if(dragTime > 0.5f && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 2f)
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
			if ((obstructItem != null && obstructItem.itemCollides.Count > 0))
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
		if (itemType != ItemType.Clock && itemType != ItemType.MedicineBox && itemType != ItemType.Picture && itemType != ItemType.Clock && itemType != ItemType.Room && itemType != ItemType.Gate)
		{
			transform.localScale = originalScale * (1 + (-transform.position.y) * scaleFactor) * highlight;
			Vector3 pos = this.transform.position;
			pos.z = this.transform.position.y * 10;
			this.transform.position = pos;
		}
	}

	protected virtual void OnMouseUp()
	{
		if (state == EquipmentState.Drag || state == EquipmentState.Hold)
		{
			if (dragTime < 0.4f && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 2f)
			{
				OnClick();
			}
			else
			{ 

				if (obstructItem != null && obstructItem.itemCollides.Count > 0)
				{
					StartCoroutine(ReturnPosition(lastPosition));
					MageManager.instance.PlaySound("Drag", false);
				}

				else
				{
					MageManager.instance.PlaySound("BubbleButton", false);
					state = EquipmentState.Idle;
				}

				OffDrag();
			}

		}

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
            if(sprites[i] != null)
			    sprites[i].color = colors[i];
		}
	}

	protected virtual void OnMouseDown()
	{

		if(itemType == ItemType.Room || itemType == ItemType.Gate)
			return;

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



	protected virtual void OnDrag()
	{
		if (animator != null)
			animator.enabled = false;
		for (int i = 0; i < animParts.Length; i++)
		{
			animParts[i].enabled = false;
		}

		dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
		state = EquipmentState.Drag;
		lastPosition = this.transform.position; 
		ItemManager.instance.SetCameraTarget(this.gameObject);
	}

	protected virtual void OffDrag()
	{
		for (int i = 0; i < animParts.Length; i++)
		{
			animParts[i].enabled = true;
		}
		if (animator != null)
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
		state = EquipmentState.Idle;
        
	}

	public virtual void OnActive()
	{
		state = EquipmentState.Active;
		if (animator != null)
			animator.Play("Active", 0);
	}

	public virtual void DeActive()
	{
		state = EquipmentState.Idle;
		if (animator != null)
			animator.Play("Idle", 0);
	}

	public bool IsActive()
	{
		if (state == EquipmentState.Active)
			return true;
		else
			return false;
	}

	public int GetPetIndex(CharController pet)
	{
		for (int i = 0; i < pets.Count; i++)
		{
			if (pets[i] == pet)
				return i;
		}
		return -1;
	}

    
    public void AddPet(CharController pet)
    {
        if (!pets.Contains(pet))
        {
			pets.Add(pet);
			foreach (AnchorPoint p in points)
			{
				if (p.pet == null)
				{
					p.pet = pet;
					return;
				}
			}
        }
    }

    bool IsPetAnchor(CharController pet)
    {
		foreach (AnchorPoint p in points)
		{
			if (p.pet == pet)
			{
				return true;
			}
		}
		return false;
	}

    public void RemovePet(CharController pet)
    {
        if (pets.Contains(pet))
        {
			pets.Remove(pet);
			foreach (AnchorPoint p in points)
			{
				if (p.pet == pet)
				{
					p.pet = null;
					break;
				}
			}
        }
    }

    public Transform GetAnchorPoint(CharController pet)
    {
		foreach (AnchorPoint p in points)
		{
			if (p.pet == pet)
			{
				return p.transform;
			}
		}
		foreach (AnchorPoint p in points)
		{
			if (p.pet == null)
			{
				return p.transform;
			}
		}
		return endPoint;
	}

    public bool IsBusy()
    {
		if (pets.Count >= anchorPoints.Length)
		{
			return true;
		}
		else
			return false;
    }

    public void OnHighlight()
    {
		highlight = 1.1f;
        for(int i = 0; i < sprites.Length; i++)
        {
            if(sprites[i] != null)
			    sprites[i].sharedMaterial = ItemManager.instance.highlightMaterial;
        }
    }

    public void OffHighlight()
    {
		highlight = 1;
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i] != null)
				sprites[i].sharedMaterial = ItemManager.instance.defaultMaterial;
		}
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
