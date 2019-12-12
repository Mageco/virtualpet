﻿using System.Collections;
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
	public int[] minigameLevels = new int[1];
	public List<PlayerAchivement> achivements = new List<PlayerAchivement>();
	
	public PlayerData(){


	}

	public void LoadData(){
		for(int i=0;i<DataHolder.Achivements().GetDataCount();i++){
			PlayerAchivement a = new PlayerAchivement();
			a.achivementId = DataHolder.Achivement(i).iD;
			a.rewardState = RewardState.None;
			a.order = DataHolder.Achivement(i).order;
			achivements.Add(a);
		}
	}

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

[System.Serializable]
public class PlayerAchivement : BaseModel{
    public int achivementId = 0;
	public int level = 0;
	public int amount = 0;

	public int order = 0;
    public RewardState rewardState = RewardState.None;

	public int Amount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
			if (this.amount >= DataHolder.GetAchivement(achivementId).maxProgress[level]){
				if(rewardState != RewardState.Ready)
					rewardState = RewardState.Ready;
			}
				
			
		}
	}

	public void Check(){
		if (this.amount >= DataHolder.GetAchivement(achivementId).maxProgress[level]){
			if(rewardState != RewardState.Ready)
				rewardState = RewardState.Ready;
		}
	}
}


