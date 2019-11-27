using Mage.Models;
using UnityEngine;

[System.Serializable]
public class Pet : BaseModel
{
	public int iD = 0;
	public string iconUrl = "";
	public LanguageItem[] languageItem = new LanguageItem[0];
	public int buyPrice = 0;
	public PriceType priceType = PriceType.Coin;
    public ItemState itemState = ItemState.OnShop;
	public string petSmall = "";
	public string petMiddle = "";
	public string petBig = "";
	public string petMiniGame1 = "";
	public int levelRequire = 0;
	public bool isAvailable = true;
	//Common Data
	public int level = 1;
	public int exp = 0;
	public int[] skills;
	public string petName = "";

	//Main Data
	public float speed = 20;
	public float weight;
	public float strength;
	public float intelligent = 10;



	//Attribute Data
	public float food = 50;
	public float water = 50;
	public float sleep = 60;
	public float energy = 50;
	public float health = 50;
	public float shit = 0;
	public float pee = 0;
	public float happy = 50;
	
	public float dirty = 50;
	public float itchi = 50;
	public float fear = 0;
	public float curious = 0;

	[HideInInspector]
	public float basicEnergyConsume = 0.05f;
	[HideInInspector]
	public float actionEnergyConsume = 0;
	[HideInInspector]
	public float healthConsume = 0.01f;
	[HideInInspector]
	public float staminaConsume = 0.001f;
	[HideInInspector]
	public float dirtyFactor = 0.01f;
	[HideInInspector]
	public float happyConsume = 0.01f;
	[HideInInspector]
	public float sleepConsume = 0.05f;


	[HideInInspector]
	public float maxFood = 100;
	[HideInInspector]
	public float maxWater = 100;
	[HideInInspector]
	public float maxShit = 100;
	[HideInInspector]
	public float maxPee = 100;
	[HideInInspector]
	public float maxSleep = 100;
	[HideInInspector]
	public float maxEnergy = 100;
	[HideInInspector]
	public float maxHealth = 100;
	[HideInInspector]
	public float maxHappy = 100;
	[HideInInspector]
	public float maxStamina = 100;
	[HideInInspector]
	public float maxDirty = 100;
    [HideInInspector]
    public float maxItchi = 100;
    [HideInInspector]
    public float maxFear = 100;
    [HideInInspector]
    public float maxCurious = 100;


    public CharController character;
    PolyNavAgent agent;

    public Pet()
	{
		iD = DataHolder.LastPetID() + 1;
        if(DataHolder.Languages() != null)
        {
            languageItem = new LanguageItem[DataHolder.Languages().GetDataCount()];
            for (int i = 0; i < languageItem.Length; i++)
            {
                languageItem[i] = new LanguageItem();
            }
            languageItem[0].Name = "New Pet";
        }

    }

    public Pet(int id)
    {
        Pet p = DataHolder.GetPet(id);
        iD = p.iD;        
        iconUrl = p.iconUrl;
        languageItem = new LanguageItem[p.languageItem.Length];
        for(int i = 0; i < p.languageItem.Length; i++)
        {
            if (p.languageItem[i] != null)
            {
                languageItem[i] = new LanguageItem();
                languageItem[i].Name = p.languageItem[i].Name;
                languageItem[i].Description = p.languageItem[i].Description;
            }
        }
        buyPrice = p.buyPrice;
        priceType = p.priceType;
        petSmall = p.petSmall;
        petMiddle = p.petMiddle;
        petBig = p.petBig;
		weight = p.weight;
		speed = p.speed;
		strength = p.strength;
		maxEnergy = p.maxEnergy;
		intelligent = p.intelligent;
        skills = new int[DataHolder.Skills().GetDataCount()];
    }

    public CharController Load(){

        if (character != null)
            GameObject.Destroy(character.gameObject);


        string url = "";

		if(GameManager.instance.gameType == GameType.Minigame1){
			url = DataHolder.GetPet(iD).petMiniGame1.Replace("Assets/Game/Resources/", "");
		}
        else if (level >= 10)
        {
            url = DataHolder.GetPet(iD).petBig.Replace("Assets/Game/Resources/", "");
        }
        else if (level >= 2 )
        {
            url = DataHolder.GetPet(iD).petMiddle.Replace("Assets/Game/Resources/", "");
        }
        else
        {
            url = DataHolder.GetPet(iD).petSmall.Replace("Assets/Game/Resources/", "");
        }

        url = url.Replace(".prefab", "");
        url = DataHolder.Pets().GetPrefabPath() + url;
        GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
        character = go.GetComponent<CharController>();
		go.transform.parent = GameManager.instance.transform;      
        character.data = this;
		character.LoadPrefab();
		GameManager.instance.UpdatePetObjects();
        return character;
    }

	public void AddLanguageItem()
	{
		this.languageItem = ArrayHelper.Add(new LanguageItem(), this.languageItem);
	}

	public void RemoveLanguage(int index)
	{
		this.languageItem = ArrayHelper.Remove (index, this.languageItem);
	}

