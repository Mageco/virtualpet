using System.Collections;
using System.Collections.Generic;
using Mage.Models;
using UnityEngine;

[System.Serializable]
public class PlayerData : BaseModel
{
	public string playerName = "House's ...";
    public int coin = 0;
	
    public int diamond = 0;
	public int happy = 0;
	public int level = 1;
	[ExtractField]
	public int collectedHappy = 0;
    public int collectedCoin = 0;
	[ExtractField]
	public int petCount = 1;
	[ExtractField]
	public int itemCount = 0;
    public float playTime = 0;
	[ExtractField]
	public int exp = 0;
    public List<PlayerItem> items = new List<PlayerItem>();
    public List<Pet> pets = new List<Pet>();
	[ExtractField]
	public int[] minigameLevels = new int[20];

	public int questId = 0;
    public int questValue = 0;
	public List<Skin> petColors = new List<Skin>();
	public List<PlayerAchivement> achivements = new List<PlayerAchivement>();
	public List<ItemSaveData> itemSaveDatas = new List<ItemSaveData>();
	public List<PlayerService> playerServices = new List<PlayerService>();
	public string startGameTime = System.DateTime.Now.ToString();
	public List<PlayerBonus> dailyBonus = new List<PlayerBonus>();

	public PlayerData(){
        for(int i = 0; i < minigameLevels.Length; i++)
        {
			minigameLevels[i] = 0;
        }

		startGameTime = System.DateTime.Now.ToString();
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
	public ItemType itemType = ItemType.All;
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

[System.Serializable]
public class PlayerService : BaseModel
{
	public ServiceType type;
	public string timeStart;
	public bool isActive = false;
	public float duration = 1800;

	public void StartService()
	{
		isActive = true;
		timeStart = System.DateTime.Now.ToString();
	}

	public void StopService()
	{
		isActive = false;
    }

    public string GetTime()
    {
		double t = (System.DateTime.Parse(timeStart).AddSeconds(duration) - System.DateTime.Now).TotalSeconds;
		int m = (int)t / 60;
		int s = (int)(t - m * 60);
		string time = m.ToString("00") + ":" + s.ToString("00");
		return time;
    }
}

[System.Serializable]
public class PlayerBonus : BaseModel
{
	public string timeReceived = "";
	public bool isCollected = false;

	public void Collect()
	{
		isCollected = true;
		timeReceived = System.DateTime.Now.ToString();
	}
}


