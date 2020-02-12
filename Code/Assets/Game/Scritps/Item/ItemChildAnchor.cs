using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChildAnchor : MonoBehaviour
{

    public GameObject[] childs;
    float[] offsets;
	void Awake()
	{
		offsets = new float[childs.Length];
		for (int i = 0; i < childs.Length; i++)
		{
			offsets[i] = childs[i].transform.position.x - this.transform.position.x;
		}
	}
	// Start is called before the first frame update


	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < childs.Length; i++)
		{
			Vector3 pos = childs[i].transform.position;
			pos.x = offsets[i] + this.transform.position.x;
			childs[i].transform.position = pos;
		}
	}
	
}
