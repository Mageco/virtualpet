using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BathShowerItem : MonoBehaviour
{
	public float maxDistance = 10;
	Vector3 dragOffset;
	Animator anim;
	public bool isDrag = false;
	Vector3 originalPosition;
	public bool isBusy = false;
	public bool isShower = false;
	public GameObject showerEffect;
    BathTubeItem bathTube;
	CharController character;

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		showerEffect.SetActive (false);
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
		foreach(CharController pet in GameManager.instance.petObjects){
			if (pet.actionType == ActionType.OnBath) {
				pet.OnShower ();
			}
		}
		GetBathTube().OnShower ();
	}

	void OffShower(){
		isShower = false;
		showerEffect.SetActive (false);
		Debug.Log ("OffShower");
		foreach(CharController pet in GameManager.instance.petObjects){
			if (pet.actionType == ActionType.OnBath) {
				pet.OffShower ();
			}
		}
	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

		if (isBusy)
			return;
		GameManager.instance.SetCameraTarget(this.gameObject);
		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isDrag = true;

	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		if(isDrag)
			StartCoroutine (Return ());
		isDrag = false;
		GameManager.instance.ResetCameraTarget();
	}

	IEnumerator Return()
	{
		isBusy = true;
		while (Vector2.Distance (this.transform.position, originalPosition) > 1f) {
			this.transform.position = Vector3.Lerp (this.transform.position, originalPosition, Time.deltaTime * 5);
			yield return new WaitForEndOfFrame ();
		}
		this.transform.position = originalPosition;
		isBusy = false;
	}
		

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

    BathTubeItem GetBathTube()
    {
        if (bathTube == null)
            bathTube = FindObjectOfType<BathTubeItem>();
        return bathTube;
    }
}
