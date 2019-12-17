﻿using System.Collections;
using System.Collections.Generic;
using MageApi;
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
    float time = 0;
    float maxTimeCheck = 1;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            GameObject.Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {

        //ES3AutoSaveMgr.Instance.Load();
        bool isLoad = false;
        while(!isLoad){
            if(GameManager.instance.isLoad){
                LoadItems();
                isLoad = true;
            }
            yield return new WaitForEndOfFrame();
        }
        if(GameManager.instance.GetPetObjects().Count == 0){
            GameManager.instance.EquipPets();
        }

        LoadItemData();
 
    }

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeCheck){
            CheckItemData();
            time = 0;
        }else{
            time += Time.deltaTime;
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
            if (isAdd)
            {
                adds.Add(data[i]);
            }
        }

        for (int i = 0; i < adds.Count; i++)
        {
            ItemObject item = AddItem(adds[i]);
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
            if (isAdd)
            {
                adds.Add(data[i]);
            }
        }

        for (int i = 0; i < adds.Count; i++)
        {
            AddItem(adds[i]);
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
        ItemSkill[] skills = GameObject.FindObjectsOfType<ItemSkill>();
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
		GizmoPoint[] points = GameObject.FindObjectsOfType <GizmoPoint> ();
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

    public void SpawnHeart(Vector3 pos,Quaternion rot, int value){
        GameObject go = GameObject.Instantiate(heartPrefab,pos,rot);
        go.GetComponent<HappyItem>().Load(value);
    }

    void CheckItemData(){
       itemSaveDatas.Clear();

       //Find All dirty items
       ItemDirty[] dirties = GameObject.FindObjectsOfType<ItemDirty>();
       for(int i=0;i<dirties.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.itemType = dirties[i].itemType;
           data.value = dirties[i].dirty;
           data.position = dirties[i].transform.position;
           itemSaveDatas.Add(data);
       }

        //Find All Food/Drink Item
        EatItem[] eats = GameObject.FindObjectsOfType<EatItem>();
        for(int i=0;i<eats.Length;i++){
           ItemSaveData data = new ItemSaveData();
           data.id = eats[i].item.itemID;
           data.itemType = eats[i].itemSaveDataType;
           data.value = eats[i].foodAmount;
           data.position = eats[i].transform.position;
           itemSaveDatas.Add(data);
       }

       HappyItem[] happies = GameObject.FindObjectsOfType<HappyItem>();
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
       if(ES2.Exists("ItemSaveData")){
            itemSaveDatas = ES2.LoadList<ItemSaveData>("ItemSaveData");
        }

        foreach(ItemSaveData item in itemSaveDatas){
            if(item.itemType == ItemSaveDataType.Pee){
                SpawnPee(item.position,item.value);
            }else if(item.itemType == ItemSaveDataType.Shit){
                SpawnShit(item.position,item.value);
            }else if(item.itemType == ItemSaveDataType.Food || item.itemType == ItemSaveDataType.Drink){
                GetItem(item.id).GetComponentInChildren<EatItem>().foodAmount = item.value;
                GetItem(item.id).GetComponentInChildren<EatItem>().transform.position = item.position;
            }else if(item.itemType == ItemSaveDataType.Happy){
                SpawnHeart(item.position,item.rotation,(int)item.value);
            }
        }


    }

}
