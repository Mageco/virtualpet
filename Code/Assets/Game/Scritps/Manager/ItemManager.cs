using System.Collections;
using System.Collections.Generic;
using MageApi;
using MageSDK.Client;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public List<ItemObject> items = new List<ItemObject>();
    public ItemCollider[] itemColliders;

    public GameObject peePrefab;
    public GameObject shitPrefab;
    public GameObject heartPrefab;
    public GameObject coinPrefab;
    public GameObject starPrefab;

    public GameObject dirtyPrefab;
    public GameObject chestPrefab;
    public GameObject healthEffectPrefab;
    public GameObject guidePrefab;
    public GameObject petGiftPrefab;

    float time = 0;
    float maxTimeCheck = 1.1f;
    System.DateTime playTime = System.DateTime.Now;

    public CameraController activeCamera;

    float timeDirty = 0;
    float maxTimeDirty = 200;
    int fruitId = 0;

    float timeChest = 0;
    float maxTimeChest = 30;

    [HideInInspector]
    public HouseItem houseItem;

    [HideInInspector]
    public bool isLoad = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(this.gameObject);

        
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        float t = 0;
        MageManager.instance.loadingBar.gameObject.SetActive(true);
        while(!isLoad){
            if(GameManager.instance.isLoad){
                LoadItems(false);
                GameManager.instance.LoadPetObjects();
                //if(GameManager.instance.isGuest)
                LoadItemData();
                isLoad = true;
            }
            t += Time.deltaTime;
            MageManager.instance.loadingBar.UpdateProgress(t);
            yield return new WaitForEndOfFrame();
        }


        if (ES2.Exists("PlayTime"))
        {
            playTime = ES2.Load<System.DateTime>("PlayTime");
            LoadWelcome((float)(System.DateTime.Now - playTime).TotalSeconds);
        }


        if (ES2.Exists("CameraPosition"))
        {
            GetActiveCamera().transform.position = ES2.Load<Vector3>("CameraPosition");
        }
        GameManager.instance.rateCount++;
        MageManager.instance.loadingBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGuest)
            return;

        if(time > maxTimeCheck){
            CheckItemData();
            time = 0;
            playTime = System.DateTime.Now;
            ES2.Save(playTime, "PlayTime");

            //Check Achivement Notification
            if (GameManager.instance.IsCollectAchivement())
            {
                UIManager.instance.achivementNotification.SetActive(true);
            }else
                UIManager.instance.achivementNotification.SetActive(false);

            //Check Gift Notification
            if (GameManager.instance.IsCollectDailyGift())
            {
                UIManager.instance.giftNotification.SetActive(true);
            }
            else
                UIManager.instance.giftNotification.SetActive(false);

            if (GameManager.instance.gameTime > 400 && !ES2.Exists("RateUs") && (int)GameManager.instance.gameTime % 400 == 0)
            {
                UIManager.instance.OnRatingPopup();
            }
        }
        else{
            time += Time.deltaTime;
        }

        if(timeDirty > maxTimeDirty){
            SpawnDirty();
            timeDirty = 0;
            maxTimeDirty = Random.Range(100,500);
        }else{
            timeDirty += Time.deltaTime;
        }

        if (timeChest > maxTimeChest)
        {
            SpawnChest();
            timeChest = 0;
            maxTimeChest = Random.Range(60, 100);
        }
        else
        {
            timeChest += Time.deltaTime;
        }


    }

    #region  Camera
    public CameraController GetActiveCamera()
    {
        return activeCamera;
    }

    public void SetCameraTarget(GameObject t)
    {
        if (activeCamera != null)
        {
            activeCamera.SetTarget(t);
        }

    }

    public void ResetCameraTarget()
    {
        if (activeCamera != null)
            activeCamera.target = null;
    }

    #endregion



    public void EquipItem()
    {
        LoadItems(true);
        UpdateItemColliders();
        
    }


    public void LoadItems(bool isAnimated)
    {
        
        List<PlayerItem> data = GameManager.instance.GetEquipedPLayerItems();
        Debug.Log("Data " + data.Count);
        List<ItemObject> removes = new List<ItemObject>();

        foreach (ItemObject item in items)
        {
            bool isRemove = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].itemId == item.itemID)
                {
                    isRemove = false;
                }
            }
            if (isRemove)
                removes.Add(item);
        }

        foreach (ItemObject item in removes)
        {
            RemoveItem(item);
        }


        List<PlayerItem> adds = new List<PlayerItem>();
        for (int i = 0; i < data.Count; i++)
        {
            bool isAdd = true;
            foreach (ItemObject item in items)
            {
                if (data[i].itemId == item.itemID)
                {
                    isAdd = false;
                }
            }
            if (isAdd && !adds.Contains(data[i]))
            {
                adds.Add(data[i]);
            }
        }

        for (int i = 0; i < adds.Count; i++)
        {
            AddItem(adds[i],isAnimated);
        }

        UpdateItemColliders();
    }

    public ItemObject GetItem(int itemId){
        foreach(ItemObject item in items){
            if(item.itemID == itemId)
                return item;
        }
        return null;
    }

    public Item GetItemData(ItemType type)
    {
        foreach (ItemObject item in items)
        {
            if (DataHolder.GetItem(item.itemID).itemType == type)
            {
                return DataHolder.GetItem(item.itemID);
            }
        }
        return null;
    }


    void AddItem(PlayerItem playerItem,bool isAnimated)
    {
        if(DataHolder.GetItem(playerItem.itemId) != null)
        {
            string url = DataHolder.GetItem(playerItem.itemId).prefabName.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".prefab", "");
            url = DataHolder.Items().GetPrefabPath() + url;
            for (int i = 0; i < playerItem.number; i++)
            {
                GameObject go = Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
                ItemObject item = go.AddComponent<ItemObject>();
                item.itemType = DataHolder.GetItem(playerItem.itemId).itemType;
                item.itemID = playerItem.itemId;
                items.Add(item);
                go.transform.parent = this.transform;
                if (isAnimated)
                {
                    for (int j = 0; j < item.transform.childCount; j++)
                    {
                        Animator anim = item.transform.GetChild(j).GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.Play("Appear", 0);
                        }
                    }
                }
                else
                {
                    /*
                    for (int j = 0; j < item.transform.childCount; j++)
                    {
                        Animator anim = item.transform.GetChild(j).GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.Play("Idle", 0);
                        }
                    }*/
                }
            }
            Debug.Log(DataHolder.GetItem(playerItem.itemId).GetName(0));
        }       
        
    }

    public void RemoveItem(int id){
        foreach(ItemObject item in items){
            if(item.itemID == id){

                RemoveItem(item);
                return;
            }
        }
    }

    void RemoveItem(ItemObject item)
    {
        if(DataHolder.GetItem(item.itemID).itemType == ItemType.Table)
        {
            foreach(CharController pet in GameManager.instance.GetPetObjects())
            {
                if(pet.enviromentType == EnviromentType.Table)
                {
                    pet.OnJumpOut();
                }
            }
        }
        if (DataHolder.GetItem(item.itemID).itemType == ItemType.Bed)
        {
            foreach (CharController pet in GameManager.instance.GetPetObjects())
            {
                if (pet.enviromentType == EnviromentType.Bed)
                {
                    pet.OnJumpOut();
                }
            }
        }
        if (DataHolder.GetItem(item.itemID).itemType == ItemType.Bath)
        {
            foreach (CharController pet in GameManager.instance.GetPetObjects())
            {
                if (pet.enviromentType == EnviromentType.Bath)
                {
                    pet.OnJumpOut();
                }
            }
        }
        if (DataHolder.GetItem(item.itemID).itemType == ItemType.Toilet)
        {
            foreach (CharController pet in GameManager.instance.GetPetObjects())
            {
                if (pet.enviromentType == EnviromentType.Toilet)
                {
                    pet.OnJumpOut();
                }
            }
        }
        items.Remove(item);
        if(item != null)
            Destroy(item.gameObject);
    }

    public ItemObject GetItem(ItemType type)
    {
        foreach (ItemObject item in items)
        {
            if (DataHolder.GetItem(item.itemID).itemType == type)
            {
                return item;
            }
        }
        return null;
    }


    public GameObject GetItemChildObject(ItemType type){
        foreach(ItemObject item in items){
            if(DataHolder.GetItem(item.itemID).itemType == type){
                //Debug.Log(item.name);
                return item.transform.GetChild(0).gameObject;
            }
        }
        return null;
    }

    public ItemCollider GetEquipment(ItemType type)
    {
        foreach (ItemObject item in items)
        {
            if (DataHolder.GetItem(item.itemID).itemType == type)
            {
                return item.transform.GetComponentInChildren<ItemCollider>();
            }
        }
        return null;
    }

    public int GetItemCount(ItemType type)
    {
        int count = 0;
        foreach (ItemObject item in items)
        {
            if (DataHolder.GetItem(item.itemID).itemType == type)
            {
                count++;
            }
        }
        return count;
    }

    public ToyItem GetRandomToyItem()
    {
        List<ItemObject> temp = new List<ItemObject>();
        foreach (ItemObject item in items)
        {
            if (DataHolder.GetItem(item.itemID).itemType == ItemType.Toy)
            {
                temp.Add(item);
            }
        }
        if (temp.Count > 0)
        {
            int n = Random.Range(0, temp.Count);
            return temp[n].GetComponentInChildren<ToyItem>();
        }
        else
            return null;

    }

    void UpdateItemColliders(){
        itemColliders = this.GetComponentsInChildren<ItemCollider>();
        houseItem = this.GetComponentInChildren<HouseItem>();
    }

    public ItemCollider GetItemCollider(ItemType type){
        foreach(ItemObject item in items){
            if(DataHolder.GetItem(item.itemID).itemType == type){
                if(item.GetComponentInChildren<ItemCollider>() != null)
                return item.GetComponentInChildren<ItemCollider>();
            }
        }
        return null;
    }

    public ItemCollider GetItemCollider(Vector3 dropPosition){
        UpdateItemColliders();
        if(itemColliders != null){
            for(int i=0;i<itemColliders.Length;i++){
                if(IsInCollider(dropPosition,itemColliders[i])){
                    return itemColliders[i];
                }
            }
            return null;
        }
        else 
            return null;

    }

    bool IsInCollider(Vector3 pos,ItemCollider col){
        if(pos.x > col.transform.position.x - col.width/2 && pos.x < col.transform.position.x + col.width/2 &&
        pos.y > col.transform.position.y - col.depth/2 && pos.y < col.transform.position.y + col.depth/2)
        {
            return true;
        }else
        {
            return false;
        }
    }


	public List<Vector3> GetRandomPoints(AreaType type,int n)
	{
		List<Vector3> randomPoints = new List<Vector3> ();
		for (int i = 0; i < n; i++) {
			randomPoints.Add (GetRandomPoint(type));
		}
		return randomPoints;
	}


    public Vector3 GetRandomPoint(AreaType type)
    {
        Vector3 r = Vector3.zero;
        if (type == AreaType.All)
        {

            float x = Random.Range(houseItem.gardenBoundX.x, houseItem.gardenBoundX.y);
            float y = Random.Range(houseItem.gardenBoundY.x, houseItem.gardenBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Garden)
        {
            bool isDone = false;
            float x = Random.Range(houseItem.gardenBoundX.x, houseItem.gardenBoundX.y);
            float y = Random.Range(houseItem.gardenBoundY.x, houseItem.gardenBoundY.y);
            while (!isDone)
            {
                x = Random.Range(houseItem.gardenBoundX.x, houseItem.gardenBoundX.y);
                y = Random.Range(houseItem.gardenBoundY.x, houseItem.gardenBoundY.y);
                if (x < houseItem.roomBoundX.x || x > houseItem.roomBoundX.y)
                {
                    isDone = true;
                }
            }
            r = new Vector3(x, y, 0);
        }else if(type == AreaType.Room)
        {
            float x = Random.Range(houseItem.roomBoundX.x, houseItem.roomBoundX.y);
            float y = Random.Range(houseItem.roomBoundY.x, houseItem.roomBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Camera)
        {
            float x = Random.Range(houseItem.cameraBoundX.x, houseItem.cameraBoundX.y);
            float y = Random.Range(houseItem.cameraBoundY.x, houseItem.cameraBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Fly)
        {
            float x = Random.Range(houseItem.roomBoundX.x, houseItem.roomBoundX.y);
            float y = Random.Range(houseItem.roomBoundY.x + 15, houseItem.roomBoundY.y + 15);
            r = new Vector3(x, y, 0);
        }
        return r;
    }

    public CharCollector GetCharCollector(int id)
    {
        CharCollector[] chars = FindObjectsOfType<CharCollector>();

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i].petId == id)
            {
                return chars[i];
            }
        }
        
        return null;
    }

    public void SpawnPee(Vector3 pos,float value)
    {
        GameObject go = Instantiate(peePrefab, pos, Quaternion.identity);
        go.GetComponent<ItemDirty>().dirty = value;
    }

    public void SpawnShit(Vector3 pos,float value)
    {
        GameObject go = Instantiate(shitPrefab, pos, Quaternion.identity);
        go.GetComponent<ItemDirty>().dirty = value;
    }


    public void SpawnStar(Vector3 pos,  int value)
    {
        if (GameManager.instance.isGuest)
            return;

        GameObject go = Instantiate(starPrefab, pos + new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),0), Quaternion.identity);
        go.GetComponent<StarItem>().Load(value);
    }

    public void SpawnCoin(Vector3 pos, int value,GameObject item = null)
    {
        GameObject go = Instantiate(coinPrefab, pos,Quaternion.identity);
        go.GetComponent<CoinItem>().Load(value);
        if(item != null)
        {
            go.transform.parent = item.transform;
        }
        SpawnStar(pos,1);
    }

    public GameObject SpawnGuideArrow(ItemType itemType)
    {
        GameObject go = Instantiate(guidePrefab, GetItem(itemType).transform.GetChild(0).transform.position + new Vector3(0,3,0), Quaternion.identity);
        go.transform.parent = GetItem(itemType).transform.GetChild(0).transform;
        return go;
    }

    public GameObject SpawnGuideArrow(GameObject item,Vector3 pos)
    {
        GameObject go = Instantiate(guidePrefab, pos, Quaternion.identity);
        go.transform.parent = item.transform;
        return go;
    }

    public void SpawnHeart(int n,Vector3 p)
    {
        GameObject go = Instantiate(heartPrefab, p + new Vector3(0, 0, -5), Quaternion.identity);
        go.GetComponent<HappyItem>().Load(n);
        SpawnStar(p, 1);
    }

    public void SpawnHealth(Vector3 pos)
    {
        GameObject go = Instantiate(healthEffectPrefab, pos, Quaternion.identity);
    }

    public void SpawnDirty()
    {
        Vector3 pos = GetRandomPoint(AreaType.All) + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        pos.z = 990;
        GameObject go = Instantiate(dirtyPrefab, pos, Quaternion.identity);
    }

    public void SpawnChest()
    {
        ChestItem[] chests = FindObjectsOfType<ChestItem>();
        if (chests.Length > 2)
            return;
        Vector3 pos = GetRandomPoint(AreaType.Garden) + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        pos.z = pos.y * 10;
        GameObject go = Instantiate(chestPrefab, pos, Quaternion.identity);
    }

    void CheckItemData(){
       GameManager.instance.myPlayer.itemSaveDatas.Clear();

       //Find All dirty items
       ItemDirty[] dirties = FindObjectsOfType<ItemDirty>();
       for(int i=0;i<dirties.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.itemType = dirties[i].itemType;
           data.value = dirties[i].dirty;
           data.position = dirties[i].transform.position;
           GameManager.instance.myPlayer.itemSaveDatas.Add(data);
       }

        //Find All Food/Drink Item
        EatItem[] eats = FindObjectsOfType<EatItem>();
        for(int i=0;i<eats.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.id = eats[i].item.itemID;
           data.itemType = eats[i].itemSaveDataType;
           data.value = eats[i].foodAmount;
           data.position = eats[i].transform.position;
           GameManager.instance.myPlayer.itemSaveDatas.Add(data);
       }


        FruitItem[] fruits = FindObjectsOfType<FruitItem>();
        for (int i = 0; i < fruits.Length; i++)
        {
            ItemSaveData data = new ItemSaveData();
            data.id = fruits[i].id;
            data.itemType = fruits[i].itemSaveDataType;
            data.value = fruits[i].step;
            data.time = fruits[i].time;
            GameManager.instance.myPlayer.itemSaveDatas.Add(data);
        }

        ToyItem[] toys = FindObjectsOfType<ToyItem>();
        for (int i = 0; i < toys.Length; i++)
        {
            ItemSaveData data = new ItemSaveData();
            data.id = toys[i].item.itemID;
            data.itemType = ItemSaveDataType.Toy;
            data.position = toys[i].transform.position;
            GameManager.instance.myPlayer.itemSaveDatas.Add(data);
        }


        ItemCollider[] equipments = FindObjectsOfType<ItemCollider>();
        for (int i = 0; i < equipments.Length; i++)
        {
            ItemSaveData data = new ItemSaveData();
            data.id = equipments[i].item.itemID;
            data.itemType = ItemSaveDataType.Equipment;
            data.position = equipments[i].transform.position;
            GameManager.instance.myPlayer.itemSaveDatas.Add(data);
        }


        foreach (ItemObject item in items)
        {

        }
        SaveItemData();
    }


    void SaveItemData(){
        ES2.Save(GetActiveCamera().transform.position, "CameraPosition");
        GameManager.instance.SavePlayer();
    }

    void LoadItemData(){

        foreach(ItemSaveData item in GameManager.instance.myPlayer.itemSaveDatas){
            if(item.itemType == ItemSaveDataType.Pee){
                SpawnPee(item.position,item.value);
            }else if(item.itemType == ItemSaveDataType.Shit){
                SpawnShit(item.position,item.value);
            }else if(item.itemType == ItemSaveDataType.Food || item.itemType == ItemSaveDataType.Drink){
                if(GetItem(item.id) != null){
                    GetItem(item.id).GetComponentInChildren<EatItem>().foodAmount = item.value;
                    GetItem(item.id).GetComponentInChildren<EatItem>().transform.position = item.position;
                }
            }else if(item.itemType == ItemSaveDataType.Fruit)
            {
                FruitItem[] fruits = FindObjectsOfType<FruitItem>();
                for (int i = 0; i < fruits.Length; i++)
                {
                    if (fruits[i].id == item.id)
                    {
                        fruits[i].step = (int)item.value;
                        fruits[i].time = item.time;
                        fruits[i].Load();
                    }
                }
            }
            else if (item.itemType == ItemSaveDataType.Toy)
            {
                if (GetItem(item.id) != null)
                {
                    GetItem(item.id).GetComponentInChildren<ToyItem>().transform.position = item.position;
                }
            }
            else if (item.itemType == ItemSaveDataType.Equipment)
            {
                if (GetItem(item.id) != null)
                {
                    GetItem(item.id).GetComponentInChildren<ItemCollider>().transform.position = item.position;
                }
            }
        }
    }

    public void LoadWelcome(float t)
    {
        int c = 0;
        int h = 0;
        #if UNITY_EDITOR
                if (MageEngine.instance.resetUserDataOnStart)
                    return;
        #endif
        //Debug.Log(t);
        if (t > 10800)
        {
            c = 30;
            h = 60;
        }
        else if (t > 7200)
        {
            c = 20;
            h = 40;
        }
        else if (t > 3600)
        {
            c = 10;
            h = 20;
        }
        else if (t > 1800)
        {
            c = 5;
            h = 10;
        }
        else if (t > 600)
        {
            c = 2;
            h = 4;
        }
        else 
        {
            c = 1;
            h = 2;
        }

        if (t >= 300)
            UIManager.instance.OnWelcomeBack(c, h);

    }

    public int GetFruitId()
    {
        fruitId++;
        return fruitId;
    }

    #region Pet
    public void LoadPetObject(Pet pet)
    {
        if (pet.isNew)
        {
            StartCoroutine(SpawnGift(pet));

        }
        else
        {
            SpawnPet(pet);
        }

    }

    IEnumerator SpawnGift(Pet pet)
    {
        GameObject gift = GameObject.Instantiate(petGiftPrefab);
        Vector3 pos = new Vector3(Camera.main.transform.position.x, Random.Range(-15, -10), 0);
        pos.z = pos.y * 10;
        gift.transform.position = pos;
        yield return new WaitForSeconds(5.4f);
        CharController petObject = SpawnPet(pet);
        petObject.agent.transform.position = pos;
        petObject.transform.position = pos;
        petObject.OnGift();
        pet.isNew = false;
    }

    CharController SpawnPet(Pet pet)
    {
        if (pet.character != null)
            return pet.character;

        Pet p = DataHolder.GetPet(pet.iD);

        if (p == null)
            return null;

        pet.rareType = p.rareType;
        if (pet.rareType == RareType.Rare)
            pet.rateHappy = 3;
        else if (pet.rareType == RareType.Epic)
            pet.rateHappy = 5;
        else if (pet.rareType == RareType.Legend)
            pet.rateHappy = 10;
        else if (pet.rareType == RareType.Common)
            pet.rateHappy = 1;

        string url = "";
        url = p.prefabName.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".prefab", "");
        url = DataHolder.Pets().GetPrefabPath() + url;
        Debug.Log(url);
        GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
        pet.character = go.GetComponent<CharController>();
        go.transform.position = ItemManager.instance.GetRandomPoint(AreaType.All);
        pet.character.data = pet;
        pet.character.LoadPrefab();
        GameManager.instance.UpdatePetObjects();
        return pet.character;
    }

    public void UnLoadPetObject(Pet p)
    {
        if (p.character.agent != null)
            GameObject.Destroy(p.character.agent.gameObject);
        if (p.character != null)
            GameObject.Destroy(p.character.gameObject);
    }


    #endregion

}
