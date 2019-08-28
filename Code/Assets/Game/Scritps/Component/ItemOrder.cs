using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOrder : MonoBehaviour
{

	ItemObject item;
	Vector3 originalPosition;
	public bool isOrder = true;

	void Awake()
	{
		item = this.GetComponent <ItemObject> ();
		originalPosition = this.transform.position;
	}



    // Update is called once per frame
    void LateUpdate()
    {
		if (isOrder) {
			Vector3 pos = this.transform.position;
			pos.z = this.transform.position.y;
			this.transform.position = pos;
		}
    }
}
