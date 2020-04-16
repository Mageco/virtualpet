using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class MSpriteButton : MonoBehaviour {
	[Serializable]
	public class ButtonClickedEvent : UnityEvent { }
	[SerializeField]
	private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

	public bool interactable = true;
    Vector3 clickPos = Vector3.zero;
	private Animator anim;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}

	public ButtonClickedEvent onClick
	{
		get { return m_OnClick; }
		set { m_OnClick = value; }
	}
	
	// Update is called once per frame
	void Update () {
 	}

    private void OnMouseDown()
    {
        clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
	{
		if (IsPointerOverUIObject ())
		{
			return;
		}


        Vector3 upPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(interactable && Vector3.Distance(upPos,clickPos) < 0.5f)
        {
			MageManager.instance.PlaySound("BubbleButton", false);
			m_OnClick.Invoke();
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
