using System.Collections;
using System.Collections.Generic;
using MageApi;
using MageSDK.Client;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public List<ItemObject> items = new List<ItemObject>();
    public List<ItemSaveData> itemSaveDatas = new List<ItemSaveData>();

    public ItemCollider[] itemColliders;
    public float expireTime = 10;

    public GameObject peePrefab;
    public GameObject shitPrefab;
    public GameObject heartPrefab; 

    public GameObject dirtyPrefab;
    public GameObject healthEffectPrefab;
    public GameObject guidePrefab;
    float time = 0;
    float maxTimeCheck = 1;
    System.DateTime playTime = System.DateTime.Now;

    

    float timeDirty = 0;
    float maxTimeDirty = 200;


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
        //ES3AutoSaveMgr.Instance.Load();
        bool isLoad = false;
        while(!isLoad){
            if(GameManager.instance.isLoad){
                LoadItems();
                GameManager.instance.EquipPets();
                GameManager.instance.LoadPetObjects();
                isLoad = true;
            }
            yield return new WaitForEndOfFrame();
        }
        LoadItemData();

        if (ES2.Exists("PlayTime"))
        {
            playTime = ES2.Load<System.DateTime>("PlayTime");
            LoadPetData((float)(System.DateTime.Now - playTime).TotalSeconds);
        }

        if (!ES2.Exists("RateUs") && GameManager.instance.gameTime > 720 && GameManager.instance.rateCount % 5 == 0)
        {
            UIManager.instance.OnRatingPopup();
           
        }
        GameManager.instance.rateCount++;

    }

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeCheck){
            CheckItemData();
            time = 0;
            playTime = System.DateTime.Now;
            ES2.Save(playTime, "PlayTime");

            //Check Notification
            if (GameManager.instance.IsCollectAchivement())
            {
                UIManager.instance.achivementNotification.SetActive(true);
            }else
                UIManager.instance.achivementNotification.SetActive(false);

            //Check Event Notification
            if(GameManager.instance.GetHappy() >= Mathf.Min(GameManager.instance.myPlayer.minigameLevels[0], 10))
            {
                UIManager.instance.eventNotification.SetActive(true);
            }else
                UIManager.instance.eventNotification.SetActive(false);
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
    }

    public void EquipItem()
    {
        StartCoroutine(EquipItemCoroutine());
        UpdateItemColliders();
        
    }

    IEnumerator EquipItemCoroutine()
    {
        List<int> data = GameManager.instance.GetEquipedItems();
        
        List<ItemObject> removes = new List<ItemObject>();

        
        foreach (ItemObject item in items)
        {
            bool isRemove = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == item.itemID)
                {
                    isRemove = false;
                }
            }
            if (isRemove)
                removes.Add(item);
        }

        foreach (ItemObject item in removes)
        {
            //GameManager.instance.SetCameraTarget(item.transform.GetChild(0).gameObject);
            for(int i=0;i<item.transform.childCount;i++){
                Animator anim = item.transform.GetChild(i).GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("Disappear", 0);
                }
            }
            yield return new WaitForSeconds(1);
            RemoveItem(item);
        }


        List<int> adds = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            bool isAdd = true;
            foreach (ItemObject item in items)
            {
                if (data[i] == item.itemID)
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
            ItemObject item = AddItem(adds[i]);
            MageManager.instance.PlaySoundName("Item_Appear",false);
            for (int j = 0; j < item.transform.childCount; j++)
            {
                Animator anim = item.transform.GetChild(j).GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("Appear", 0);
                }
            }
            //GameManager.instance.SetCameraTarget(item.transform.GetChild(0).gameObject);
        }
        
        yield return new WaitForSeconds(2);
        //GameManager.instance.ResetCameraTarget();
        
    }

    public void LoadItems()
    {
        List<int> data = GameManager.instance.GetEquipedItems();
        Debug.Log("Data " + data.Count);
        List<ItemObject> removes = new List<ItemObject>();

        foreach (ItemObject item in items)
        {
            bool isRemove = true;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] == item.itemID)
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


        List<int> adds = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            bool isAdd = true;
            foreach (ItemObject item in items)
            {
                if (data[i] == item.itemID)
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
            ItemObject item = AddItem(adds[i]);

            for (int j = 0; j < item.transform.childCount; j++)
            {
                Animator anim = item.transform.GetChild(j).GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("Idle", 0);
                }
            }
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


    ItemObject AddItem(int itemId)
    {
       
        string url = DataHolder.GetItem(itemId).prefabName.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".prefab", "");
        url = DataHolder.Items().GetPrefabPath() + url;
        GameObject go = Instantiate((Resources.Load(url) as GameObject), Vector3.zero, Quaternion.identity) as GameObject;
        ItemObject item = go.AddComponent<ItemObject>();
        item.itemType = DataHolder.GetItem(itemId).itemType;
        item.itemID = itemId;
        items.Add(item);
        go.transform.parent = this.transform;
        Debug.Log(DataHolder.GetItem(itemId).GetName(0));

        return item;
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

    public GameObject GetItem(ItemType type){
        foreach(ItemObject item in items){
            if(DataHolder.GetItem(item.itemID).itemType == type){
                return item.gameObject;
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

    void UpdateItemColliders(){
        itemColliders = this.GetComponentsInChildren<ItemCollider>();
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


    #region Skill

    public void ActivateSkillItems(SkillType type){
        Debug.Log(type);
        List<ItemSkill> itemSkills = GetSkillItem(type);
        foreach(ItemSkill s in itemSkills){
            
            s.OnActive(expireTime);
        }
    }

    public void DeActivateSkillItems(SkillType type){
        List<ItemSkill> itemSkills = GetSkillItem(type);
        foreach(ItemSkill s in itemSkills){
            s.DeActive();
        }
    }

    List<ItemSkill> GetSkillItem(SkillType type){
        List<ItemSkill> itemSkills = new List<ItemSkill>();
        ItemSkill[] skills = FindObjectsOfType<ItemSkill>();
        for(int i=0;i<skills.Length;i++){
            if(skills[i].skillType == type){
                itemSkills.Add(skills[i]);
            }
        }
        return itemSkills;
    }

    public void SetExpireSkillTime(float t){
        expireTime = t;
    }

    public void ResetExpireSkillTime(){
        expireTime = 10;
    }

    #endregion

    List<GizmoPoint> GetPoints(PointType type)
	{
		List<GizmoPoint> temp = new List<GizmoPoint>();
		GizmoPoint[] points = FindObjectsOfType <GizmoPoint> ();
		for(int i=0;i<points.Length;i++)
		{
			if(points[i].type == type)
				temp.Add(points[i]);
		}
		return temp;
	}

	public Transform GetRandomPoint(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		if(points != null && points.Count > 0){
			int id = Random.Range (0, points.Count);
			return points [id].transform;
		}else
			return null;

	}

	public List<Transform> GetRandomPoints(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		List<Transform> randomPoints = new List<Transform> ();
		for (int i = 0; i < points.Count; i++) {
			randomPoints.Add (points [i].transform);
		}

		for (int i = 0; i < randomPoints.Count; i++) {
			if (i < randomPoints.Count - 1) {
				int j = Random.Range (i, randomPoints.Count);
				Transform temp = randomPoints [i];
				randomPoints [i] = randomPoints [j];
				randomPoints [j] = temp;
			}
		}
		return randomPoints;
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

    public void SpawnHeart(Vector3 pos,Quaternion rot, int value,bool isSound){
        GameObject go = Instantiate(heartPrefab,pos,rot);
        go.GetComponent<HappyItem>().Load(value,isSound);
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
        for (int i = 0; i < n; i++)
        {
            int ran = Random.Range(0, 100);
            Quaternion rot = Quaternion.identity;
            if (ran > 50)
                rot = Quaternion.Euler(new Vector3(0, 180, -1));
            Vector3 pos = p + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-1, 1), 0);
            SpawnHeart(pos, rot, 1, true);
        }
    }

    public void SpawnHealth(Vector3 pos)
    {
        GameObject go = Instantiate(healthEffectPrefab, pos, Quaternion.identity);
    }

    public void SpawnDirty()
    {
        Vector3 pos = GetRandomPoint(PointType.Patrol).position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        pos.z = 990;
        GameObject go = Instantiate(dirtyPrefab, pos, Quaternion.identity);
    }

    void CheckItemData(){
       itemSaveDatas.Clear();

       //Find All dirty items
       ItemDirty[] dirties = FindObjectsOfType<ItemDirty>();
       for(int i=0;i<dirties.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.itemType = dirties[i].itemType;
           data.value = dirties[i].dirty;
           data.position = dirties[i].transform.position;
           itemSaveDatas.Add(data);
       }

        //Find All Food/Drink Item
        EatItem[] eats = FindObjectsOfType<EatItem>();
        for(int i=0;i<eats.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.id = eats[i].item.itemID;
           data.itemType = eats[i].itemSaveDataType;
           data.value = eats[i].foodAmount;
           data.position = eats[i].transform.position;
           itemSaveDatas.Add(data);
       }

       HappyItem[] happies = FindObjectsOfType<HappyItem>();
       for(int i=0;i<happies.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.itemType = happies[i].itemSaveDataType;
           data.value = happies[i].value;
           data.position = happies[i].transform.position;
           itemSaveDatas.Add(data);
       }
       
       SaveItemData();
    }


    void SaveItemData(){
        ES2.Save(itemSaveDatas,"ItemSaveData");
    }

    void LoadItemData(){

    #if UNITY_EDITOR
        if (MageEngine.instance.resetUserDataOnStart)
            return;
    #endif
        if(ES2.Exists("ItemSaveData")){
            itemSaveDatas = ES2.LoadList<ItemSaveData>("ItemSaveData");
        }

        foreach(ItemSaveData item in itemSaveDatas){
            if(item.itemType == ItemSaveDataType.Pee){
                SpawnPee(item.position,item.value);
            }else if(item.itemType == ItemSaveDataType.Shit){
                SpawnShit(item.position,item.value);
            }else if(item.itemType == ItemSaveDataType.Food || item.itemType == ItemSaveDataType.Drink){
                if(GetItem(item.id) != null){
                    GetItem(item.id).GetComponentInChildren<EatItem>().foodAmount = item.value;
                    GetItem(item.id).GetComponentInChildren<EatItem>().transform.position = item.position;
                }
            }else if(item.itemType == ItemSaveDataType.Happy){
                SpawnHeart(item.position,item.rotation,(int)item.value,false);
            }
        }
    }

    public void LoadPetData(float t)
    {
        //Debug.Log(t);
        if (t > 86400)
        {
            t = 7272 + (t - 86400) / 100;
        }
        else if (t > 28800)
        {
            t = 6120 + (t - 28800) / 50;
        }
        else if (t > 3600)
        {
            t = 3600 + (t-3600)/10;
        }
        

        int petNumber = GameManager.instance.GetPetObjects().Count;
        int peeNumber = (int)Mathf.Clamp(t / 7200 * petNumber, 0, 5);
        Debug.Log("Pee Number " + peeNumber);
        int shitNumber = (int)Mathf.Clamp(t / 14400 * petNumber, 0, 5);
        int dirtyNumber = (int)Mathf.Clamp(t / 14400, 0, 3);
        
        foreach (CharController p in GameManager.instance.GetPetObjects())
        {
            p.LoadTime(t);
        }

        for (int i = 0; i < dirtyNumber; i++)
        {
            SpawnDirty();
        }

        for (int i = 0; i < peeNumber; i++)
        {
            SpawnPee(GetRandomPoint(PointType.Patrol).position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0), Random.Range(50,100));
        }

        for (int i = 0; i < shitNumber; i++)
        {
            SpawnShit(GetRandomPoint(PointType.Patrol).position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0), Random.Range(50, 100));
        }


        if (GetItem(ItemType.Food) != null && GetItem(ItemType.Food).GetComponent<EatItem>() != null)
        {
            Debug.Log(GetItem(ItemType.Food).GetComponent<EatItem>().foodAmount);
            int n = Mathf.Min((int)(0.01f * t * petNumber / 50), (int)GetItem(ItemType.Food).GetComponent<EatItem>().foodAmount / 50);
            GetItem(ItemType.Food).GetComponent<FoodBowlItem>().Eat(0.01f * t * petNumber);
            
            for (int i = 0; i < n; i++)
            {
                int ran = Random.Range(0, 100);
                Quaternion rot = Quaternion.identity;
                if (ran > 50)
                    rot = Quaternion.Euler(new Vector3(0, 180, -1));
                Vector3 pos = GetItem(ItemType.Food).transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
                ItemManager.instance.SpawnHeart(pos, rot, 1, true);
            }
        }

        if (GetItem(ItemType.Drink) != null && GetItem(ItemType.Drink).GetComponent<EatItem>() != null)
        {
            int n = Mathf.Min((int)(0.01f * t * petNumber / 50), (int)GetItem(ItemType.Drink).GetComponent<EatItem>().foodAmount / 50);
            GetItem(ItemType.Drink).GetComponent<DrinkBowlItem>().Eat(0.01f * t * petNumber);
            for (int i = 0; i < n; i++)
            {
                int ran = Random.Range(0, 100);
                Quaternion rot = Quaternion.identity;
                if (ran > 50)
                    rot = Quaternion.Euler(new Vector3(0, 180, -1));
                Vector3 pos = GetItem(ItemType.Drink).transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
                ItemManager.instance.SpawnHeart(pos, rot, 1, true);
            }
        }

        



    }

}
