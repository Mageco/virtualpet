using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
	public static EnviromentController instance;

	void Awake()
	{
		if (instance == null)
			instance = this;

		Load ();
	}

	void Load()
	{
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnFall(Vector3 pos){
	}

	public void OffFall()
	{
	}

}
