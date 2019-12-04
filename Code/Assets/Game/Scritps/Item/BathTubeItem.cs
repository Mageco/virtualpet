﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathTubeItem : MonoBehaviour
{

	Animator anim;
	bool isSoap = false;

	void Awake()
	{
		anim = this.GetComponent <Animator> ();

	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnSoap()
	{
		if (!isSoap) {
			anim.Play ("Bath_Start", 0);
			isSoap = true;
		}
	}

	public void OnShower()
	{
		if (isSoap) {
			anim.Play ("Bath_End", 0);
			isSoap = false;
			if(GameManager.instance.GetPetObject(0).data.dirty >= 50)
			{
				GameManager.instance.GetPetObject(0).data.dirty -= 50;
				GameManager.instance.AddExp(5);
				GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Bath);
			}
			

		}
	}
}
