using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScale : MonoBehaviour
{
	public float initZ = -6;
	public float scaleFactor = 0.1f;
	ItemObject item;
	Vector3 originalScale;

	void Awake()
	{
		item = this.GetComponent <ItemObject> ();
		originalScale = this.transform.localScale;
	}
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void LateUpdate()
	{

		float offset = initZ;


		if (transform.position.y < offset)
			transform.localScale = originalScale * (1 + (-transform.position.y + offset) * scaleFactor);
		else
			transform.localScale = originalScale;
	}
}
