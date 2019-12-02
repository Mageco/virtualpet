using System.Collections;
using System.Collections.Generic;
using Mage.Models;
using UnityEngine;

[System.Serializable]
public class PlayerData : BaseModel
{
    public int coin;
    public int diamond;
    public List<PlayerItem> items = new List<PlayerItem>();
    public List<Pet> pets = new List<Pet>();

	public int Coin
	{
		get
		{
			return this.coin;
		}
		set
		{
			this.coin = value;
			if (this.coin < 0)
				this.coin = 0;
		}
	}

    public int Diamond
	{
		get
		{
			return this.diamond;
		}
		set
		{
			this.diamond = value;
			if (this.diamond < 0)
				this.diamond = 0;
		}
	}

}

[System.Serializable]
public class PlayerItem : BaseModel{
    public int itemId = 0;
    public ItemState state = ItemState.OnShop;
}
