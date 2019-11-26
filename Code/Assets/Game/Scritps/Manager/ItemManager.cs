using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public List<ItemObject> items = new List<ItemObject>();
    public float expireTime = 10;


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
        bool isLoad = false;
        while(!isLoad){
            if(GameManager.instance.isLoad){
                LoadItems();
                isLoad = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipItem()
    {
        StartCoroutine(EquipItemCoroutine());
    }

    IEnumerator EquipItemCoroutine()
    {
        List<int> data =ApiManager.GetInstance().GetEquipedItems();
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
            GameManager.instance.SetCameraTarget(item.transform.GetChild(0).gameObject);
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
            GameManager.instance.SetCameraTarget(item.transform.GetChild(0).gameObject);
        }
        yield return new WaitForSeconds(2);
        GameManager.instance.ResetCameraTarget();
    }

    public void LoadItems()
    {
        List<int> data =ApiManager.GetInstance().GetEquipedItems();
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
                Debug.Log(item.name);
                return item.transform.GetChild(0).gameObject;
            }
        }
        return null;
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
            Debug.Log(skills[i].skillType);
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
}
