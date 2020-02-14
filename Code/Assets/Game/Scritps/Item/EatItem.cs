using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class EatItem : BaseFloorItem
{
	public ItemSaveDataType itemSaveDataType = ItemSaveDataType.None;
	public float foodAmount = 0;
	public float maxfoodAmount = 100;
	public SpriteRenderer image;
	public Sprite[] foodSprites;
	public Transform anchor;


	int foodId = 0;
	int lastId = -1;

	protected override void Start()
	{
		base.Start();
		maxfoodAmount = DataHolder.GetItem(item.itemID).value;	
	}


	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		foodId = (int)(foodAmount / (maxfoodAmount / foodSprites.Length));
        if(lastId != foodId)
        {
			image.sprite = foodSprites[foodId];
			lastId = foodId;
		}
	}

	protected override void OnClick()
	{
		Fill();
		state = EquipmentState.Idle;
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


	public void Fill()
	{
		if(foodAmount < maxfoodAmount-1)
			MageManager.instance.PlaySoundName("happy_collect_item_06",false);
		
		foodAmount = maxfoodAmount-1;
	}


}
