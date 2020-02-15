using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScale : MonoBehaviour
{
	float scaleFactor = 0.05f;
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
		transform.localScale = originalScale * (1 + (-transform.position.y) * scaleFactor);
	}
}
