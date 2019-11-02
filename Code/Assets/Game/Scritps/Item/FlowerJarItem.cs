using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowerJarItem : MonoBehaviour
{
	Vector3 dragOffset;
	Animator anim;
	ItemDragState state = ItemDragState.None;
	Vector3 originalPosition;
	Quaternion originalRotation;

	Vector3 originalScale;
	Vector3 lastPosition;
	bool isDragable = true;
	bool isBusy = false;
	
    float fallSpeed = 0;
    Vector3 dropPosition;

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	Vector3 dragScale;
	Vector3 targetScale = Vector3.one;
	float offset = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		originalRotation = this.transform.rotation;
		originalScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
     	if (state == ItemDragState.Drag ) {
			if (isDragable) {
				Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
				pos.z = this.transform.position.z;
				pos.z = -50;
				if (pos.y > 20)
                	pos.y = 20;
				else if (pos.y < -20)
					pos.y = -20;

				if (pos.x > 52)
					pos.x = 52;
				else if (pos.x < -49)
					pos.x = -49;
				this.transform.position = pos;
			} 
		}else if(state == ItemDragState.Drop ){
			RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0, -2, 0), -Vector2.up, 100);
			Vector3 pos2 = this.transform.position;
			pos2.y = pos2.y - 22;
			if (pos2.y < -20)
				pos2.y = -20;
			dropPosition = pos2;

			for (int i = 0; i < hit.Length; i++)
			{
				if (hit[i].collider.tag == "Table")
				{
					pos2.y = hit[i].collider.transform.position.y;
					dropPosition = pos2 + new Vector3(0,0.5f,0);
					break;
				}
			}


			state = ItemDragState.Fall;
		}else if(state == ItemDragState.Fall){			
			fallSpeed += 100f * Time.deltaTime;            
			if (fallSpeed > 50)
				fallSpeed = 50;
			Vector3 pos = this.transform.position;
			pos.y -= fallSpeed * Time.deltaTime;
			pos.z = dropPosition.y;
			this.transform.position = pos;
			if (Vector2.Distance (this.transform.position, dropPosition) < 1f) {
				state = ItemDragState.Hit;
			}
		}else if(state == ItemDragState.Hit){
			StartCoroutine(OnHit());
		}
    }

	void LateUpdate()
    {
		offset = initZ;
		if (state == ItemDragState.Drop || state == ItemDragState.Fall || state == ItemDragState.Hit)
		{
			//return;
			targetScale = dragScale;
		}
		else if (state == ItemDragState.Drag) {
			offset = initZ + 20;
			if (this.transform.position.y > 2)
			{
				if (this.transform.position.y < offset)
					targetScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
				else
					targetScale = originalScale;
			}else{
				targetScale = originalScale * (1 + (-2 + offset) * scaleFactor);
			}
			dragScale = targetScale;
		}else{
			if (this.transform.position.y < offset)
				targetScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
			else
				targetScale = originalScale;
		}

		//Debug.Log(targetScale);
		this.transform.localScale = Vector3.Lerp(targetScale,this.transform.localScale,Time.deltaTime *  3f);

    }

	IEnumerator OnHit(){
		state = ItemDragState.Hited;
		if(this.transform.position.y < originalPosition.y - 2){
			Vector3 pos = this.transform.position;
			pos.z += 2;
			this.transform.position = pos;

			float l = Vector2.Distance(InputController.instance.character.transform.position,this.transform.position);
			InputController.instance.character.OnListening(9 + 30f/l);
			yield return StartCoroutine(DoAnim("Break",2));
			InputController.instance.ResetCameraTarget();
			yield return new WaitForSeconds(2);
			this.transform.position = originalPosition;
			//CharController char = ;
		}else {
			yield return StartCoroutine(DoAnim("Shake",0.5f));
			InputController.instance.ResetCameraTarget();
			Vector3 pos = originalPosition;
			pos.x = this.transform.position.x;
			pos.y = this.transform.position.y;
			this.transform.position = pos;
		}

		
		
		this.transform.rotation = originalRotation;
		fallSpeed = 0;
		anim.Play("Idle",0);
		state = ItemDragState.None;
		
	}

	IEnumerator DoAnim(string a,float maxTime)
    {
        float time = 0;
        anim.Play(a, 0);
        yield return new WaitForEndOfFrame();
        while (time < maxTime)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
	
    public void OnFall(){

    }

    public void StartDrag(){
		OnMouseDown();
	}

	public void EndDrag(){
		OnMouseUp();
	}

	void OnMouseDown()
	{
		if(IsPointerOverUIObject())
			return;

		if (isBusy)
			return;

		if(state == ItemDragState.None){
			dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
			state = ItemDragState.Drag;
			lastPosition = this.transform.position;
			InputController.instance.cameraController.SetTarget (this.gameObject);
		}

	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		if(state == ItemDragState.Drag)
			state = ItemDragState.Drop;
		
	}


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

