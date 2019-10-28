using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalScale : MonoBehaviour
{

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	MouseController character;
	Vector3 originalScale;

	void Awake()
	{
		character = this.GetComponent <MouseController> ();
		originalScale = character.transform.localScale;
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
		float offset = initZ;

		if (character.transform.position.y < offset)
			character.transform.localScale = originalScale * (1 + (-character.transform.position.y + offset) * scaleFactor);
		else
			character.transform.localScale = originalScale;
    }
}
