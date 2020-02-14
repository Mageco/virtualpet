﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemCollider : BaseFloorItem
{
    public float height = 5;
    public float depth = 2;
    public float width = 4;
    public float edge = 3;

	public Transform anchorPoint;
	public Transform startPoint;
	public Transform endPoint;

	Animator[] animParts;

	protected override void Awake()
	{
		base.Awake();
		animParts = this.GetComponentsInChildren<Animator>(true);
	}



	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		List<CharController> temp = new List<CharController>();
        if(pets.Count > 0)
        {
            foreach(CharController pet in pets)
            {
                if(pet != null && pet.enviromentType == EnviromentType.Room)
                {
					temp.Add(pet);
                }
            }
        }

		foreach (CharController pet in temp)
		{
			pets.Remove(pet);
		}
	}

	protected override void LateUpdate()
	{


	}

	protected override void OnDrag()
	{


		if (animator != null)
			animator.enabled = false;
        for(int i = 0; i < animParts.Length; i++)
        {
			animParts[i].enabled = false;
        }
		base.OnDrag();
	}

	protected override void OffDrag()
	{
		for (int i = 0; i < animParts.Length; i++)
		{
			animParts[i].enabled = true;
		}
        if(animator != null)
		    animator.enabled = true;

		base.OffDrag();
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		roomCollide = other.gameObject;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (roomCollide == other.gameObject)
		{
			roomCollide = null;
		}
	}
}

