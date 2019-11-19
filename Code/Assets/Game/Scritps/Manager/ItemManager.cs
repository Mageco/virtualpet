using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    public List<ItemObject> items = new List<ItemObject>();
    public List<ItemSkill> itemSkills = new List<ItemSkill>();


    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            GameObject.Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
        List<int> data = ApiManager.instance.GetEquipedItems();
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
            GameManager.instance.camera.SetTarget(item.transform.GetChild(0).gameObject);
            Animator anim = item.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("Disaapear", 0);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }
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
            Animator anim = item.GetComponent<Animator>();
            if (anim != null)
            {
                GameManager.instance.camera.SetTarget(item.transform.GetChild(0).gameObject);
                anim.Play("Appear", 0);
                yield return new WaitForEndOfFrame();
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }

            yield return new WaitForSeconds(1);
            GameManager.instance.camera.SetTarget( GameManager.instance.petObjects[0].gameObject);

        }
    }

    public void LoadItems()
    {
        List<int> data = ApiManager.instance.GetEquipedItems();
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
        Destroy(item.gameObject);
    }
}
