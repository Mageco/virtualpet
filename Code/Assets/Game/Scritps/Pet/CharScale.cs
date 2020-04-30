﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScale : MonoBehaviour
{
    [HideInInspector]
	public float maxHeight = 15;
	float scaleFactor = 0.01f;
	CharController character;
	CharInteract interact;
	Vector3 originalScale;
	Vector3 dragScale;
	public float height = 0;
	public Vector3 scalePosition = Vector3.zero;
	Vector3 lastPosition = Vector3.zero;
    public float speedFactor = 1;
    //public float scaleAgeFactor = 1;
	

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
		if (interact.interactType == InteractType.Drag || interact.interactType == InteractType.Touch)
        {
			scalePosition.x = this.transform.position.x;
			float delta = this.transform.position.y - lastPosition.y;
			height += delta;
			if(height <= 0 && this.transform.position.y <= scalePosition.y ){
				scalePosition.y = this.transform.position.y;
				height = 0;
			}else{
				if(delta >= 0 && height > maxHeight){
					scalePosition.y += height - maxHeight;	
					height = maxHeight;
					if(scalePosition.y > -1){
						scalePosition.y = -1;
						Vector3 p = this.transform.position;
						p.y = lastPosition.y;
						character.agent.transform.position = p;
						this.transform.position = p;
					}
				}else if(delta < 0 && height > 0){
					if(scalePosition.y > -24){
						scalePosition.y += delta;
						height -= delta;
					}
				}		
			}
			if(character.shadow != null){
                character.shadow.transform.position = scalePosition + new Vector3(0,0,600);
				character.shadow.transform.localScale = character.originalShadowScale * (1f - 0.5f*height/maxHeight); 
            }
		}else if(interact.interactType == InteractType.Drop){
			scalePosition.x = this.transform.position.x;
			height = this.transform.position.y - scalePosition.y;
			if(height <= 0 && this.transform.position.y <= scalePosition.y ){
				Vector3 p = this.transform.position;
				p.y = scalePosition.y;
				character.agent.transform.position = p;
				this.transform.position = p;
				height = 0;
			}
			if(character.shadow != null){
                character.shadow.transform.position = scalePosition + new Vector3(0,0,600);
				character.shadow.transform.localScale = character.originalShadowScale * (1f - 0.5f*height/maxHeight); 
            }
		}else if(interact.interactType == InteractType.Fly){
			if(character.shadow != null){
                character.shadow.transform.position = scalePosition + new Vector3(0,0,600);
				character.shadow.transform.localScale = character.originalShadowScale * (1f - 0.5f*height/maxHeight); 
            }
		}

		if(interact.interactType ==InteractType.None && character.enviromentType == EnviromentType.Room)
		{
			scalePosition = this.transform.position;
			height = 0;
		}

        if(interact.interactType != InteractType.Toy)
        {
			dragScale = originalScale * (1 - scalePosition.y * scaleFactor);
			character.transform.localScale = dragScale;
		}

		character.agent.maxSpeed = 0.3f * character.data.speed * speedFactor *(1 - scalePosition.y * scaleFactor);

		lastPosition = this.transform.position;
	}
}
