using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDirty : MonoBehaviour
{

	public float maxDirty = 100;
	float dirty = 0;
	public float clean = 20;
	Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
		dirty = maxDirty;
    }

    // Update is called once per frame
    void Update()
    {
		this.transform.localScale = dirty / maxDirty * originalScale;
    }

	public void OnClean()
	{
		dirty -= clean;
		if (dirty < 0)
			GameObject.Destroy (this.gameObject);
	}
}
