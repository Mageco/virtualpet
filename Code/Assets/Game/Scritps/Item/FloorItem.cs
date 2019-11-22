using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorItem : MonoBehaviour
{
 public GameObject doubleClickEffect;

	float time;
	float maxDoubleClickTime = 0.4f;
	bool isClick = false;

	void Awake()
	{

	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
	void Update()
	{
		if (isClick)
			time += Time.deltaTime;
	}


	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

	}

	void OnMouseUp()
	{
		if (isClick) {
			if (time > maxDoubleClickTime) {
				time = 0;
			} else {
				OnDoubleClick ();
				time = 0;
				isClick = false;
				return;
			}
		} else {
			time = 0;
			isClick = true;
		}
	}

	void OnDoubleClick()
	{
        Debug.Log("Double Click");
		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos.z = 0;
		GameManager.instance.GetPetObject(0).OnCall(pos);
        if(doubleClickEffect != null){
            GameObject go = GameObject.Instantiate(doubleClickEffect,pos,Quaternion.identity);
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
