using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMapButton : MonoBehaviour
{

    public MapType mapType;
	Vector3 clickPos = Vector3.zero;

	// Use this for initialization
	void Start()
	{
	}


	// Update is called once per frame
	void Update()
	{
	}

	private void OnMouseDown()
	{
		if (GameManager.instance.isGuest)
			return;

		clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	void OnMouseUp()
	{
		if (GameManager.instance.isGuest)
			return;

		if (IsPointerOverUIObject())
		{
			return;
		}

		Vector3 upPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if (Vector3.Distance(upPos, clickPos) < 0.5f)
		{
            if(mapType == MapType.House)
            {
				UIManager.instance.OnMap(mapType);
			}else
			    UIManager.instance.OnMapRequirement(mapType);
		}

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
