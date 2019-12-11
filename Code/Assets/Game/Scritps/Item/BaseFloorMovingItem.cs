using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class BaseFloorMovingItem : CleanItem
{
    public float initZ = -6;
	public float scaleFactor = 0.05f;
	Vector3 originalScale;
	Vector3 originalPosition;
	Vector3 lastPosition;
	public bool isBusy = false;
	

	protected override void Awake(){
        base.Awake();
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
	}
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

	void LateUpdate()
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
        if(IsPointerOverUIObject()){
            return;
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
