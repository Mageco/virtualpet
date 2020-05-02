﻿using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public float gameTime = 0;
    public List<CharController> petObjects = new List<CharController>();
    public bool isGuest = false;
    //GameType lastGameType = GameType.House;

    //Testing
    public bool isLoad = false;
    public bool isTest = false;

    public PlayerData myPlayer = new PlayerData();
    public PlayerData guest = new PlayerData();

    public int rateCount = 0;
    [HideInInspector]
    public int expScale = 1;

    float saveTime = 0;
    float maxSaveTime = 1;

    string passWord = "M@ge2013";
    [HideInInspector]
    public int count = 14536;

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
        //Save
        if (saveTime > maxSaveTime)
        {
            saveTime = 0;
            SavePlayer();
        }
        else
            saveTime += Time.deltaTime;

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
                        if (c.transform.position.y <= c1.transform.position.y)
                        {
                            Vector3 pos = c.transform.position;
                            pos.z += 5;
                            c.transform.position = pos;
                            isDone = false;
                        }
                        else
                        {
                            Vector3 pos = c1.transform.position;
                            pos.z += 5;
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

    public int GetRealItemId()
    {
        myPlayer.realItemId++;
        return myPlayer.realItemId;
    }

    public int GetRealPetId()
    {
        myPlayer.realPetId++;
        return myPlayer.realPetId;
    }

    public void AddPetObject(CharController petObject)
    {
        if (!petObjects.Contains(petObject))
            petObjects.Add(petObject);
    }

    public void LoadPetObjects()
    {
        if (ItemManager.instance == null)
            return;

        if (!isGuest)
        {
            for (int i = 0; i < myPlayer.petDatas.Count; i++)
            {
                if (myPlayer.petDatas[i].itemState == ItemState.Equiped)
                {
                    ItemManager.instance.LoadPetObject(myPlayer.petDatas[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < guest.petDatas.Count; i++)
            {
                if (guest.petDatas[i].itemState == ItemState.Equiped)
                {
                    ItemManager.instance.LoadPetObject(guest.petDatas[i]);
                }
            }
        }

    }



    public int AddPet(int itemId,string key)
    {
        if (IsOK(key))
        {
            PlayerPet p = new PlayerPet(itemId);
            p.realId = GetRealPetId();
            p.petName = DataHolder.GetPet(itemId).GetName(0);
            p.itemState = ItemState.Have;
            myPlayer.petDatas.Add(p);
            return p.realId;
        }
        return 0;
    }

    bool IsOK(string key)
    {
        if(key == Utils.instance.Md5Sum(count.ToString() + myPlayer.playTime.ToString() + myPlayer.Happy.ToString() + passWord ))
        {
            count++;
            return true;
        }
        return false;
    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(count.ToString() + myPlayer.playTime.ToString() + myPlayer.Happy.ToString() + passWord);
    }

    public void AddRandomPet(RareType rareType,string key)
    {
        if (IsOK(key))
        {

            List<Pet> pets = new List<Pet>();
            for (int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
            {
                if (DataHolder.Pet(i).isAvailable && !IsHavePet(DataHolder.Pet(i).iD) && DataHolder.Pet(i).rareType == rareType)
                {
                    pets.Add(DataHolder.Pet(i));
                }
            }

            Debug.Log(pets.Count);

            if (pets.Count > 0)
            {
                int itemId = Random.Range(0, pets.Count);
                PlayerPet p = new PlayerPet(pets[itemId].iD);
                p.realId = GetRealPetId();
                p.itemState = ItemState.Equiped;
                p.isNew = true;
                p.petName = DataHolder.GetPet(itemId).GetName(0);
                myPlayer.petDatas.Add(p);
                if (ItemManager.instance != null)
                    GameManager.instance.EquipPet(itemId);
            }
            else
            {
                if (rareType == RareType.Common)
                    AddDiamond(49,GetKey());
                else if (rareType == RareType.Rare)
                    AddDiamond(99, GetKey());
                else if (rareType == RareType.Epic)
                    AddDiamond(199, GetKey());
            }
            if (UIManager.instance.shopPanel != null)
                UIManager.instance.shopPanel.Close();

            if (UIManager.instance.chestSalePanel != null)
                UIManager.instance.chestSalePanel.Close();
        }
    }

    public void EquipPet(int itemId)
    {
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            if (p.realId == itemId)
            {
                p.itemState = ItemState.Equiped;
            }
        }
        LoadPetObjects();
    }

    public void EquipPets()
    {
        foreach (PlayerPet p in myPlayer.petDatas)
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

        
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            if (p.realId == itemId)
            {
                p.itemState = ItemState.Have;
                if (ItemManager.instance != null)
                {
                    CharController c = GetPetObject(p.realId);
                    petObjects.Remove(c);
                    ItemManager.instance.UnLoadPetObject(c);
                    
                }
            }
        }
    }

    public void UnEquipPets()
    {
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            p.itemState = ItemState.Have;
            if (ItemManager.instance != null)
            {
                petObjects.Remove(GetPetObject(p.realId));
                ItemManager.instance.UnLoadPetObject(GetPetObject(p.realId));
            }
        }
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



    public int BuyPet(int petId)
    {
        int realId = 0;
        Debug.Log("Buy pet " + petId);
        PriceType type = DataHolder.GetPet(petId).priceType;
        int price = DataHolder.GetPet(petId).buyPrice;
        if (type == PriceType.Coin)
        {
            if (price > GetCoin())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
            }
            AddCoin(-price, GetKey());
            realId = AddPet(petId, GetKey());
            GetPet(realId).isNew = true;
            Debug.Log("Buy pet " + petId);
         }
        else if (type == PriceType.Diamond)
        {
            if (price > GetDiamond())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.GetDialog(7).GetDescription(MageManager.instance.GetLanguage()));
            }
            AddDiamond(-price, GetKey());
            realId = AddPet(petId, GetKey());
            GetPet(realId).isNew = true;
        }
        else if (type == PriceType.Happy)
        {
            if (price > GetHappy())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
            }
            AddHappy(-price,GetKey());
            realId = AddPet(petId, GetKey());
            GetPet(realId).isNew = true;
       }
        return realId;
        
    }

    public void SellPet(int realId)
    {
        if (GetPetObject(realId).actionType == ActionType.Toy)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(107).GetName(MageManager.instance.GetLanguage()));
            return;
        }

        int petId = GetPet(realId).iD;
        PriceType type = DataHolder.GetPet(petId).priceType;
        int price = DataHolder.GetPet(petId).buyPrice / 2;
        if (type == PriceType.Coin)
        {
            AddCoin(price, GetKey());
        }
        else if (type == PriceType.Diamond)
        {
            AddDiamond(price, GetKey());
        }
        else if (type == PriceType.Happy)
        {
            AddHappy(price, GetKey());
        }
        RemovePet(realId);
        if (UIManager.instance.profilePanel != null)
            UIManager.instance.profilePanel.Load();
    }


    public PlayerPet GetPet(int realId)
    {
        if (isGuest)
        {
            foreach (PlayerPet p in this.guest.petDatas)
            {
                if (p.realId == realId)
                    return p;
            }
        }
        else
        {
            foreach (PlayerPet p in this.myPlayer.petDatas)
            {
                if (p.realId == realId)
                    return p;
            }
        }

        return null;
    }

    public List<PlayerPet> GetPets()
    {
        if (isGuest)
        {
            return guest.petDatas;
        }
        else
            return myPlayer.petDatas;
    }

    public List<CharController> GetPetObjects()
    {
        return petObjects;
    }

    public CharController GetPetObject(int realId)
    {
        foreach (CharController p in this.petObjects)
        {
            if (p.data.realId == realId)
                return p;
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

    void RemovePet(int realId)
    {
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            if (p.realId == realId)
            {
                 if (ItemManager.instance != null)
                {
                    CharController petObject = GetPetObject(realId);
                    petObjects.Remove(petObject);
                    ItemManager.instance.UnLoadPetObject(petObject);
                }
                if(GetPetNumber(p.iD) >= 2)
                {
                    myPlayer.petDatas.Remove(p);
                }else
                    p.itemState = ItemState.OnShop;
                return;
            }
        }
    }

    public void RemoveAllPetObjects()
    {
        foreach(CharController p in petObjects)
        {
            ItemManager.instance.UnLoadPetObject(p);
        }
        petObjects.Clear();
    }

    public PlayerPet GetActivePet()
    {
        return myPlayer.petDatas[0];
    }

    public CharController GetActivePetObject()
    {
        if (petObjects.Count > 0)
            return petObjects[0];
        else
            return null;
    }

    public bool IsHavePet(int petId)
    {
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            if (p.iD == petId && (p.itemState == ItemState.Have || p.itemState == ItemState.Equiped))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEquipPet(int petId)
    {
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            if (p.iD == petId && p.itemState == ItemState.Equiped)
            {
                return true;
            }
        }
        return false;
    }

    public int GetPetNumber(int petId)
    {
        int count = 0;
        foreach (PlayerPet p in myPlayer.petDatas)
        {
            if (p.iD == petId)
            {
                count++;
            }
        }
        return count;
    }

    //Items
    public int BuyItem(int itemId)
    {
        int realId = 0;
        PriceType type = DataHolder.GetItem(itemId).priceType;
        int price = DataHolder.GetItem(itemId).buyPrice;
        if (type == PriceType.Coin)
        {
            if (price > GetCoin())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
                return 0;
            }
            AddCoin(-price, GetKey());
            realId = AddItem(itemId, GetKey());
        }
        else if (type == PriceType.Diamond)
        {
            if (price > GetDiamond())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
                return 0;
            }
            AddDiamond(-price, GetKey());
            realId = AddItem(itemId, GetKey());
        }
        else if (type == PriceType.Happy)
        {
            if (price > GetHappy())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
                return 0;
            }
            AddHappy(-price, GetKey());
            realId = AddItem(itemId, GetKey());
        }

        return realId;

    }

    public void SellItem(int realId)
    {
        int itemId = GetItem(realId).itemId;
        PriceType type = DataHolder.GetItem(itemId).priceType;
        int price = DataHolder.GetItem(itemId).buyPrice / 2;
        if (type == PriceType.Coin)
        {
            AddCoin(price, GetKey());
        }
        else if (type == PriceType.Diamond)
        {
            AddDiamond(price, GetKey());
        }
        else if (type == PriceType.Happy)
        {
            AddHappy(price, GetKey());
        }
        RemoveItem(realId);
    }

    public int AddItem(int id,string key)
    {
        int realId = 0;
        if (IsOK(key))
        {
            if (DataHolder.GetItem(id).itemType == ItemType.Diamond)
            {
                AddDiamond(DataHolder.GetItem(id).sellPrice, GetKey());
            }
            else if (DataHolder.GetItem(id).itemType == ItemType.Coin)
            {
                AddCoin(DataHolder.GetItem(id).sellPrice, GetKey());
            }
            else
            {
                if (DataHolder.GetItem(id).consume)
                {
                    bool isExist = false;
                    
                    foreach (PlayerItem item in myPlayer.items)
                    {
                        if (item.itemId == id)
                        {
                            item.state = ItemState.Have;
                            item.number++;
                            isExist = true;
                            realId = item.realId;
                        }
                    }
                    if (!isExist)
                    {
                        PlayerItem item = new PlayerItem();
                        item.itemId = id;
                        item.realId = GetRealItemId();
                        item.state = ItemState.Have;
                        item.itemType = DataHolder.GetItem(id).itemType;
                        item.isConsumable = DataHolder.GetItem(id).consume;
                        item.number = 1;
                        Debug.Log("Not Exist " + id);
                        myPlayer.items.Add(item);
                        realId = item.realId;
                    }
               }
                else
                {
                    PlayerItem item = new PlayerItem();
                    item.itemId = id;
                    item.realId = GetRealItemId();
                    item.state = ItemState.Have;
                    item.itemType = DataHolder.GetItem(id).itemType;
                    item.isConsumable = DataHolder.GetItem(id).consume;
                    item.number = 1;
                    Debug.Log("Not Exist " + id);
                    myPlayer.items.Add(item);
                    realId = item.realId;
                }
            }
        }

        return realId;
    }

    public PlayerItem GetItem(int realId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.realId == realId)
            {
                return item;
            }
        }
        return null;
    }

    public void RemoveItem(int realId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.realId == realId)
            {
                if (item.state == ItemState.Equiped)
                {
                    myPlayer.items.Remove(item);
                    ItemManager.instance.RemoveItem(item.realId);
                }
                return;
            }
        }
    }

    public void EquipItem(int realId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.realId == realId)
            {
                foreach(PlayerItem item1 in myPlayer.items)
                {
                    if (item != item1 && item.itemId == item1.itemId && (item.itemType == ItemType.Room || item.itemType == ItemType.Gate || item.itemType == ItemType.Board || item.itemType == ItemType.Clean))
                        item1.state = ItemState.Have;
                }
                item.state = ItemState.Equiped;
                if (ItemManager.instance != null)
                    ItemManager.instance.EquipItem();
            }
        }



    }

    public void UnEquipItem(int realId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.realId == realId)
            {
                item.state = ItemState.Have;
                if (ItemManager.instance != null)
                    ItemManager.instance.RemoveItem(item.realId);
                
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


    public bool IsEquipItem(int itemId)
    {
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.realId == itemId && item.state == ItemState.Equiped)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItemNumber(ItemType type)
    {
        int count = 0;
        foreach (PlayerItem item in myPlayer.items)
        {
            if (item.itemType == type && (item.state == ItemState.Have || item.state == ItemState.Equiped))
            {
                count++;
            }
        }
        return count;
    }


    public List<PlayerItem> GetEquipedPLayerItems()
    {
        List<PlayerItem> temp = new List<PlayerItem>();
        if (isGuest)
        {
            foreach (PlayerItem item in guest.items)
            {
                if (item.state == ItemState.Equiped)
                {
                    temp.Add(item);
                }
            }
        }
        else
        {
            foreach (PlayerItem item in myPlayer.items)
            {
                if (item.state == ItemState.Equiped)
                {
                    temp.Add(item);
                }
            }
        }

        return temp;
    }

    public int GetItemNumber(int id)
    {
        int count = 0;
        foreach(PlayerItem item in myPlayer.items)
        {
            if(item.itemId == id && (item.state == ItemState.Have || item.state == ItemState.Equiped))
            {
                count++;
                if (item.isConsumable)
                    count = item.number;
            }
        }

        return count;
    }

    #region Accessory
    public void BuyAccessory(int itemId,int realId)
    {
        PriceType type = DataHolder.GetAccessory(itemId).priceType;
        int price = DataHolder.GetAccessory(itemId).buyPrice;
        if (type == PriceType.Coin)
        {
            if (price > GetCoin())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(6).GetDescription(MageManager.instance.GetLanguage()));
                return;
            }
            AddCoin(-price, GetKey());
            AddAccessory(itemId, realId, GetKey());
        }
        else if (type == PriceType.Diamond)
        {
            if (price > GetDiamond())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(7).GetDescription(MageManager.instance.GetLanguage()));
                return;
            }
            AddDiamond(-price, GetKey());
            AddAccessory(itemId, realId, GetKey());
        }
        else if (type == PriceType.Happy)
        {
            if (price > GetHappy())
            {
                MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
                return;
            }
            AddHappy(-price, GetKey());
            AddAccessory(itemId,realId,GetKey());
        }
        CharController pet = GetPetObject(realId);
        if(pet != null)
        {
            pet.LoadCharObject();
        }

    }

    public void EquipAccessory(int id, int realId)
    {
        PlayerPet p = GetPet(realId);
        if (p != null)
        {
            p.accessoryId = id;
            CharController pet = GetPetObject(realId);
            if (pet != null)
            {
                pet.LoadCharObject();
            }
        }
    }

    public void AddAccessory(int id,int realId, string key)
    {
        if (IsOK(key))
        {
            PlayerPet p = GetPet(realId);
            if(p != null)
            {
                p.accessoryId = id;
                p.accessories.Add(id);
            }
        }
    }

    public bool IsHaveAccessory(int petId,int itemId)
    {
        foreach (PlayerPet pet in myPlayer.petDatas)
        {
            if (pet.iD == petId && pet.accessoryId == itemId)
            {
                return true;
            }
        }
        return false;
    }

    public void Evolve(int realId)
    {
        foreach (PlayerPet pet in myPlayer.petDatas)
        {
            if (pet.realId == realId)
            {
                List<Accessory> temp = DataHolder.GetAccessories(pet.iD);
                int ran = Random.Range(0, temp.Count);
                pet.accessoryId = ran;
                CharController petObject = GetPetObject(realId);
                if (petObject != null)
                {
                    petObject.LoadCharObject();
                }
            }
        }
    }
    #endregion


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


    public void AddDiamond(int d,string key)
    {
        if (GameManager.instance.isGuest)
            return;

        Debug.Log(key);

        if (IsOK(key))
        {
            if (d > 0)
                myPlayer.collectedDiamond += d;
            myPlayer.Diamond += d;
            if (UIManager.instance != null)
                UIManager.instance.diamonText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        }
        
    }

    public void AddCoin(int c,string key)
    {
        if (IsOK(key))
        {
            myPlayer.Coin += c;
            if (c > 0)
                myPlayer.collectedCoin += c;
            if (UIManager.instance != null)
                UIManager.instance.coinText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        }
    }

    public void AddHappy(int c,string key)
    {
        if (GameManager.instance.isGuest)
            return;

        if (IsOK(key))
        {
            myPlayer.Happy += c;
            if (c > 0)
                myPlayer.collectedHappy += c;

            if (UIManager.instance != null)
                UIManager.instance.heartText.transform.parent.GetComponent<Animator>().Play("Active", 0);
        }
        
    }

    public void AddExp(int c,string key)
    {
        if (GameManager.instance.isGuest)
            return;

        if (IsOK(key))
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
        }
        
        //Force
    }

    public void LevelUp(int realId)
    {
        if (GetPet(realId) != null)
        {
            AddHappy(-10 * GetPet(realId).level * GetPet(realId).level,GetKey());
            GetPet(realId).level++;

            if (GetPetObject(realId) != null)
            {
                MageManager.instance.PlaySound3D("points_ticker_bonus_score_reward_single_06", false, GetPetObject(realId).transform.position);
                GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Effects/LevelUp") as GameObject);
                go.transform.parent = GetPetObject(realId).transform;
                go.transform.position = GetPetObject(realId).transform.position + new Vector3(0, 2, -1);
                GetPetObject(realId).data.level = GetPet(realId).level;
            }
        }
    }


    void OnTip()
    {
        if (myPlayer.level == 2)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(77).GetName(MageManager.instance.GetLanguage()));
        }
        else if (myPlayer.level == 3)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(143).GetName(MageManager.instance.GetLanguage()));
        }
    }

    public void CollectAchivementRewards(int achivementId)
    {
        foreach (PlayerAchivement a in myPlayer.achivements)
        {
            if (a.achivementId == achivementId)
            {
                AddCoin(DataHolder.GetAchivement(achivementId).coinValue[a.level], GetKey());
                AddDiamond(DataHolder.GetAchivement(achivementId).diamondValue[a.level], GetKey());
                a.rewardState = RewardState.Received;
                a.level++;
                a.Check();
                
                return;
            }
        }
    }


    public void SavePlayer()
    {
        myPlayer.petCount = myPlayer.petDatas.Count;
        myPlayer.itemCount = myPlayer.items.Count;
        myPlayer.playTime = gameTime;
        ES2.Save(gameTime, "GameTime");
        MageEngine.instance.UpdateUserData<PlayerData>(myPlayer);
    }

    public void ForceSavePlayer()
    {
        myPlayer.petCount = myPlayer.petDatas.Count;
        myPlayer.itemCount = myPlayer.items.Count;
        myPlayer.playTime = gameTime;
        ES2.Save(gameTime, "GameTime");
        Debug.Log("Force Update");
        MageEngine.instance.UpdateUserData<PlayerData>(myPlayer,true);
    }

    public void LoadPlayer()
    {
        if (MageEngine.instance.GetUserData<PlayerData>() != null)
        {
            Debug.Log("Load data from local");
            Debug.Log(MageEngine.instance.GetUser().ToJson());
            myPlayer = MageEngine.instance.GetUserData<PlayerData>();

            if(myPlayer.pets.Count > 0)
            {
                foreach(Pet p in myPlayer.pets)
                {
                    PlayerPet pet = new PlayerPet(p.iD);
                    pet.level = p.level;
                    pet.realId = GetRealPetId();
                    pet.itemState = p.itemState;
                    pet.isNew = p.isNew;
                    p.petName = DataHolder.GetPet(p.iD).GetName(0);
                    myPlayer.petDatas.Add(pet);
                }
                myPlayer.pets.Clear();
            }

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

        foreach(PlayerItem item in myPlayer.items)
        {
            if (item.realId == 0)
                item.realId = GetRealItemId();
        }

        foreach (PlayerPet pet in myPlayer.petDatas)
        {
            if (pet.realId == 0)
                pet.realId = GetRealPetId();
        }

        foreach (PlayerPet pet in myPlayer.petDatas)
        {
            if (pet.accessories == null || pet.accessories.Count == 0)
            {
                pet.accessories = new List<int>();
                pet.accessories.Add(0);
            }
        }
    }

    void LoadNewUser()
    {

        myPlayer = new PlayerData();
        myPlayer.LoadData();
        myPlayer.version = Application.version;
        Debug.Log("Version " + myPlayer.version);

        
        AddCoin(100, GetKey());
        AddDiamond(1, GetKey());
        AddHappy(10, GetKey());

        AddItem(17, GetKey());
        AddItem(41, GetKey());
        AddItem(170, GetKey());

        /*
        AddItem(77, GetKey());
        AddItem(8, GetKey());
        AddItem(4, GetKey());
        
        AddItem(58, GetKey());
        AddItem(59, GetKey());
        AddItem(11, GetKey());
        AddItem(2, GetKey());
        //AddItem(13);
        AddItem(1, GetKey());
        AddItem(69, GetKey());
        AddItem(81, GetKey());
        AddItem(87, GetKey());
        
        AddItem(163, GetKey());*/
        int realId = AddPet(0, GetKey());
        GetPet(realId).isNew = true;

        foreach (PlayerItem item in myPlayer.items)
        {
            item.state = ItemState.Equiped;
        }

        EquipPets();
        SavePlayer();
        isLoad = true;
    }


    #region Achivement
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
    #endregion

    #region Daily Quest
    public DailyQuestData GetDailyQuestData(int id)
    {
        foreach(DailyQuestData quest in myPlayer.dailyQuests)
        {
            if (quest.achivementId == id)
                return quest;
        }
        return null;
    }

    public void CompleteDailyQuest(int id)
    {
        foreach (DailyQuestData quest in myPlayer.dailyQuests)
        {
            if (quest.achivementId == id)
            {
                quest.state = DailyQuestState.Collected;
                quest.timeCollected = MageEngine.instance.GetServerTimeStamp().ToString();
                GameManager.instance.AddHappy(quest.bonus,GetKey());
            }
        }

    }

    #endregion

    public void OnTreatment(CharController p, SickType sickType, int coin)
    {
        AddCoin(-coin, GetKey());
        if (sickType == SickType.Sick)
            LogAchivement(AchivementType.Sick);
        else if (sickType == SickType.Injured)
            LogAchivement(AchivementType.Injured);
        p.OnTreatment(sickType);
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
            if (System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Year < MageEngine.instance.GetServerTimeStamp().Year || System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Month < MageEngine.instance.GetServerTimeStamp().Month || System.DateTime.Parse(GameManager.instance.myPlayer.dailyBonus[n - 1].timeReceived).Day < MageEngine.instance.GetServerTimeStamp().Day)
            {
                isCollect = true;
            }
        }

        return isCollect;
    }

}
