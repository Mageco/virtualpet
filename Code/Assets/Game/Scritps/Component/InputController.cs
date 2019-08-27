using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class InputController : MonoBehaviour
{
	public static InputController instance;
	public Transform target;
	public LeanScreenDepth ScreenDepth;
	public Transform[] points;
	CharController character;

	void Awake()
	{
		if (instance == null)
			instance = this;

		character = GameObject.FindObjectOfType<CharController> ();
	}


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

		if (character.interactType == InteractType.None && character.enviromentType == EnviromentType.Room) {
			character.OnMove ();
		}
	}

	public void OnCall()
	{
		target.transform.position = points [0].position;
		character.OnCall ();
	}
}
