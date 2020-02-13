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

	GameObject roomCollide;
	Vector3 clickPosition;
	GameObject arrow;
	List<Color> colors = new List<Color>();
	SpriteRenderer[] sprites;
	ObstructItem obstructItem;
	public MovementType movementType = MovementType.FourDirection;

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
	}


	// Update is called once per frame
	protected virtual void Update()
	{
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
			if ((obstructItem != null && obstructItem.itemCollides.Count > 0) || roomCollide != null)
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

		List<CharController> temp = new List<CharController>();
        if(pets.Count > 0)
        {
            foreach(CharController pet in pets)
            {
                if(pet != null && pet.enviromentType == EnviromentType.Room)
                {
					temp.Add(pet);
                }
            }
        }

		foreach (CharController pet in temp)
		{
			pets.Remove(pet);
		}
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
				if ((obstructItem != null && obstructItem.itemCollides.Count > 0) || roomCollide != null)
					StartCoroutine(ReturnPosition(lastPosition));
				else
					state = EquipmentState.Idle;
			}
		}
		OffDrag();
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

	private void OnDrag()
	{
		dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
		state = EquipmentState.Drag;
		lastPosition = this.transform.position;
		ItemManager.instance.SetCameraTarget(this.gameObject);
		if (arrow != null)
			Destroy(arrow);

	}

	void OffDrag()
	{
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
		roomCollide = other.gameObject;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (roomCollide == other.gameObject)
		{
			roomCollide = null;
		}
	}
}

