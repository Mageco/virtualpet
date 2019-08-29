using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestroy : MonoBehaviour
{

	public float duration;
	float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (time > duration)
			GameObject.Destroy (this.gameObject);
		else
			time += Time.deltaTime;
    }
}
