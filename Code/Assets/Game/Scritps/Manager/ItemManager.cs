using System.Collections;
using System.Collections.Generic;
using MageApi;
using MageSDK.Client;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public List<BaseFloorItem> items = new List<BaseFloorItem>();

    public GameObject peePrefab;
    public GameObject shitPrefab;
    public GameObject heartPrefab;
    public GameObject coinPrefab;
    public GameObject starPrefab;

    public GameObject dirtyPrefab;
    public GameObject chestPrefab;
    public GameObject pillEffectPrefab;
    public GameObject bandageEffectPrefab;
    public GameObject guidePrefab;
    public GameObject petGiftPrefab;
    public GameObject petHappyPrefab;

    public Vector2 roomBoundX = new Vector2(-50, 80);
    public Vector2 roomBoundY = new Vector2(-26, 0);
    public Vector2 roomWallBoundY = new Vector2(3, 30);
    public Vector2 gardenBoundX = new Vector2(-270, 150);
    public Vector2 gardenBoundY = new Vector2(-26, 0);
    public Vector2 cameraBoundX = new Vector2(-320, 200);
    public Vector2 cameraBoundY = new Vector2(-26, 40);

    System.DateTime startTime = System.DateTime.Now;
    float time = 0;
    float maxTimeCheck = 1.1f;
    System.DateTime playTime = System.DateTime.Now;

    public CameraController activeCamera;

    float timeDirty = 0;
    float maxTimeDirty = 200;
    int fruitId = 0;

    float timeChest = 0;
    float maxTimeChest = 10;

    [HideInInspector]
    public bool isLoad = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        if (ES2.Exists("PlayTime"))
        {
            startTime = ES2.Load<System.DateTime>("PlayTime");
        }
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        float t = 0;
        MageManager.instance.loadingBar.gameObject.SetActive(true);
        while(!isLoad){
            if(GameManager.instance.isLoad){
                LoadItems();
                LoadItemData();
                LoadArea();
                GameManager.instance.LoadPetObjects();
                isLoad = true;
            }
            t += Time.deltaTime;
            MageManager.instance.loadingBar.UpdateProgress(t);
            yield return new WaitForEndOfFrame();
        }

        if (ES2.Exists("CameraPosition"))
        {
            GetActiveCamera().transform.position = ES2.Load<Vector3>("CameraPosition");
        }
        GetActiveCamera().boundX = cameraBoundX;
        GetActiveCamera().boundY = cameraBoundY;
        GameManager.instance.rateCount++;
        t += Time.deltaTime;
        MageManager.instance.loadingBar.UpdateProgress(t);
        yield return new WaitForEndOfFrame();
        MageManager.instance.loadingBar.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        if(!GameManager.instance.isGuest)
            LoadWelcome((float)(System.DateTime.Now - startTime).TotalSeconds);
        
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

        if (timeChest > maxTimeChest && GameManager.instance.myPlayer.level >=2)
        {
            SpawnChest();
            timeChest = 0;
            maxTimeChest = Random.Range(10, 30);
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


    public void LoadItems()
    {
        
        List<PlayerItem> data = GameManager.instance.GetEquipedPLayerItems();
        List<BaseFloorItem> removes = new List<BaseFloorItem>();
        foreach (BaseFloorItem item in items)
        {
            bool isRemove = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].realId == item.realID)
                {
                    isRemove = false;
                }
            }
            if (isRemove && !removes.Contains(item))
            {
                Debug.Log(item.name);
                removes.Add(item);
            }
        }


        foreach (BaseFloorItem item in removes)
        {
            RemoveItem(item);
        }

        List<PlayerItem> adds = new List<PlayerItem>();
        for (int i = 0; i < data.Count; i++)
        {
            bool isAdd = true;
            foreach (BaseFloorItem item in items)
            {
                if (data[i].realId == item.realID)
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
            AddItem(adds[i]);
        }
    }

   

    public BaseFloorItem GetRandomItem(int itemId){
        List<BaseFloorItem> temp = new List<BaseFloorItem>();
        foreach(BaseFloorItem item in items){
            if (item.itemID == itemId)
                temp.Add(item);
        }

        if(temp.Count > 0)
        {
            int id = Random.Range(0, temp.Count);
            return temp[id];
        }else
            return null;
    }

    public BaseFloorItem GetRandomItem(ItemType type)
    {
        List<BaseFloorItem> temp = new List<BaseFloorItem>();
        foreach (BaseFloorItem item in items)
        {
            if (item.itemType == type)
            {
                temp.Add(item);
            }
        }
        if (temp.Count > 0)
        {
            int n = Random.Range(0, temp.Count);
            return temp[n];
        }
        else
            return null;
    }

    public BaseFloorItem FindFreeRandomItem(ItemType type)
    {
        List<BaseFloorItem> temp = new List<BaseFloorItem>();
        foreach (BaseFloorItem item in items)
        {
            if (item.itemType == type && !item.IsBusy())
            {
                temp.Add(item);
            }
        }
        if (temp.Count > 0)
        {
            int n = Random.Range(0, temp.Count);
            return temp[n];
        }
        else
            return null;
    }

    public BaseFloorItem GetItem(int readId)
    {
       foreach (BaseFloorItem item in items)
        {
            if (item.realID == readId)
                return item;
        }
        return null;
    }


    void AddItem(PlayerItem playerItem)
    {
        if(DataHolder.GetItem(playerItem.itemId) != null)
        {
            string url = DataHolder.GetItem(playerItem.itemId).prefabName.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".prefab", "");
            url = DataHolder.Items().GetPrefabPath() + url;
            for (int i = 0; i < playerItem.number; i++)
            {
                GameObject go = Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
                BaseFloorItem item = go.GetComponentInChildren<BaseFloorItem>(true);
                item.itemType = playerItem.itemType;
                item.itemID = playerItem.itemId;
                item.realID = playerItem.realId;
                items.Add(item);
                go.transform.parent = this.transform;
            }
        }       
    }

    public void RemoveItem(int realID)
    {
        foreach(BaseFloorItem item in items){
            if(item.realID == realID)
            {
                RemoveItem(item);
                return;
            }
        }
    }

    void RemoveItem(BaseFloorItem item)
    {

        foreach(CharController pet in GameManager.instance.GetPetObjects())
        {
            if(pet.equipment == item)
            {
                pet.agent.transform.position = item.endPoint.position ;
                pet.OnStop();
            }
        }
       
        items.Remove(item);
        if(item != null)
            Destroy(item.gameObject);
    }

    public int GetItemCount(ItemType type)
    {
        int count = 0;
        foreach (BaseFloorItem item in items)
        {
            if (item.itemType == type)
            {
                count++;
            }
        }
        return count;
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

            float x = Random.Range(gardenBoundX.x + 10, gardenBoundX.y - 10);
            float y = Random.Range(gardenBoundY.x + 3, gardenBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Garden)
        {
            float x = Random.Range(gardenBoundX.x + 10, roomBoundX.x - 10);
            float y = Random.Range(gardenBoundY.x + 3, gardenBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.GardenRight)
        {
            float x = Random.Range(roomBoundX.y + 10,gardenBoundX.y - 10);
            float y = Random.Range(gardenBoundY.x + 3, gardenBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if(type == AreaType.Room)
        {
            float x = Random.Range(roomBoundX.x + 10, roomBoundX.y - 10);
            float y = Random.Range(roomBoundY.x + 3, roomBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Camera)
        {
            float x = Random.Range(cameraBoundX.x, cameraBoundX.y);
            float y = Random.Range(cameraBoundY.x, cameraBoundY.y);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Fly)
        {
            float x = Random.Range(roomBoundX.x, roomBoundX.y);
            float y = Random.Range(roomBoundY.x + 15, roomBoundY.y + 15);
            r = new Vector3(x, y, 0);
        }
        else if (type == AreaType.Wall)
        {
            float x = Random.Range(roomBoundX.x, roomBoundX.y);
            float y = Random.Range(roomWallBoundY.x + 3, roomWallBoundY.y);
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
        if(value >= 0)
            SpawnStar(pos,1);
    }

    public GameObject SpawnGuideArrow(ItemType itemType)
    {
        GameObject go = Instantiate(guidePrefab, GetRandomItem(itemType).transform.GetChild(0).transform.position + new Vector3(0,3,0), Quaternion.identity);
        go.transform.parent = GetRandomItem(itemType).transform.GetChild(0).transform;
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

    public void SpawnPillEffect(CharController pet,float time)
    {
        GameObject go = Instantiate(pillEffectPrefab, pet.transform.position, Quaternion.identity);
        go.transform.parent = pet.transform;
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<AutoDestroy>().liveTime = time;
    }

    public void SpawnBandageEffect(CharController pet, float time)
    {
        GameObject go = Instantiate(bandageEffectPrefab, pet.transform.position, Quaternion.identity);
        go.transform.parent = pet.transform;
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<AutoDestroy>().liveTime = time;
    }

    public void SpawnPetHappy(CharController pet, int value)
    {
        GameObject go = Instantiate(petHappyPrefab, pet.transform.position, Quaternion.identity);
        go.GetComponent<PetHappyItem>().Load(value);
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
        Vector3 pos = GetRandomPoint(AreaType.GardenRight);
        bool isOk = false;
        while (!isOk)
        {
            pos = GetRandomPoint(AreaType.GardenRight);
            isOk = true;
            for(int i = 0; i < chests.Length; i++)
            {
                if (Vector2.Distance(chests[i].transform.position, pos) < 10)
                    isOk = false;
            }
        }
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

        //Find All Food/Drink 
        foreach (BaseFloorItem item in items)
        {
            ItemSaveData data = new ItemSaveData();
            data.id = item.realID;
            if(item.itemType == ItemType.Food || item.itemType == ItemType.Drink)
            {
                data.value = item.GetComponent<EatItem>().foodAmount;
                data.itemType = ItemSaveDataType.Food;
            }else
                data.itemType = ItemSaveDataType.Equipment;

            data.position = item.transform.position;
            GameManager.instance.myPlayer.itemSaveDatas.Add(data);
        }


        FruitItem[] fruits = FindObjectsOfType<FruitItem>();
        for (int i = 0; i < fruits.Length; i++)
        {
            ItemSaveData data = new ItemSaveData();
            data.id = fruits[i].id;
            data.itemType = ItemSaveDataType.Fruit;
            data.value = fruits[i].step;
            data.time = fruits[i].time;
            GameManager.instance.myPlayer.itemSaveDatas.Add(data);
        }

        SaveItemData();
    }


    void SaveItemData(){
        ES2.Save(GetActiveCamera().transform.position, "CameraPosition");
        GameManager.instance.SavePlayer();
    }

    void LoadItemData(){


        PlayerData data;
        if (!GameManager.instance.isGuest)
        {
            data = GameManager.instance.myPlayer;
        }
        else
            data = GameManager.instance.guest;
                
        foreach (ItemSaveData item in data.itemSaveDatas){

            if (float.Parse(GameManager.instance.myPlayer.version) >= 1.20f)
            { 
                if (item.itemType == ItemSaveDataType.Pee)
                {
                    SpawnPee(item.position, item.value);
                }
                else if (item.itemType == ItemSaveDataType.Shit)
                {
                    SpawnShit(item.position, item.value);
                }
                else if (item.itemType == ItemSaveDataType.Food || item.itemType == ItemSaveDataType.Drink)
                {
                    if (GetItem(item.id) != null)
                    {
                        GetItem(item.id).GetComponent<EatItem>().foodAmount = item.value;
                        GetItem(item.id).GetComponent<EatItem>().transform.position = item.position;
                    }
                }
                else if (item.itemType == ItemSaveDataType.Fruit)
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
                else if (item.itemType == ItemSaveDataType.Equipment)
                {
                    if (GetItem(item.id) != null)
                    {
                        GetItem(item.id).transform.position = item.position;
                    }
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
            c = 20;
            h = 60;
        }
        else if (t > 7200)
        {
            c = 15;
            h = 40;
        }
        else if (t > 3600)
        {
            c = 10;
            h = 20;
        }
        else if (t > 1800)
        {
            c = 6;
            h = 10;
        }
        else if (t > 600)
        {
            c = 4;
            h = 4;
        }
        else 
        {
            c = 2;
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

    public void LoadArea()
    {
        if(GameManager.instance.myPlayer.level <= 1)
        {
            gardenBoundX = roomBoundX;
            cameraBoundX = roomBoundX;
        }else if(GameManager.instance.myPlayer.level <= 2)
        {
            gardenBoundX = new Vector2(-50, 150);
            cameraBoundX = new Vector2(-50, 200);
        }else
        {
            gardenBoundX = new Vector2(-270, 150);
            cameraBoundX = new Vector2(-320, 200);
        }
        GetActiveCamera().boundX = cameraBoundX;
        GetActiveCamera().boundY = cameraBoundY;
    }

    #region Pet
    public void LoadPetObject(PlayerPet pet)
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

    IEnumerator SpawnGift(PlayerPet pet)
    {
        pet.isNew = false;
        CharController petObject = SpawnPet(pet);
        petObject.agent.transform.position = new Vector3(10000,00,0);
        GameObject gift = GameObject.Instantiate(petGiftPrefab);
        Vector3 pos = new Vector3(Camera.main.transform.position.x, Random.Range(-15, -10), 0);
        pos.z = pos.y * 10;
        gift.transform.position = pos;
        yield return new WaitForSeconds(5.4f);
        petObject.agent.transform.position = pos;
        petObject.transform.position = pos;
        petObject.OnGift();
        
    }

    CharController SpawnPet(PlayerPet pet)
    {
        
        if (GameManager.instance.GetPetObject(pet.realId) != null)
            return GameManager.instance.GetPetObject(pet.realId);

        Pet p = DataHolder.GetPet(pet.iD);

        if (p == null)
            return null;

        string url = "";
        url = p.prefabName.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".prefab", "");
        url = DataHolder.Pets().GetPrefabPath() + url;
        //Debug.Log(url);
        GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
        CharController character = go.GetComponent<CharController>();
        go.transform.position = ItemManager.instance.GetRandomPoint(AreaType.All);
        character.LoadData(pet);
        character.LoadPrefab();
        GameManager.instance.AddPetObject(character);
        return character;
    }

    public void UnLoadPetObject(CharController p)
    {
        if (p != null && p.agent != null)
            GameObject.Destroy(p.agent.gameObject);
        if (p != null)
            GameObject.Destroy(p.gameObject);
    }


    #endregion
    public void OnSpinWheelPanel()
    {
        UIManager.instance.OnSpinWheelPanel();
    }
}
