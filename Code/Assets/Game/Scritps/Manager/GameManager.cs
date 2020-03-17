using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public float gameTime = 0;
    public List<CharController> petObjects = new List<CharController>();

    //GameType lastGameType = GameType.House;

    //Testing
    public bool isLoad = false;
    public bool isTest = false;

    public PlayerData myPlayer = new PlayerData();

    public int rateCount = 0;
    [HideInInspector]
    public int expScale = 1;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            GameObject.Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        //Application.targetFrameRate = 50;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DataHolder.Instance();
        DataHolder.Instance().Init();
        if (ES2.Exists("GameTime"))
        {
            gameTime = ES2.Load<float>("GameTime");
        }
    }


    private void Start()
    {
        LoadPlayer();
        isLoad = true;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        foreach (CharController c in petObjects)
        {
            if (c.charInteract.interactType != InteractType.Toy)
            {
                Vector3 pos = c.transform.position;
                pos.z = (int)(c.charScale.scalePosition.y * 10);
                c.transform.position = pos;
            }
        }


        bool isDone = false;
        while (!isDone)
        {
            isDone = true;

            foreach (CharController c in petObjects)
            {
                foreach (CharController c1 in petObjects)
                {
                    if (c != c1 && c.transform.position.z == c1.transform.position.z && c.charInteract.interactType == InteractType.None && c1.charInteract.interactType == InteractType.None)
                    {
                        if (c.transform.position.y >= c1.transform.position.y)
                        {
                            Vector3 pos = c.transform.position;
                            pos.z -= 1;
                            c.transform.position = pos;
                            isDone = false;
                        }
                        else
                        {
                            Vector3 pos = c1.transform.position;
                            pos.z -= 1;
                            c1.transform.position = pos;
                            isDone = false;
                        }
                    }
                }
            }
        }
    }

    public PlayerData GetPlayer()
    {
        return myPlayer;
    }

    public void LoadPetObjects()
    {
        if (ItemManager.instance == null)
            return;

        for (int i = 0; i < myPlayer.pets.Count; i++)
        {
            if (myPlayer.pets[i].itemState == ItemState.Equiped)
            {
                ItemManager.instance.LoadPetObject(myPlayer.pets[i]);
                UpdatePetObjects();
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

    public void AddRandomPet(RareType rareType)
    {
        List<Pet> pets = new List<Pet>();
        for(int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
        {
            if(DataHolder.Pet(i).isAvailable && !IsHavePet(DataHolder.Pet(i).iD) && DataHolder.Pet(i).rareType == rareType)
            {
                pets.Add(DataHolder.Pet(i));
            }
        }

        if(pets.Count > 0)
        {
            int itemId = Random.Range(0, pets.Count);
            Pet p = new Pet(itemId);
            p.itemState = ItemState.Equiped;
            p.isNew = true;
            myPlayer.pets.Add(p);
            
        }
        else
        {
            if(rareType == RareType.Common)
                AddDiamond(99);
            else if(rareType == RareType.Rare)
                AddDiamond(299);
            else if (rareType == RareType.Epic)
                AddDiamond(999);
        }

        SavePlayer();

    }

    public void EquipPet(int itemId)
    {
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == itemId)
            {
                p.itemState = ItemState.Equiped;
            }
        }
        LoadPetObjects();
    }

    public void EquipPets()
    {
        foreach (Pet p in myPlayer.pets)
        {
            p.itemState = ItemState.Equiped;
        }
    }

    public void UnEquipPet(int itemId)
    {
        if(GetPetObject(itemId).actionType == ActionType.Toy || GetPetObject(itemId).enviromentType != EnviromentType.Room)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(107).GetName(MageManager.instance.GetLanguage()));
            return;
        }

        
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == itemId)
            {
                p.itemState = ItemState.Have;
                if (ItemManager.instance != null)
                {
                    petObjects.Remove(p.character);
                    ItemManager.instance.UnLoadPetObject(p);
                }
            }
        }
        SavePlayer();

    }

    public void UnEquipPets()
    {
        foreach (Pet p in myPlayer.pets)
        {
            p.itemState = ItemState.Have;
            if (ItemManager.instance != null)
            {
                petObjects.Remove(p.character);
                ItemManager.instance.UnLoadPetObject(p);
            }
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



    public void UpdatePetObjects()
    {
        if (ItemManager.instance == null)
            return;
        petObjects.Clear();
        for (int i = 0; i < myPlayer.pets.Count; i++)
        {
            if (myPlayer.pets[i].itemState == ItemState.Equiped && myPlayer.pets[i].character != null)
                petObjects.Add(myPlayer.pets[i].character);
        }
    }

    public bool BuyPet(int petId)
    {
        Debug.Log("Buy pet " + petId);
        PriceType type = DataHolder.GetPet(petId).priceType;
        int price = DataHolder.GetPet(petId).buyPrice;
        if (type == PriceType.Coin)
        {
            if (price > GetCoin())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
                return false;
            }
            AddCoin(-price);
            AddPet(petId);
            GetPet(petId).isNew = true;
            Debug.Log("Buy pet " + petId);
            return true;
        }
        else if (type == PriceType.Diamond)
        {
            if (price > GetDiamond())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.GetDialog(7).GetDescription(MageManager.instance.GetLanguage()));
                return false;
            }
            AddDiamond(-price);
            AddPet(petId);
            GetPet(petId).isNew = true;
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
            GetPet(petId).isNew = true;
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
        int price = DataHolder.GetPet(petId).buyPrice / 2;
        if (type == PriceType.Coin)
        {
            AddCoin(price);
        }
        else if (type == PriceType.Diamond)
        {
            AddDiamond(price);
        }
        else if (type == PriceType.Happy)
        {
            AddHappy(price);
        }
        RemovePet(petId);
    }


    public Pet GetPet(int id)
    {
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == id)
                return p;
        }
        return null;
    }

    public List<Pet> GetPets()
    {
        return myPlayer.pets;
    }

    public List<CharController> GetPetObjects()
    {
        return petObjects;
    }

    public CharController GetPetObject(int id)
    {
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == id)
                return p.character;
        }
        return null;
    }

    public CharController GetRandomPetObject()
    {
        if (petObjects.Count > 0)
        {
            int n = Random.Range(0, petObjects.Count);
            return petObjects[n];
        }
        else
            return null;

    }

    void RemovePet(int id)
    {
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == id)
            {
                myPlayer.pets.Remove(p);
                if (ItemManager.instance != null)
                {
                    petObjects.Remove(p.character);
                    ItemManager.instance.UnLoadPetObject(p);
                }
                return;
            }
        }
    }

    public Pet GetActivePet()
    {
        return myPlayer.pets[0];
    }


    public bool IsHavePet(int petId)
    {
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == petId)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEquipPet(int petId)
    {
        foreach (Pet p in myPlayer.pets)
        {
            if (p.iD == petId && p.itemState == ItemState.Equiped)
            {
                return true;
            }
        }
        return false;
    }

    public List<int> GetBuyPets()
    {
        List<int> pets = new List<int>();
        for (int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
        {
            if (IsHavePet(DataHolder.Pet(i).iD))
            {
                pets.Add(DataHolder.Pet(i).iD);
            }
        }
        return pets;
    }

    public List<int> GetEquipedPets()
    {
        List<int> pets = new List<int>();
        for (int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
        {
            if (IsEquipPet(DataHolder.Pet(i).iD))
            {
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
        if (type == PriceType.Coin)
        {
            if (price > GetCoin())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
                return false;
            }
            AddCoin(-price);
            AddItem(itemId);
            return true;
        }
        else if (type == PriceType.Diamond)
        {
            if (price > GetDiamond())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
                return false;
            }
            AddDiamond(-price);
            AddItem(itemId);
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

    public void SellItem(int itemId)
    {
        PriceType type = DataHolder.GetItem(itemId).priceType;
        int price = DataHolder.GetItem(itemId).buyPrice / 2;
        if (type == PriceType.Coin)
        {
            AddCoin(price);
        }
        else if (type == PriceType.Diamond)
        {
            AddDiamond(price);
        }
        else if (type == PriceType.Happy)
        {
            AddHappy(price);
        }
        RemoveItem(itemId);
    }

    public void AddItem(int id)
    {
        if (DataHolder.GetItem(id).itemType == ItemType.Diamond)
        {
            AddDiamond(DataHolder.GetItem(id).sellPrice);
        }
        else if (DataHolder.GetItem(id).itemType == ItemType.Coin)
        {
            AddCoin(DataHolder.GetItem(id).sellPrice);
        }
        else
        {

            bool isExist = false;
            foreach (PlayerItem item in myPlayer.items)
            {
                if (item.itemId == id)
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
                        Debug.Log(" Exist " + id);
                    }
                }
            }
            if (!isExist)
            {
                PlayerItem item = new PlayerItem();
                item.itemId = id;
                item.state = ItemState.Have;
                item.itemType = DataHolder.GetItem(id).itemType;
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
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.itemId == itemId)
            {
                myPlayer.items.Remove(item);
                if (item.state == ItemState.Equiped)
                {
                    ItemManager.instance.RemoveItem(item.itemId);
                }
                SavePlayer();
                return;
            }
        }
    }

    public void EquipItem(int id)
    {

        //Fix all item that same id
        List<PlayerItem> temp = new List<PlayerItem>();
        foreach (PlayerItem item1 in myPlayer.items)
        {
            foreach (PlayerItem item2 in myPlayer.items)
            {
                if (item1 != item2 && item1.itemId == item2.itemId)
                {
                    temp.Add(item2);
                }
            }
        }

        foreach (PlayerItem item in temp)
        {
            myPlayer.items.Remove(item);
        }

        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.itemId == id)
            {
                item.state = ItemState.Equiped;
            }
            else if (DataHolder.GetItem(item.itemId) != null && DataHolder.GetItem(id).itemType == DataHolder.GetItem(item.itemId).itemType
               && DataHolder.GetItem(id).itemType != ItemType.Toy && DataHolder.GetItem(id).itemType != ItemType.Fruit)
            {
                item.state = ItemState.Have;
            }
        }




        if (ItemManager.instance != null && DataHolder.GetItem(id).itemType != ItemType.Coin && DataHolder.GetItem(id).itemType != ItemType.Diamond)
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
                if (ItemManager.instance != null)
                    ItemManager.instance.RemoveItem(item.itemId);
                SavePlayer();
                return;
            }
        }
    }

    public bool IsHaveItem(int itemId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.itemId == itemId)
            {
                return true;
            }
        }
        return false;
    }

    public Item GetEquipedItem(ItemType type)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.state == ItemState.Equiped && DataHolder.GetItem(item.itemId).itemType == type)
            {
                return DataHolder.GetItem(item.itemId);
            }
        }
        return null;
    }

    public bool IsEquipItem(int itemId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.itemId == itemId && item.state == ItemState.Equiped)
            {
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

    public List<int> GetBuyItems()
    {
        List<int> items = new List<int>();
        for (int i = 0; i < DataHolder.Items().GetDataCount(); i++)
        {
            if (IsHaveItem(DataHolder.Item(i).iD))
            {
                items.Add(DataHolder.Item(i).iD);
            }
        }
        return items;
    }

    public List<int> GetBuyItems(ItemType type)
    {
        List<int> items = new List<int>();
        for (int i = 0; i < DataHolder.Items().GetDataCount(); i++)
        {
            if (IsHaveItem(DataHolder.Item(i).iD) && DataHolder.Item(i).itemType == type)
            {
                items.Add(DataHolder.Item(i).iD);
            }
        }
        return items;
    }

    public List<int> GetEquipedItems()
    {
        List<int> temp = new List<int>();
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.state == ItemState.Equiped)
            {
                temp.Add(item.itemId);
            }
        }
        return temp;
    }

    public List<PlayerItem> GetEquipedPLayerItems()
    {
        List<PlayerItem> temp = new List<PlayerItem>();
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.state == ItemState.Equiped)
            {
                temp.Add(item);
            }
        }
        return temp;
    }

    public int GetDiamond()
    {
        return myPlayer.Diamond;
    }

    public int GetCoin()
    {
        return myPlayer.Coin;
    }

    public int GetHappy()
    {
        return myPlayer.Happy;
    }


    public int GetExp(int id)
    {
        return GetPet(id).Exp;
    }

    public void AddDiamond(int d)
    {
        myPlayer.Diamond += d;
        if (UIManager.instance != null)
            UIManager.instance.diamonText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        SavePlayer();
    }

    public void AddCoin(int c)
    {
        myPlayer.Coin += c;
        if (c > 0)
            myPlayer.collectedCoin += c;
        if (UIManager.instance != null)
            UIManager.instance.coinText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        SavePlayer();
    }

    public void AddHappy(int c)
    {
        myPlayer.Happy += c;
        if (c > 0)
            myPlayer.collectedHappy += c;

        if (UIManager.instance != null)
            UIManager.instance.heartText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        SavePlayer();
    }

    public void AddExp(int c)
    {
        myPlayer.exp += c;
        int l = 1;
        float e = 20 * l + 20 * l * l;
        while (myPlayer.exp >= e)
        {
            l++;
            e = 20 * l + 20 * l * l;
        }

        if (l > myPlayer.level)
        {
            myPlayer.level = l;
            UIManager.instance.OnLevelUpPanel();
            OnTip();
            UIManager.instance.OnSale();
            Debug.Log("Level Up");
        }

        SavePlayer();
    }

    public void LevelUp(int petId)
    {
        if (GetPet(petId) != null)
        {
            MageManager.instance.PlaySound3D("points_ticker_bonus_score_reward_single_06", false, GetPetObject(petId).transform.position);
            GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Effects/LevelUp") as GameObject);
            go.transform.parent = GetPetObject(petId).transform;
            go.transform.position = GetPetObject(petId).transform.position + new Vector3(0, 2, -1);
            AddHappy(-10 * GetPet(petId).level * GetPet(petId).level);
            GetPet(petId).level++;
            SavePlayer();
        }

    }


    void OnTip()
    {
        if (myPlayer.level == 2)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(85).GetName(MageManager.instance.GetLanguage()));
        }
        else if (myPlayer.level == 3)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(77).GetName(MageManager.instance.GetLanguage()));
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(79).GetName(MageManager.instance.GetLanguage()));
        }
    }

    public void CollectAchivementRewards(int achivementId)
    {
        foreach (PlayerAchivement a in myPlayer.achivements)
        {
            if (a.achivementId == achivementId)
            {
                AddCoin(DataHolder.GetAchivement(achivementId).coinValue[a.level]);
                AddDiamond(DataHolder.GetAchivement(achivementId).diamondValue[a.level]);
                a.rewardState = RewardState.Received;
                a.level++;
                a.Check();
                SavePlayer();
                return;
            }
        }
    }


    public void SavePlayer()
    {
        myPlayer.petCount = myPlayer.pets.Count;
        myPlayer.itemCount = myPlayer.items.Count;
        myPlayer.playTime = gameTime;
        ES2.Save(gameTime, "GameTime");
        MageEngine.instance.UpdateUserData<PlayerData>(myPlayer);
    }

    public void LoadPlayer()
    {
        if (MageEngine.instance.GetUserData<PlayerData>() != null)
        {
            Debug.Log("Load data from local");
            Debug.Log(MageEngine.instance.GetUser().ToJson());
            myPlayer = MageEngine.instance.GetUserData<PlayerData>();
            if (myPlayer.minigameLevels.Length == 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    myPlayer.minigameLevels = ArrayHelper.Add(0, myPlayer.minigameLevels);
                }
            }
            
        }
        else
        {
            Debug.Log("Create New Data");
            LoadNewUser();
        }
    }

    void LoadNewUser()
    {

        myPlayer = new PlayerData();
        myPlayer.LoadData();

        AddCoin(200);
        AddDiamond(1);

        AddItem(17);
        //AddItem(7);
        //AddItem(8);
        //AddItem(4);
        AddItem(41);
        AddItem(58);
        AddItem(59);
        //AddItem(11);
        AddItem(2);
        //AddItem(13);

        AddPet(0);
        GetPet(0).isNew = true;

        //#if UNITY_EDITOR
        if (isTest)
        {
            AddCoin(10000);
            AddDiamond(10000);
            AddPet(1);
            AddPet(2);
            AddPet(3);
            AddPet(4);
            AddPet(5);
        }
        //#endif

        foreach (PlayerItem item in myPlayer.items)
        {
            item.state = ItemState.Equiped;
        }

        EquipPets();
        SavePlayer();
        isLoad = true;
    }

    public void LogAchivement(AchivementType achivementType = AchivementType.Do_Action, ActionType actionType = ActionType.None, int itemId = -1, AnimalType animalType = AnimalType.Mouse)
    {
        foreach (PlayerAchivement a in myPlayer.achivements)
        {
            Achivement achivement = DataHolder.GetAchivement(a.achivementId);
            if (achivement.achivementType == achivementType)
            {
                if (achivement.achivementType == AchivementType.Do_Action)
                {
                    if (achivement.actionType == actionType)
                    {
                        a.Amount++;
                    }
                }
                else if (achivement.achivementType == AchivementType.Buy_Item)
                {
                    a.Amount++;
                }
                else if (achivement.achivementType == AchivementType.Use_Item || achivement.achivementType == AchivementType.Drink
               || achivement.achivementType == AchivementType.Eat)
                {
                    if (achivement.itemId == itemId)
                    {
                        a.Amount++;
                    }
                }
                else if (achivement.achivementType == AchivementType.Tap_Animal || achivement.achivementType == AchivementType.Dissmiss_Animal)
                {
                    if (achivement.animalType == animalType)
                    {
                        a.Amount++;
                    }
                }
                else if (achivement.achivementType == AchivementType.Minigame_Level || achivement.achivementType == AchivementType.Play_MiniGame)
                {
                    if (achivement.itemId == itemId)
                    {
                        a.Amount++;
                    }
                }
                else
                {
                    a.Amount++;
                }
            }
        }
    }

    public int GetAchivement(AchivementType type)
    {
        foreach (var item in myPlayer.achivements)
        {
            if (item.achivementType == type)
                return item.Amount;
        }
        return 0;
    }

    public int GetAchivement(int id)
    {
        if (myPlayer.achivements == null)
            myPlayer.achivements = new List<PlayerAchivement>();
        foreach (var item in myPlayer.achivements)
        {
            if (item.achivementId == id)
                return item.Amount;
        }
        return 0;
    }

    public int GetAchivementCollectTime()
    {
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
            if (DataHolder.GetAchivement(item.achivementId).isAvailable && item.rewardState == RewardState.Ready)
                return true;
        }
        return false;
    }

    public void OnTreatment(Pet p, SickType sickType, int coin)
    {
        AddCoin(-coin);
        if (sickType == SickType.Sick)
            LogAchivement(AchivementType.Sick);
        else if (sickType == SickType.Injured)
            LogAchivement(AchivementType.Injured);
        p.character.OnTreatment(sickType);
    }

    public bool IsCollectDailyGift()
    {
        bool isCollect = false;
        int n = 0;
        for (int i = 0; i < myPlayer.dailyBonus.Count; i++)
        {
            if (GameManager.instance.myPlayer.dailyBonus[i].isCollected)
            {
                n++;
            }
        }

        if (n == 0)
        {
            isCollect = true;
        }
        else
        {
           if (System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Year < System.DateTime.Now.Year || System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Month < System.DateTime.Now.Month || System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Day < System.DateTime.Now.Day)
            {
                isCollect = true;
            }
        }

        return isCollect;
    }

}
