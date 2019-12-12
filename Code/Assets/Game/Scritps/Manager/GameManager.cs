﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    
 	public static GameManager instance;
    public float gameTime = 0;
    public List<CharController> petObjects = new List<CharController>();
    CameraController camera;
    public int questId = 0;
    public GameType gameType = GameType.House;

    //Testing
    public int addExp = 0;
    public bool isLoad = false;
    
    public GameObject expPrefab;

    public bool isTest = false;

    public PlayerData myPlayer = new PlayerData();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            GameObject.Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        Application.targetFrameRate = 50;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		DataHolder.Instance();
		DataHolder.Instance().Init();    
        camera = Camera.main.GetComponent<CameraController>();
    }


    private void Start() {
        LoadPlayer();
        isLoad = true;
        LoadPetObjects();
    }

    void Update(){
		gameTime += Time.deltaTime;
	}

    public PlayerData GetPlayer(){
        return myPlayer;
    }

    public void LoadPetObjects()
    {
        for (int i = 0; i < myPlayer.pets.Count; i++)
        {
            if(myPlayer.pets[i].itemState == ItemState.Equiped){
                CharController c = myPlayer.pets[i].Load();
                petObjects.Add(c);
            }
        }
        UIManager.instance.LoadProfiles();
    }

    void AddPet(int itemId)
    {
        Pet p = new Pet(itemId);
        p.itemState = ItemState.Have;
        p.Exp +=addExp;
        myPlayer.pets.Add(p);
        SavePlayer();
    }

    public void EquipPet(int itemId)
    {
        foreach(Pet p in myPlayer.pets){
            if(p.iD == itemId){
                p.itemState = ItemState.Equiped;
            }
        }
        LoadPetObjects();
    }

    public void EquipPets()
    {
        foreach(Pet p in myPlayer.pets){
            p.itemState = ItemState.Equiped;
        }
        LoadPetObjects();
    }

    public void UnEquipPet(int itemId)
    {
        foreach(Pet p in myPlayer.pets){
            if(p.iD == itemId){
                p.itemState = ItemState.Have;
                petObjects.Remove(p.character);
                p.UnLoad();
            }
        }
    }

    public void UnEquipPets()
    {
        foreach(Pet p in myPlayer.pets){
            p.itemState = ItemState.Have;
            p.UnLoad();
        }
        UpdatePetObjects();
    }



    public void UpdatePetObjects(){
        petObjects.Clear();
        for(int i=0;i<myPlayer.pets.Count;i++){
            if(myPlayer.pets[i].itemState == ItemState.Equiped && myPlayer.pets[i].character != null)
                petObjects.Add(myPlayer.pets[i].character);
        }
    }

    public bool BuyPet(int petId)
	{
        Debug.Log("Buy pet " + petId);
		PriceType type = DataHolder.GetPet(petId).priceType;
		int price = DataHolder.GetPet(petId).buyPrice;
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
                MageManager.instance.OnNotificationPopup ("You have not enough Coin");
				return false;
			}
			AddCoin (-price);
			AddPet (petId);
            Debug.Log("Buy pet " + petId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
                MageManager.instance.OnNotificationPopup ("You have not enough Diamond");
				return false;
			}
			AddDiamond (-price);
			AddPet (petId);
			return true;
		}else
		{
			return false;
		}
	}

    public void SellPet(int petId)
	{
		PriceType type = DataHolder.GetPet(petId).priceType;
		int price = DataHolder.GetPet(petId).buyPrice/2;
		if(type == PriceType.Coin){
			AddCoin (price);
		}else if(type == PriceType.Diamond){
			AddDiamond (price);
		}
		RemovePet (petId);
	}

    public Pet GetPet(int id){
        foreach(Pet p in myPlayer.pets){
            if(p.iD == id)
                return p;
        }
        return null;
    }

    public List<Pet> GetPets(){
        return myPlayer.pets;
    }

    public List<CharController> GetPetObjects(){
        return petObjects;
    }

    public CharController GetPetObject(int id){
        foreach(Pet p in myPlayer.pets){
            if(p.iD == id)
                return p.character;
        }
        return null;
    }
 
    void RemovePet(int id){
        foreach(Pet p in myPlayer.pets){
            if(p.iD == id){
                myPlayer.pets.Remove(p);
                UpdatePetObjects();
                p.UnLoad();
                return;
            }
        }     
    }

    public Pet GetActivePet(){
        return myPlayer.pets[0];
    }


    public bool IsHavePet(int petId)
	{
        foreach(Pet p in myPlayer.pets){
            if(p.iD == petId){
                return true;
            }
        }
        return false;
	}

	public bool IsEquipPet(int petId)
	{
        foreach(Pet p in myPlayer.pets){
            if(p.iD == petId && p.itemState == ItemState.Equiped){
                return true;
            }
        }
        return false;
	}

	public List<int> GetBuyPets(){
		List<int> pets = new List<int>();
		for(int i=0;i<DataHolder.Pets().GetDataCount();i++){
			if(IsHavePet(DataHolder.Pet(i).iD)){
				pets.Add(DataHolder.Pet(i).iD);
			}
		}
		return pets;
	}

	public List<int> GetEquipedPets(){
		List<int> pets = new List<int>();
		for(int i=0;i<DataHolder.Pets().GetDataCount();i++){
			if(IsEquipPet(DataHolder.Pet(i).iD)){
				pets.Add(DataHolder.Pet(i).iD);
			}
		}
		return pets;
	}

    public bool BuyItem(int itemId)
	{
		PriceType type = DataHolder.GetItem(itemId).priceType;
		int price = DataHolder.GetItem(itemId).buyPrice;
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
				MageManager.instance.OnNotificationPopup ("You have not enough Coin");
				return false;
			}
			AddCoin (-price);
			AddItem (itemId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
				MageManager.instance.OnNotificationPopup ("You have not enough Coin");
				return false;
			}
			AddDiamond (-price);
			AddItem (itemId);
			return true;
		}else
		{
			return false;
		}
	}

    public void SellItem(int itemId){
        PriceType type = DataHolder.GetItem(itemId).priceType;
		int price = DataHolder.GetItem(itemId).buyPrice;
		if(type == PriceType.Coin){
			AddCoin (price);
		}else if(type == PriceType.Diamond){
			AddDiamond (price);
		}
        RemoveItem(itemId);
    }

    public void AddItem(int id){
        PlayerItem item = new PlayerItem();
        item.itemId = id;
        item.state = ItemState.OnShop;
        myPlayer.items.Add(item);
        SavePlayer();
    }

    public void RemoveItem(int itemId)
	{
		foreach(PlayerItem item in myPlayer.items){
            if(item.itemId == itemId){
                myPlayer.items.Remove(item);
                if(item.state == ItemState.Equiped){
                    ItemManager.instance.RemoveItem(item.itemId);
                }
                return;
            }
        }
	}

    public void EquipItem(int id){
        foreach(PlayerItem item in myPlayer.items){
            if(item.itemId == id){
                item.state = ItemState.Equiped;
                foreach(PlayerItem item1 in myPlayer.items){
                    if(item1.itemId != item.itemId && DataHolder.GetItem(item1.itemId).itemType == DataHolder.GetItem(item.itemId).itemType && item1.state == ItemState.Equiped){
                        item1.state = ItemState.Have;
                    }
                }
            }
        }
        
        if(ItemManager.instance != null)
            ItemManager.instance.EquipItem();
    }

    public bool IsHaveItem(int itemId)
	{
       foreach(PlayerItem item in myPlayer.items){
            if(item.itemId == itemId){
                return true;
            }
        }
        return false;
	}

    public Item GetEquipedItem(ItemType type){
        foreach(PlayerItem item in myPlayer.items){   
            if(item.state == ItemState.Equiped && DataHolder.GetItem(item.itemId).itemType == type){
                return DataHolder.GetItem(item.itemId);
            }
        }    
        return  null;    
    }

	public bool IsEquipItem(int itemId)
	{
        foreach(PlayerItem item in myPlayer.items){
            if(item.itemId == itemId && item.state == ItemState.Equiped){
                return true;
            }
        }
        return false;
	}

	public List<int> GetBuyItems(){
		List<int> items = new List<int>();
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(IsHaveItem(DataHolder.Item(i).iD)){
				items.Add(DataHolder.Item(i).iD);
			}
		}
		return items;
	}

    public List<int> GetEquipedItems(){
        List<int> temp = new List<int>();
        foreach(PlayerItem item in myPlayer.items){   
            if(item.state == ItemState.Equiped){
                temp.Add(item.itemId);
            }
        }
        return temp;
    }

    public int GetDiamond(){
        return myPlayer.Diamond;
    }

    public int GetCoin(){
        return myPlayer.Coin;
    }

    public int GetExp(int id){
        return GetPet(id).Exp;
    }

    public void AddDiamond(int d){
        myPlayer.Diamond += d;
        SavePlayer();
    }

    public void AddCoin(int c){
        myPlayer.Coin += c;
        SavePlayer();
    }

    public void AddExp(int e,int petId){
        GetPet(petId).Exp += e;
         if(GetPetObject(petId) != null){
            GameObject go = GameObject.Instantiate(expPrefab,petObjects[0].transform.position,Quaternion.identity);
            go.GetComponent<ExpItem>().Load(e);
         }
    }

    public void CheckEnviroment(CharController charController, EnviromentType type){
        foreach(CharController p in petObjects){
            if(p != charController && p.enviromentType == type){
                p.OnJumpOut();
            }
        }
    }

    public void CollectSkillRewards(int skillId){
/*         foreach(PetSkill s in GetActivePet().skills){
            if(s.skillId == skillId){
                AddCoin(DataHolder.Skills().GetSkill(skillId).coinValue);
                AddDiamond(DataHolder.Skills().GetSkill(skillId).diamondValue);
                AddExp(DataHolder.Skills().GetSkill(skillId).expValue);
                s.rewardState = RewardState.Received;
                SavePlayer();
                return;
            }
        } */
    }

    public void CollectAchivementRewards(int achivementId){
        foreach(PlayerAchivement a in myPlayer.achivements){
            if(a.achivementId == achivementId){
                AddCoin(DataHolder.GetAchivement(achivementId).coinValue[a.level]);
                AddDiamond(DataHolder.GetAchivement(achivementId).diamondValue[a.level]);
                a.rewardState = RewardState.Received;
                a.level ++;
                a.Check();
                SavePlayer();
                return;
            }
        }
    }


    public void SavePlayer(){
         ApiManager.GetInstance().UpdateUserData<PlayerData>(myPlayer); 
    }

    public void LoadPlayer(){
        if(ApiManager.GetInstance().GetUserData<PlayerData>() != null){
            Debug.Log("Load data from local");
            Debug.Log(ApiManager.GetInstance().GetUser().ToJson());
            myPlayer = ApiManager.GetInstance().GetUserData<PlayerData>();
            //LoadNewUser();
        }else{
            Debug.Log("Create New Data");
            LoadNewUser();
        }
    }

    void LoadNewUser(){

        myPlayer = new PlayerData();
        myPlayer.LoadData();

        AddCoin(10000);
        AddDiamond(10000);
        AddItem(17);
        AddItem(41);
        //AddItem(57);
        AddPet(0);
        AddPet(1);
        AddPet(2);
        AddPet(3);
        AddPet(4);
        AddPet(5);
        
        EquipItem(17);    
        EquipItem(41);    
        //EquipItem(57);
        EquipPets();

        
        //#if UNITY_EDITOR
        if(isTest){
            AddItem(2);
            AddItem(11);                
            AddItem(8);
            AddItem(41);
            AddItem(58);
            EquipItem(2);
            EquipItem(11);                
            EquipItem(8);
            AddItem(4);
            EquipItem(4); 
            EquipItem(41);
            EquipItem(58);
        }
        //#endif
        SavePlayer();
    }

    public void LogAchivement(AchivementType achivementType = AchivementType.Do_Action, ActionType actionType = ActionType.None,int itemId = -1,AnimalType animalType = AnimalType.Mouse){
        foreach(PlayerAchivement a in myPlayer.achivements){
            Achivement achivement = DataHolder.GetAchivement(a.achivementId);
            if(achivement.achivementType == achivementType){
                if(achivement.achivementType == AchivementType.Do_Action){
                    if(achivement.actionType == actionType){
                        a.Amount ++;
                    }
                }else if(achivement.achivementType == AchivementType.Buy_Item){                   
                    a.Amount ++;
                }else if(achivement.achivementType == AchivementType.Use_Item || achivement.achivementType == AchivementType.Drink
                || achivement.achivementType == AchivementType.Eat){
                    if(achivement.itemId == itemId){
                            a.Amount ++;
                    }
                }
                else if(achivement.achivementType == AchivementType.Tap_Animal || achivement.achivementType == AchivementType.Dissmiss_Animal){
                    if(achivement.animalType == animalType){
                            a.Amount ++;
                    }
                }else if(achivement.achivementType == AchivementType.LevelUp 
                || achivement.achivementType == AchivementType.Minigame_Level || achivement.achivementType == AchivementType.Play_MiniGame){
                    a.Amount ++;
                }
            }
        }
    }

    #region  Camera
   public void SetCameraTarget(GameObject t){
        if(camera == null)
            camera = Camera.main.GetComponent<CameraController>();
        if(camera != null){
            camera.SetTarget(t);
        }
            
    }

    public void ResetCameraTarget(){
        if(camera == null)
            camera = Camera.main.GetComponent<CameraController>();
        if(camera != null)
            camera.target = null;
    }


    #endregion

}
