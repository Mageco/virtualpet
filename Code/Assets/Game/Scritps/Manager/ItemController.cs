using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{

	public static ItemController instance;
	[HideInInspector]
	public CharController character;
	public FoodBowlItem foodBowl;
	public FoodBowlItem waterBowl;
	public SoapItem soapItem;
	public BathShowerItem showerItem;
	public BathTubeItem bathTubeItem;

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
}
