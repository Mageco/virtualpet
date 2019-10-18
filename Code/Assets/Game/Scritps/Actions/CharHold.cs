using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
public class CharHold : CharAction
{
    //Interact
	bool isTouch = false;
	Vector3 dragOffset;
	Vector3 dropPosition;
	[HideInInspector]
	public float fallSpeed = 0;
    HoldType holdType = HoldType.Drag;
    // Start is called before the first frame update
    EnviromentType enviromentType = EnviromentType.Room;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(character.actionType == ActionType.Hold){ 
            if(holdType == HoldType.Drag)      {
                Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
                pos.z = 0;
                if (pos.y > 20)
                    pos.y = 20;
                else if (pos.y < -20)
                    pos.y = -20;

                if (pos.x > 60)
                    pos.x = 60;
                else if (pos.x < -60)
                    pos.x = -60;

                pos.z = -50;
                agent.transform.position = pos;
                anim.Play("Hold_D");
                this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.identity, Time.deltaTime * 2);
            }else if(holdType == HoldType.Drop){
                fallSpeed += 100f * Time.deltaTime;
                if (fallSpeed > 50)
                    fallSpeed = 50;
                Vector3 pos = agent.transform.position;
                pos.y -= fallSpeed * Time.deltaTime;
                pos.z = dropPosition.z;
                agent.transform.position = pos;
                if (Vector2.Distance (agent.transform.position, dropPosition) < fallSpeed * Time.deltaTime * 2) {

                    if (fallSpeed < 50) {
                        anim.Play("Drop_Light_D");
                        maxInteractTime = 2;
                    } else {
                        anim.Play("Drop_Hard_D");
                        maxInteractTime = 3;
                    }

                    interactTime = 0;
                    fallSpeed = 0;
                    this.transform.rotation = Quaternion.identity;
                    holdType = HoldType.Fall;
                }
            }else if(holdType == HoldType.Fall){
                if (interactTime > maxInteractTime) {
                    if (enviromentType == EnviromentType.Bath) {
                        character.OnBath ();
                    } else
                        EndAction();
                } else {
                    interactTime += Time.deltaTime;
                }
            }
        }

        
    }

    void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}
		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		holdType = HoldType.Drag;
	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		isTouch = false;
		if (holdType == HoldType.Drag) {
			OnDrop ();
		} 
	}
    void OnDrop()
	{
		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0,-2,0), -Vector2.up,100);

		Vector3 pos = this.transform.position;
		pos.y = pos.y - 22;
		if (pos.y < -20)
			pos.y = -20;
		dropPosition = pos;
		enviromentType = EnviromentType.Room;

		for (int i = 0; i < hit.Length; i++) {
			if (hit[i].collider.tag == "Table") {
				pos.y = hit[i].collider.transform.position.y;
				dropPosition = pos;
				enviromentType = EnviromentType.Table;
				break;
			}else if(hit[i].collider.tag == "Bath") {
				pos.y = hit[i].collider.transform.position.y;
				pos.z = hit [i].collider.transform.position.z;
				dropPosition = pos;
				enviromentType = EnviromentType.Bath;
				break;
			}
		}
        holdType = HoldType.Drop;
	}

    public void OnFingerSwipe(LeanFinger finger)
	{
		if (holdType == HoldType.Drag) {
			float angle = finger.ScreenDelta.x;
			if (angle > 30)
				angle = 30;
			if (angle < -30)
				angle = -30;

			this.transform.rotation = Quaternion.Euler (0, 0, -angle);
		}
	}
}

public enum HoldType {Drag,Drop,Fall}
