using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BathShowerItem : MonoBehaviour
{
	public float maxDistance = 10;
	Vector3 dragOffset;
	Animator anim;
	bool isDrag = false;
	Vector3 originalPosition;
	bool isBusy = false;
	bool isShower = false;
	public GameObject showerEffect;

	CharBath character;

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		showerEffect.SetActive (false);
		character = GameObject.FindObjectOfType<CharBath>();
	}

    // Update is called once per frame
    void Update()
	{		
	
		if (isDrag) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
			pos.z = 0;
			pos.x = this.transform.position.x;

			if (pos.y > originalPosition.y)
				pos.y = originalPosition.y;
			else if (pos.y < originalPosition.y - maxDistance)
				pos.y = originalPosition.y - maxDistance;

			this.transform.position = pos + new Vector3 (0, 0, -10);
		}

		if (this.transform.position.y < (originalPosition.y - maxDistance + 1)) {
			if (!isShower)
				OnShower ();
		} else {
			if (isShower)
				OffShower ();
		}
        
    }

	void OnShower(){
		isShower = true;
		showerEffect.SetActive (true);
		Debug.Log ("Shower");
		character.OnShower ();
		ItemController.instance.bathTubeItem.OnShower ();
	}

	void OffShower(){
		isShower = false;
		showerEffect.SetActive (false);
		Debug.Log ("OffShower");
		character.OffShower ();
	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

		if (isBusy)
			return;
		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isDrag = true;

	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		if(isDrag)
			StartCoroutine (Return ());
		isDrag = false;
	}

	IEnumerator Return()
	{
		isBusy = true;
		while (Vector2.Distance (this.transform.position, originalPosition) > 1f) {
			this.transform.position = Vector3.Lerp (this.transform.position, originalPosition, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame ();
		}
		isBusy = false;
	}
		

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
