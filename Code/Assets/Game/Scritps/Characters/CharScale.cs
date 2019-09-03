using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScale : MonoBehaviour
{

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	CharController character;
	Vector3 originalScale;

	void Awake()
	{
		character = this.GetComponent <CharController> ();
		originalScale = character.transform.localScale;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
		if (character.interactType == InteractType.Drop)
			return;

		float offset = initZ;
		if (character.interactType == InteractType.Drag) {
			if (character.transform.position.y > 2)
				offset = initZ + 22;
			else
				return;
		}


		if (character.transform.position.y < offset)
			character.transform.localScale = originalScale * (1 + (-character.transform.position.y + offset) * scaleFactor);
		else
			character.transform.localScale = originalScale;
    }
}
