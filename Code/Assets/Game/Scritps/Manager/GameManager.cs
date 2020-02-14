using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    
 	public static GameManager instance;
    public float gameTime = 0;
    public List<CharController> petObjects = new List<CharController>();
    CameraController camera;
    
    //GameType lastGameType = GameType.House;

    //Testing
    public bool isLoad = false;
    
    public GameObject heartPrefab;
    public GameObject expPrefab;

    public bool isTest = false;

    public PlayerData myPlayer = new PlayerData();

    public int rateCount = 0;

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
        if (ES2.Exists("GameTime"))
        {
            gameTime = ES2.Load<float>("GameTime");
        }
    }


    private void Start() {
        LoadPlayer();
        isLoad = true;
    }

    void Update(){
		gameTime += Time.deltaTime;
	}

    void LateUpdate(){
        foreach(CharController c in petObjects){
            Vector3 pos = c.transform.position;
            pos.z = (int)(c.charScale.scalePosition.y * 10);
            c.transform.position = pos;
        }

        bool isDone = false;
        while(!isDone){
            isDone = true;

            foreach(CharController c in petObjects){
                foreach(CharController c1 in petObjects){
                    if(c != c1 && c.transform.position.z == c1.transform.position.z){
                        if(c.transform.position.y >= c1.transform.position.y){
                            Vector3 pos = c.transform.position;
                            pos.z -= 1;
                            c.transform.position = pos;
                            isDone = false;
                        }else{
                            Vector3 pos = c1.transform.position;
                            pos.z -=1;
                            c1.transform.position =pos;
                            isDone = false;
                        }
                    }
                }
            }
        } 
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
    }

    public void AddPet(int itemId)
    {
        Pet p = new Pet(itemId);
        p.itemState = ItemState.Have;
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

    public void UnLoadPets()
    {
        foreach (CharController p in petObjects)
        {
            Destroy(p.agent.gameObject);
            Destroy(p.gameObject);

        }
        petObjects.Clear();
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
                MageManager.instance.OnNotificationPopup (DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
				return false;
			}
			AddCoin (-price);
			AddPet (petId);
            Debug.Log("Buy pet " + petId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
                MageManager.instance.OnNotificationPopup (DataHolder.GetDialog(7).GetDescription(MageManager.instance.GetLanguage()));
				return false;
			}
			AddDiamond (-price);
			AddPet (petId);
			return true;
		}
        else if (type == PriceType.Happy)
        {
            if (price > GetHappy())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
                return false;
            }
            AddHappy(-price);
            AddPet(petId);
            return true;
        }
        else
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
        else if (type == PriceType.Happy)
        {
            AddHappy(price);
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

    public CharController GetRandomPetObject(){
        int n = Random.Range(0,petObjects.Count);
        return petObjects[n];
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

    
     

    //Items
    public bool BuyItem(int itemId)
	{
		PriceType type = DataHolder.GetItem(itemId).priceType;
		int price = DataHolder.GetItem(itemId).buyPrice;
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
				MageManager.instance.OnNotificationPopup (DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
				return false;
			}
			AddCoin (-price);
			AddItem (itemId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
				MageManager.instance.OnNotificationPopup (DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
				return false;
			}
			AddDiamond (-price);
			AddItem (itemId);
			return true;
		}
        else if (type == PriceType.Happy)
        {
            if (price > GetHappy())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
                return false;
            }
            AddHappy(-price);
            AddItem(itemId);
            return true;
        }
        else
		{
			return false;
		}
        
    }

    public void SellItem(int itemId){
        PriceType type = DataHolder.GetItem(itemId).priceType;
		int price = DataHolder.GetItem(itemId).buyPrice/2;
		if(type == PriceType.Coin){
			AddCoin (price);
		}else if(type == PriceType.Diamond){
			AddDiamond (price);
		}
        else if (type == PriceType.Happy)
        {
            AddHappy(price);
        }
        RemoveItem(itemId);
    }

    public void AddItem(int id){
        if(DataHolder.GetItem(id).itemType == ItemType.Diamond){
            AddDiamond(DataHolder.GetItem(id).sellPrice);
        }else if(DataHolder.GetItem(id).itemType == ItemType.Coin){
            AddCoin(DataHolder.GetItem(id).sellPrice);
        }else{

            bool isExist = false;
            foreach(PlayerItem item in myPlayer.items)
            {
                if(item.itemId == id)
                {
                    if (DataHolder.GetItem(id).consume)
                    {
                        item.state = ItemState.Have;
                        item.number++;
                        isExist = true;
                    }
                    else
                    {
                        item.state = ItemState.Have;
                        item.number = 1;
                        isExist = true;
                        Debug.Log( " Exist " + id);
                    }
                }
            }
            if (!isExist)
            {
                PlayerItem item = new PlayerItem();
                item.itemId = id;
                item.state = ItemState.Have;
                item.isConsumable = DataHolder.GetItem(id).consume;
                item.number = 1;
                Debug.Log("Not Exist " + id);
                myPlayer.items.Add(item);
            }
            
        }
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
                SavePlayer();
                return;
            }
        }
	}

    public void EquipItem(int id){

        //Fix all item that same id
        List<PlayerItem> temp = new List<PlayerItem>();
        foreach(PlayerItem item1 in myPlayer.items)
        {
            foreach(PlayerItem item2 in myPlayer.items)
            {
                if(item1 != item2 && item1.itemId == item2.itemId)
                {
                    temp.Add(item2);
                }
            }
        }

        foreach(PlayerItem item in temp)
        {
            myPlayer.items.Remove(item);
        }

        foreach(PlayerItem item in myPlayer.items){
            if(item.itemId == id){
                item.state = ItemState.Equiped;
            }else if(DataHolder.GetItem(item.itemId) != null && DataHolder.GetItem(id).itemType == DataHolder.GetItem(item.itemId).itemType
                && DataHolder.GetItem(id).itemType != ItemType.Toy && DataHolder.GetItem(id).itemType != ItemType.Fruit)
            {
                item.state = ItemState.Have;
            }
        }

        
        
        
        if(ItemManager.instance != null && DataHolder.GetItem(id).itemType != ItemType.Coin && DataHolder.GetItem(id).itemType != ItemType.Diamond)
            ItemManager.instance.EquipItem();

        SavePlayer();
    }

    public void UnEquipItem(int itemId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.itemId == itemId)
            {
                item.state = ItemState.Have;
                ItemManager.instance.RemoveItem(item.itemId);
                SavePlayer();
                return;
            }
        }
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

    public bool IsEquipItem(ItemType itemType)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (DataHolder.GetItem(item.itemId).itemType == itemType && item.state == ItemState.Equiped)
            {
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

    public List<int> GetBuyItems(ItemType type){
		List<int> items = new List<int>();
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(IsHaveItem(DataHolder.Item(i).iD) && DataHolder.Item(i).itemType == type){
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

    public List<PlayerItem> GetEquipedPLayerItems(){
        List<PlayerItem> temp = new List<PlayerItem>();
        foreach(PlayerItem item in myPlayer.items){   
            if(item.state == ItemState.Equiped){
                temp.Add(item);
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

    public int GetHappy(){
        return myPlayer.Happy;
    }


    public int GetExp(int id){
        return GetPet(id).Exp;
    }

    public void AddDiamond(int d){
        myPlayer.Diamond += d;
        if (UIManager.instance != null)
            UIManager.instance.diamonText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        SavePlayer();
    }

    public void AddCoin(int c){
        myPlayer.Coin += c;
        if(c > 0)
            myPlayer.collectedCoin += c;
        if(UIManager.instance != null)
            UIManager.instance.coinText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        SavePlayer();
    }

    public void AddHappy(int c){
        myPlayer.Happy += c;
        if (c > 0)
            myPlayer.collectedHappy += c;
        int temp = myPlayer.level;
        int l = 1;
        float e = 20 * l + 20 * l * l;
        while (myPlayer.collectedHappy > e)
        {
            l++;
            e = 20 * l + 20 * l * l;
        }
        //if (l > temp)
        //{
            myPlayer.level = l;
        //}
        if (UIManager.instance != null)
            UIManager.instance.heartText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        SavePlayer();
    }

    public void AddExp(int e,int petId,bool happy = true){
        GetPet(petId).Exp += e;
        if(GetPetObject(petId) != null){

            if(happy){
                GameObject go = GameObject.Instantiate(heartPrefab,GetPetObject(petId).transform.position,Quaternion.identity);
                int n = (e + GetPet(petId).level)/5;
                for(int i=0;i<n;i++){
                    int ran = Random.Range(0,100);
                    Quaternion rot = Quaternion.identity;
                    if(ran > 50)
                        rot = Quaternion.Euler(new Vector3(0,180,-1));
                    Vector3 pos = GetPetObject(petId).charScale.scalePosition + new Vector3(Random.Range(-1,1),Random.Range(-1,1),0);
                    ItemManager.instance.SpawnHeart(pos,rot,1,true);
                }
            }else{
                GameObject go = GameObject.Instantiate(expPrefab,petObjects[0].transform.position,Quaternion.identity);
                go.GetComponent<ExpItem>().Load(e);
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
        myPlayer.petCount = myPlayer.pets.Count;
        myPlayer.itemCount = myPlayer.items.Count;
        myPlayer.playTime = gameTime;
        ES2.Save(gameTime, "GameTime");
        MageEngine.instance.UpdateUserData<PlayerData>(myPlayer); 
    }

    public void LoadPlayer(){
        if(MageEngine.instance.GetUserData<PlayerData>() != null){
            Debug.Log("Load data from local");
            Debug.Log(MageEngine.instance.GetUser().ToJson());
            myPlayer = MageEngine.instance.GetUserData<PlayerData>();
            if(myPlayer.minigameLevels.Length == 1)
            {
                for(int i = 0; i < 20; i++)
                {
                    myPlayer.minigameLevels = ArrayHelper.Add(0, myPlayer.minigameLevels);
                }
            }
        }else{
            Debug.Log("Create New Data");
            LoadNewUser();
        }
    }

    void LoadNewUser(){

        myPlayer = new PlayerData();
        myPlayer.LoadData();

        AddCoin(0);
        AddDiamond(1);
        
        AddItem(17);
        AddItem(7);
        AddItem(8);
        AddItem(4);
        AddItem(41);
        AddItem(58);
        AddItem(59);
        AddItem(11);
        AddItem(2);
        AddItem(13);

        AddPet(0);
        
        //#if UNITY_EDITOR
        if(isTest){
            AddCoin(10000);
            AddDiamond(10000);
            AddPet(1);
            AddPet(2);
            AddPet(3);
            AddPet(4);
            AddPet(5);
        }
        //#endif

        foreach(PlayerItem item in myPlayer.items){
                item.state = ItemState.Equiped;
        }

        EquipPets();
        SavePlayer();
        isLoad = true;
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
                }else{
                    a.Amount ++;
                }
            }
        }
    }

    public int GetAchivement(AchivementType type){
        foreach (var item in myPlayer.achivements)
        {
            if(item.achivementType == type)
                return item.Amount;
        }
        return 0;
    }

    public int GetAchivement(int id){
        foreach (var item in myPlayer.achivements)
        {
            if(item.achivementId == id)
                return item.Amount;
        }
        return 0;
    }

    public int GetAchivementCollectTime(){
        int count = 0;
        foreach (var item in myPlayer.achivements)
        {
            count += item.level;
        }
        return count;
    }

    public bool IsCollectAchivement()
    {
        foreach (var item in myPlayer.achivements)
        {
            if (item.rewardState == RewardState.Ready)
                return true;
        }
        return false;
    }

    public void OnTreatment(Pet p,SickType sickType,int coin)
    {
        AddCoin(-coin);
        ItemManager.instance.SpawnHealth(p.character.transform.position);
        if (sickType == SickType.Sick)
            LogAchivement(AchivementType.Sick);
        else if (sickType == SickType.Injured)
            LogAchivement(AchivementType.Injured);
        p.character.OnTreatment(sickType);
    }



}
