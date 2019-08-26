using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class InputController : MonoBehaviour
{
	public static InputController instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public Transform target;
	public LeanScreenDepth ScreenDepth;
	public Transform[] points;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetTarget(LeanFinger finger)
	{
		var worldPoint = ScreenDepth.Convert (finger.ScreenPosition, gameObject);
		worldPoint.z = target.position.z;
		target.position = worldPoint;

	}

	public void OnCall()
	{
		target.transform.position = points [0].position;
	}
}
