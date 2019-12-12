using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class EatItem : BaseFloorItem
{
	public float foodAmount = 0;
	public float maxfoodAmount = 100;
	public SpriteRenderer image;
	public Sprite[] foodSprites;

	float time;
	float maxDoubleClickTime = 0.4f;
	bool isClick = false;

	public Transform anchor;

	protected override void Start(){
		base.Start();
		maxfoodAmount = DataHolder.GetItem(item.itemID).value;
	}


    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
		int id = (int)(foodAmount/(maxfoodAmount/foodSprites.Length));
		image.sprite = foodSprites [id];
    }

	public void Eat(float f)
	{
		foodAmount -= f;
		if (foodAmount < 0){
			foodAmount = 0;
		}
			
	}

	public bool CanEat()
	{
		if (foodAmount <= 0)
			return false;
		else
			return true;
	}

	protected override void OnMouseUp()
	{

		base.OnMouseUp();

		if (isClick) {
			if (time > maxDoubleClickTime) {
				time = 0;
			} else {
				Fill ();
				time = 0;
				isClick = false;
				return;
			}
		} else {
			time = 0;
			isClick = true;
		}
	}

	protected override void OnCollidePet(CharController pet){
		pet.OnEat();
	}

	void Fill()
	{
		foodAmount = maxfoodAmount-1;
	}
}
