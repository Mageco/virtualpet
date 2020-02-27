using Mage.Models;
using UnityEngine;
using PolyNav;
using System.Collections.Generic;

[System.Serializable]
public class Pet : BaseModel
{
    public int iD = 0;
    public string iconUrl = "";
    public LanguageItem[] languageItem = new LanguageItem[0];
    public int buyPrice = 0;
    public PriceType priceType = PriceType.Coin;
    public ItemState itemState = ItemState.OnShop;
    public string prefabName = "";
    public bool isAvailable = true;
    public int[] requireEquipments = new int[0];
    public int[] requirePets = new int[0];
    //Common Data
    public List<PetSkill> skills = new List<PetSkill>();
    public string petName = "";
    public int shopOrder = 0;
    //Main Data
    public float speed = 20;
    public int level = 1;
    public int exp = 0;
    public float intelligent = 0;
    public int happy = 0;
    //Attribute Data
    public float food = 50;
    public float water = 50;
    public float sleep = 60;
    public float energy = 50;
    public float health = 100;
    public float damage = 0;
    public float shit = 0;
    public float pee = 0;
    public float dirty = 10;
    public float itchi = 10;
    public float fear = 0;
    public float curious = 70;
    public System.DateTime timeSick;
    public float maxTimeSick = 3600;
    public bool isNew = false;


    [HideInInspector]
    public float rateFood = 10f;
    [HideInInspector]
    public float rateWater = 10f;
    [HideInInspector]
    public float rateSleep = 0.5f;
    [HideInInspector]
    public float recoverSleep = 0.1f;
    [HideInInspector]
    public float ratePee = 10f;
    [HideInInspector]
    public float rateShit = 10f;
    [HideInInspector]
    public float recoverDirty = 0.1f;
    [HideInInspector]
    public float recoverHealth = 0;
    [HideInInspector]
    public float recoverEnergy = 5;



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
    public float maxDamage = 100;
    [HideInInspector]
    public float maxDirty = 100;
    [HideInInspector]
    public float maxItchi = 100;
    [HideInInspector]
    public float maxCurious = 100;

    int levelRate = 10;


    public CharController character;
    PolyNavAgent agent;

    public ActionType actionType = ActionType.None;
    public Vector3 position = Vector3.zero;
    public Vector3 scalePosition = Vector3.zero;
    public float height = 0;
    public EnviromentType enviromentType = EnviromentType.Room;

    public Pet()
    {
        iD = DataHolder.LastPetID() + 1;
        if (DataHolder.Languages() != null)
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
        petName = p.GetName(MageManager.instance.GetLanguage());
        languageItem = new LanguageItem[p.languageItem.Length];
        for (int i = 0; i < p.languageItem.Length; i++)
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
        prefabName = p.prefabName;
        speed = p.speed;
        maxEnergy = p.maxHealth;
        maxHealth = p.maxHealth;
        maxFood = p.maxFood;
        maxWater = p.maxWater;
        maxPee = p.maxPee;
        maxShit = p.maxShit;
        maxSleep = p.maxSleep;
        maxDirty = p.maxDirty;
        rateFood = p.rateFood;
        rateWater = p.rateWater;
        rateSleep = p.rateSleep;
        ratePee = p.ratePee;
        rateShit = p.rateShit;

        recoverEnergy = p.recoverEnergy;
        recoverSleep = p.recoverSleep;
        recoverHealth = p.recoverHealth;

        food = Random.Range(maxFood / 2, maxFood);
        water = Random.Range(maxWater / 2, maxWater);
        sleep = Random.Range(maxSleep / 2, maxSleep);
        energy = Random.Range(maxEnergy / 2, maxEnergy);
        health = maxHealth;


        LoadSkill();
    }

    void LoadSkill()
    {
        for (int i = 0; i < DataHolder.Skills().GetDataCount(); i++)
        {
            PetSkill s = new PetSkill();
            s.skillId = DataHolder.Skill(i).iD;
            s.rewardState = RewardState.None;
            skills.Add(s);
        }
    }

    public void AddLanguageItem()
    {
        this.languageItem = ArrayHelper.Add(new LanguageItem(), this.languageItem);
    }

    public void RemoveLanguage(int index)
    {
        this.languageItem = ArrayHelper.Remove(index, this.languageItem);
    }

    public string GetDescription(int languageID)
    {
        return languageItem[languageID].Description;
    }

    public void SetDescription(int languageID, string text)
    {
        if (this.languageItem.Length > languageID)
        {
            languageItem[languageID].Description = text;
        }
        else
        {
            LanguageItem item = new LanguageItem();
            item.Description = text;
            this.languageItem = ArrayHelper.Add(item, this.languageItem);
        }
    }

    public string GetName(int languageID)
    {
        return languageItem[languageID].Name;
    }

