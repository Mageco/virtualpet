using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScale : MonoBehaviour
{

	public float maxHeight = 10;
	public float scaleFactor = 0.1f;
	CharController character;
	CharInteract interact;
	Vector3 originalScale;
	Vector3 dragScale;
	public float height = 0;
	public Vector3 dropPosition = Vector3.zero;
	Vector3 lastPosition = Vector3.zero;

	void Awake()
	{
		character = this.GetComponent <CharController> ();
		originalScale = character.transform.localScale;
		interact = this.GetComponent<CharInteract>();
		
	}
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = this.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
		dropPosition.x = this.transform.position.x;
		if(interact.interactType == InteractType.Drag){
			float delta = this.transform.position.y - lastPosition.y;
			height += delta;
			if(height <= 0 && this.transform.position.y <= dropPosition.y ){
				Vector3 p = this.transform.position;
				p.y = lastPosition.y;
				character.agent.transform.position = p;
				this.transform.position = p;
				height = 0;
			}else{
				if(delta >= 0 && height > maxHeight){
					dropPosition.y += height - maxHeight;	
					height = maxHeight;
				}else if(delta < 0 && height > 0){
					if(dropPosition.y > -20){
						dropPosition.y += delta;
						height -= delta;
					}
				}		
			}
		}else if(interact.interactType == InteractType.None){
			dropPosition = this.transform.position;
			height = 0;
		}

		dragScale = originalScale * (1 - dropPosition.y * scaleFactor);
		character.transform.localScale = Vector3.Lerp(dragScale,character.transform.localScale,Time.deltaTime *  3f);

		Vector3 pos = this.transform.position;
		pos.z = dropPosition.y;
		this.transform.position = pos;

		lastPosition = this.transform.position;
    }
}
