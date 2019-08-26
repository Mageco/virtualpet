using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharScale : MonoBehaviour
{

	public float initZ = -20;
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
    void Update()
    {
		if (character.transform.position.y < initZ)
			character.transform.localScale = originalScale * (1 + (-character.transform.position.y + initZ) * scaleFactor);
		else
			character.transform.localScale = originalScale;
    }
}
