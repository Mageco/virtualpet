using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SoapItem : MonoBehaviour
{

	bool isTouch = false;
	CharController character;
	public GameObject bubbleEffect;

	void Awake(){
		bubbleEffect.SetActive(false);
	}

	void Update()
	{
	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}
		isTouch = true;
	}

	void OnMouseUp()
	{
		bubbleEffect.SetActive(false);
		isTouch = false;
	}



	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent <CharController>() != null) {
			character = other.GetComponent <CharController>();
			if(character.actionType == ActionType.Bath && isTouch){
				bubbleEffect.SetActive(true);
				character.OnSoap();
				ItemController.instance.GetBathTubeItem().OnSoap ();
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.GetComponent <CharController>() == character) {
			character = null;
			bubbleEffect.SetActive(false);
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