	public string GetDescription(int languageID)
	{
		return languageItem[languageID].Description;  
	}

	public void SetDescription(int languageID,string text)
	{
		if (this.languageItem.Length > languageID) {
			languageItem [languageID].Description = text;
		} else {
			LanguageItem item = new LanguageItem ();
			item.Description = text;
			this.languageItem = ArrayHelper.Add (item, this.languageItem);
		}
	}

	public string GetName(int languageID)
	{
		return languageItem[languageID].Name;  
	}

	public void SetName(int languageID,string text)
	{
		if (this.languageItem.Length > languageID) {
			languageItem [languageID].Name = text;
		} else {
			LanguageItem item = new LanguageItem ();
			item.Name = text;
			this.languageItem = ArrayHelper.Add (item, this.languageItem);
		}
	}

	void Save()
	{

	}

	public float Food
	{
		get
		{
			return this.food;
		}
		set
		{
			this.food = value;
			if (this.food < 0)
				this.food = 0;
			else if (this.food > maxFood)
				this.food = maxFood;
		}
	}

	public float Water
	{
		get
		{
			return this.water;
		}
		set
		{
			this.water = value;
			if (this.water < 0)
				this.water = 0;
			else if (this.water > maxWater)
				this.water = maxWater;
		}
	}

	public float Sleep
	{
		get
		{
			return this.sleep;
		}
		set
		{
			this.sleep = value;
			if (this.sleep < 0)
				this.sleep = 0;
			else if (this.sleep > maxSleep)
				this.sleep = maxSleep;
		}
	}

	public float Energy
	{
		get
		{
			return this.energy;
		}
		set
		{
			this.energy = value;
			if (this.energy < 0)
				this.energy = 0;
			else if (this.energy > maxEnergy)
				this.energy = maxEnergy;
		}
	}

	public float Health
	{
		get
		{
			return this.health;
		}
		set
		{
			this.health = value;
			if (this.health < 0)
				this.health = 0;
			else if (this.health > maxHealth)
				this.health = maxHealth;
		}
	}

	public float Shit
	{
		get
		{
			return this.shit;
		}
		set
		{
			this.shit = value;
			if (this.shit < 0)
				this.shit = 0;
			else if (this.shit > maxShit)
				this.shit = maxShit;
		}
	}

	public float Pee
	{
		get
		{
			return this.pee;
		}
		set
		{
			this.pee = value;
			if (this.pee < 0)
				this.pee = 0;
			else if (this.pee > maxPee)
				this.pee = maxPee;
		}
	}

	public float Happy
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
			else if (this.happy > maxHappy)
				this.happy = maxHappy;
		}
	}

	public float Dirty
	{
		get
		{
			return this.dirty;
		}
		set
		{
			this.dirty = value;
			if (this.dirty < 0)
				this.dirty = 0;
			else if (this.dirty > maxDirty)
				this.dirty = maxDirty;
		}
	}

	public float Itchi
	{
		get
		{
			return this.itchi;
		}
		set
		{
			this.itchi = value;
			if (this.itchi < 0)
				this.itchi = 0;
			else if (this.itchi > maxItchi)
				this.itchi = maxItchi;
		}
	}

	public float Fear
	{
		get
		{
			return this.fear;
		}
		set
		{
			this.fear = value;
			if (this.fear < 0)
				this.fear = 0;
			else if (this.fear > maxFear)
				this.fear = maxFear;
		}
	}

	public float Curious
	{
		get
		{
			return this.curious;
		}
		set
		{
			this.curious = value;
			if (this.curious < 0)
				this.curious = 0;
			else if (this.curious > maxCurious)
				this.curious = maxCurious;
		}
	}

	public int Exp
	{
		get
		{
			return this.exp;
		}
		set
		{
			this.exp = value;
			int temp = level;
			level = 1;
			float e = 10 * level + 2 * level * level;
			while(exp > e)
			{
				e = 10 * level + 2 * level * level;
				level ++;
			}
			if(level > temp){
				if(character != null){
					character.OnLevelUp();
				}
				//if(temp < 5)
				//	Load();	
			}
		}	
	}

	public int GetSkillProgress(SkillType type){
		for(int i=0;i<skills.Length;i++){
			if(DataHolder.Skill(i).skillType == type){
				return skills[i];
			}
		}
		return 0;
	}

	public void LevelUpSkill(SkillType type){
		for(int i=0;i<skills.Length;i++){
			if(DataHolder.Skill(i).skillType == type){
				skills[i] ++;
				if(skills[i] > DataHolder.Skill(i).maxProgress)
					skills[i] = DataHolder.Skill(i).maxProgress;
				Debug.Log("Skill Progress " + type.ToString() + " " + skills[i]);				
				return;
			}
		}
	}

	public bool SkillLearned(SkillType type){
		for(int i=0;i<skills.Length;i++){
			if(DataHolder.Skill(i).skillType == type){
				if(skills[i] >= DataHolder.Skill(i).maxProgress)
					return true;
				else 
					return false;
			}
		}
		return false;
	}





}
