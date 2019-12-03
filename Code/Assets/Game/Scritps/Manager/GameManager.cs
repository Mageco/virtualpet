using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    
 	public static GameManager instance;
    public float gameTime = 0;
    List<CharController> petObjects = new List<CharController>();
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


    public void LoadPetObjects()
    {
        for (int i = 0; i < myPlayer.pets.Count; i++)
        {
            if(myPlayer.pets[i].itemState == ItemState.Equiped){
                CharController c = myPlayer.pets[i].Load();
                petObjects.Add(c);
            }
        }
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
    }



    public CharController GetPetObject(int id){
        return petObjects[id];
    }

    public void UpdatePetObjects(){
        petObjects.Clear();
        for(int i=0;i<myPlayer.pets.Count;i++){
            petObjects.Add(myPlayer.pets[i].character);
        }
    }

    public bool BuyPet(int petId)
	{
		PriceType type = DataHolder.GetPet(petId).priceType;
		int price = DataHolder.GetPet(petId).buyPrice;
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
				return false;
			}
			AddCoin (-price);
			AddPet (petId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
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

    public Pet GetPet(int id){
        foreach(Pet p in myPlayer.pets){
            if(p.iD == id)
                return p;
        }
        return null;
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
        return myPlayer.pets[id].Exp;
    }

    public void AddDiamond(int d){
        myPlayer.Diamond += d;
        SavePlayer();
    }

    public void AddCoin(int c){
        myPlayer.Coin += c;
        SavePlayer();
    }

    public void AddExp(int e){
         myPlayer.pets[0].Exp += e;
         if(petObjects[0] != null){
            GameObject go = GameObject.Instantiate(expPrefab,petObjects[0].transform.position,Quaternion.identity);
            go.GetComponent<ExpItem>().Load(e);
         }
    }

    public void CollectSkillRewards(int skillId){
        foreach(PetSkill s in GetActivePet().skills){
            if(s.skillId == skillId){
                AddCoin(DataHolder.Skills().GetSkill(skillId).coinValue);
                AddDiamond(DataHolder.Skills().GetSkill(skillId).diamondValue);
                AddExp(DataHolder.Skills().GetSkill(skillId).expValue);
                s.rewardState = RewardState.Received;
                SavePlayer();
                return;
            }
        }
    }

    public void CollectAchivementRewards(int achivementId){

    }


    public void SavePlayer(){
         ApiManager.GetInstance().UpdateUserData<PlayerData>(myPlayer); 
    }

    public void LoadPlayer(){
        if(ApiManager.GetInstance().GetUserData<PlayerData>() != null){
            Debug.Log("Load data from local");
            Debug.Log(ApiManager.GetInstance().GetUser().ToJson());
            myPlayer = ApiManager.GetInstance().GetUserData<PlayerData>();
        }else{
            Debug.Log("Create New Data");
            LoadNewUser();
        }
    }

    void LoadNewUser(){

        myPlayer = new PlayerData();
        myPlayer.LoadData();

        AddItem(17);
        AddItem(57);
        AddPet(0);
        
        EquipItem(17);        
        EquipItem(57);
        EquipPet(0);

        
        #if UNITY_EDITOR
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
        #endif
        
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


    public void OnMinigame(int id){
        MageManager.instance.LoadSceneWithLoading("Minigame" + id.ToString());
        gameType = GameType.Minigame1;
        GetActivePet().Load();
    }

    #endregion

}
