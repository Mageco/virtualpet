using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class EatItem : BaseFloorItem
{
    [HideInInspector]
	public float foodAmount = 0;
    [HideInInspector]
	public float maxfoodAmount = 100;
	public SpriteRenderer image;
	public Sprite[] foodSprites;

	int foodId = 0;
	int lastId = -1;

	protected override void Start()
	{
		base.Start();
		maxfoodAmount = DataHolder.GetItem(this.itemID).value;	
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

	protected override void OnMouseDown()
	{

		if (IsPointerOverUIObject())
		{
			return;
		}

		clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		state = EquipmentState.Hold;
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
        if(foodAmount < maxfoodAmount - 1)
        {
            /*
			int price = (int)((maxfoodAmount - foodAmount)/20);

			if (GameManager.instance.GetCoin() < price)
			{
				MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
			}
			else
			{
                if(price > 0)
                {
					ItemManager.instance.SpawnCoinPaid(this.transform.position, -price);
					GameManager.instance.AddCoin(-price, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
				}

			}*/

			if (foodAmount < maxfoodAmount - 1)
				MageManager.instance.PlaySound3D("happy_collect_item_06", false, this.transform.position);

			foodAmount = maxfoodAmount - 1;
		}
	}

    public int GetPrice()
    {
		return (int)((maxfoodAmount - foodAmount) / 20);
	}


}