    public void SetName(int languageID, string text)
    {
        if (this.languageItem.Length > languageID)
        {
            languageItem[languageID].Name = text;
        }
        else
        {
            LanguageItem item = new LanguageItem();
            item.Name = text;
            this.languageItem = ArrayHelper.Add(item, this.languageItem);
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
            else if (this.food > MaxFood)
                this.food = MaxFood;
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
            else if (this.water > MaxWater)
                this.water = MaxWater;
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
            else if (this.sleep > MaxSleep)
                this.sleep = MaxSleep;
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
            else if (this.energy > MaxEnergy)
                this.energy = MaxEnergy;
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
            else if (this.health > MaxHealth)
                this.health = MaxHealth;
        }
    }

    public float Damage
    {
        get
        {
            return this.damage;
        }
        set
        {
            this.damage = value;
            if (this.damage < 0)
                this.damage = 0;
            else if (this.damage > MaxDamage)
                this.damage = MaxDamage;
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
            else if (this.shit > MaxShit)
                this.shit = MaxShit;
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
            else if (this.pee > MaxPee)
                this.pee = MaxPee;
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
            else if (this.dirty > MaxDirty)
                this.dirty = MaxDirty;
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
            else if (this.itchi > MaxItchi)
                this.itchi = MaxItchi;
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

    public float Intelligent
    {
        get
        {
            float s = intelligent;
            for (int i = 0; i < skills.Count; i++)
            {
                s += skills[i].level;
            }
            return s + level / 2;
        }
        set
        {
            this.intelligent = value;
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
            float e = 10 * level + 10 * level * level;
            while (exp > e)
            {
                level++;
                e = 10 * level + 10 * level * level;
            }
            if (level > temp)
            {
                if (character != null)
                {
                    character.OnLevelUp();
                }
            }
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            this.speed = value;
        }
    }

    public float MaxFood
    {
        get
        {
            return maxFood + level * levelRate;
        }
        set
        {
            this.maxFood = value;
        }
    }

    public float MaxWater
    {
        get
        {
            return maxWater + level * levelRate;
        }
        set
        {
            this.maxWater = value;
        }
    }

    public float MaxShit
    {
        get
        {
            return maxShit + level * levelRate;
        }
        set
        {
            this.maxShit = value;
        }
    }



    public float MaxPee
    {
        get
        {
            return maxPee + level * levelRate;
        }
        set
        {
            this.maxPee = value;
        }
    }

    public float MaxSleep
    {
        get
        {
            return maxSleep + level * levelRate;
        }
        set
        {
            this.maxSleep = value;
        }
    }

    public float MaxEnergy
    {
        get
        {
            return maxEnergy + level * levelRate;
        }
        set
        {
            this.maxEnergy = value;
        }
    }

    public float MaxHealth
    {
        get
        {
            return maxHealth + level * levelRate;
        }
        set
        {
            this.maxHealth = value;
        }
    }

    public float MaxDamage
    {
        get
        {
            return maxDamage + level * levelRate;
        }
        set
        {
            this.maxDamage = value;
        }
    }

    public float MaxDirty
    {
        get
        {
            return maxDirty + level * levelRate;
        }
        set
        {
            this.maxDirty = value;
        }
    }

    public float MaxItchi
    {
        get
        {
            return maxItchi + level * levelRate;
        }
        set
        {
            this.maxItchi = value;
        }
    }



    public float MaxCurious
    {
        get
        {
            return maxCurious;
        }
        set
        {
            this.maxCurious = value;
        }
    }

    public float MaxTimeSick
    {
        get
        {
            //return this.maxTimeSick;
            return Mathf.Min(600 * level, 86400);
        }
        set
        {
            this.maxTimeSick = value;
        }
    }

    public float RecoveryEnergy
    {
        get
        {
            return recoverEnergy + level;
        }
        set
        {
            this.recoverEnergy = value;
        }
    }

    public float RecoverHealth
    {
        get
        {
            return recoverHealth + level / 25;
        }
        set
        {
            this.recoverHealth = value;
        }
    }


    public int GetSkillProgress(SkillType type)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (DataHolder.Skills().GetSkill(skills[i].skillId).skillType == type)
            {
                return skills[i].level;
            }
        }
        return 0;
    }

    public void LevelUpSkill(SkillType type)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (DataHolder.Skills().GetSkill(skills[i].skillId).skillType == type)
            {
                skills[i].level++;
                if (skills[i].level >= DataHolder.Skills().GetSkill(skills[i].skillId).maxProgress)
                {
                    skills[i].rewardState = RewardState.Ready;
                    skills[i].level = DataHolder.Skills().GetSkill(skills[i].skillId).maxProgress;
                }

                return;
            }
        }
    }

    public bool SkillLearned(SkillType type)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (DataHolder.Skills().GetSkill(skills[i].skillId).skillType == type)
            {
                if (skills[i].level >= DataHolder.Skills().GetSkill(skills[i].skillId).maxProgress)
                    return true;
                else
                    return false;
            }
        }
        return false;
    }
}

[System.Serializable]
public class PetSkill : BaseModel
{
    public int skillId = 0;
    public int level = 0;
    public int order = 0;
    public RewardState rewardState = RewardState.None;
}
