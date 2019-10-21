using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScale : MonoBehaviour
{

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	CharController character;
	CharInteract interact;
	Vector3 originalScale;
	Vector3 dragScale;
	Vector3 targetScale = Vector3.one;

	float offset = 0;

	void Awake()
	{
		character = this.GetComponent <CharController> ();
		originalScale = character.transform.localScale;
		interact = this.GetComponent<CharInteract>();
		targetScale = originalScale;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
		offset = initZ;
		if (interact.interactType == InteractType.Drop)
		{
			//return;
			targetScale = dragScale;
		}
		else if (interact.interactType == InteractType.Drag) {
			offset = initZ + 20;
			if (character.transform.position.y > 2)
			{
				if (character.transform.position.y < offset)
					targetScale = originalScale * (1 + (-character.transform.position.y + offset) * scaleFactor);
				else
					targetScale = originalScale;
			}else{
				targetScale = originalScale * (1 + (-2 + offset) * scaleFactor);
			}
			dragScale = targetScale;
		}else{
			if (character.transform.position.y < offset)
				targetScale = originalScale * (1 + (-character.transform.position.y + offset) * scaleFactor);
			else
				targetScale = originalScale;
		}

		//Debug.Log(targetScale);
		character.transform.localScale = Vector3.Lerp(targetScale,character.transform.localScale,Time.deltaTime *  3f);

    }
}
