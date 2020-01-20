using System.Collections;
using System.Collections.Generic;
using Mage.Models;
using UnityEngine;

[System.Serializable]
public class PlayerData : BaseModel
{
    public int coin;
    public int diamond;
	public int happy;

    public int collectedHappy = 0;
    public int collectedCoin = 0;
    public int petCount = 1;
    public int itemCount = 0;
    public float playTime = 0;
    public List<PlayerItem> items = new List<PlayerItem>();
    public List<Pet> pets = new List<Pet>();
	public int[] minigameLevels = new int[20];

	public int questId = 0;
    public int questValue = 0;
	public List<PlayerAchivement> achivements = new List<PlayerAchivement>();
    public GameType gameType = GameType.House;

    public PlayerData(){
        for(int i = 0; i < minigameLevels.Length; i++)
        {
			minigameLevels[i] = 0;
        }
	}

	public void LoadData(){
		for(int i=0;i<DataHolder.Achivements().GetDataCount();i++){
			PlayerAchivement a = new PlayerAchivement();
			a.achivementId = DataHolder.Achivement(i).iD;
			a.rewardState = RewardState.None;
			a.achivementType = DataHolder.Achivement(i).achivementType;
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

	
    public int Happy
	{
		get
		{
			return this.happy;
		}
		set
		{
			this.happy = value;
			if (this.happy < 0)
				this.happy = 0;
		}
	}

}

[System.Serializable]
public class PlayerItem : BaseModel{
    public int itemId = 0;
    public ItemState state = ItemState.OnShop;
	public Vector3 position = Vector3.zero;
	public float value = 0;
	public int number = 1;
	public bool isConsumable = false;
}

[System.Serializable]
public class PlayerAchivement : BaseModel{
    public int achivementId = 0;
	public int level = 0;
	public int amount = 0;

	

	public int order = 0;
    public RewardState rewardState = RewardState.None;
	public AchivementType achivementType;

	public int Amount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
			if (DataHolder.GetAchivement(achivementId).maxProgress.Length > level && this.amount >= DataHolder.GetAchivement(achivementId).maxProgress[level]){
				if(rewardState != RewardState.Ready)
					rewardState = RewardState.Ready;
			}
				
			
		}
	}

	public void Check(){
		if (DataHolder.GetAchivement(achivementId).maxProgress.Length > level && this.amount >= DataHolder.GetAchivement(achivementId).maxProgress[level]){
			if(rewardState != RewardState.Ready)
				rewardState = RewardState.Ready;
		}
	}
}


