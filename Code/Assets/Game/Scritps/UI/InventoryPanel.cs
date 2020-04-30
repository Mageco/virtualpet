using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    ItemType itemType = ItemType.All;
    public Transform anchor;
    List<InventoryUI> items = new List<InventoryUI>();
    public GameObject InventoryUIPrefab;
    int currentTab = -1;

    // Start is called before the first frame update

    void Awake()
    {

    }
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }



    public void Load()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        itemType = (ItemType)currentTab;
        ClearItems();

        List<PlayerItem> temp = new List<PlayerItem>();

        foreach(PlayerItem item in GameManager.instance.myPlayer.items)
        {
            if(item.state != ItemState.OnShop)
            {
                if (itemType == ItemType.All)
                {
                    temp.Add(item);
                }
                else if (item.itemType == itemType)
                {
                    temp.Add(item);
                }
            }
        }

        temp.Sort((p1, p2) => ((int)p1.itemType).CompareTo((int)p2.itemType));

        foreach(PlayerItem item in temp)
        {
            LoadItem(item);
        }
    }

    public InventoryUI GetItem(int id)
    {
        foreach (InventoryUI item in items)
        {
            if (item.realId == id)
                return item;
        }
        return null;
    }

    void LoadItem(PlayerItem data)
    {
        GameObject go = Instantiate(InventoryUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        InventoryUI item = go.GetComponent<InventoryUI>();
        items.Add(item);
        item.Load(data);
    }


    void ClearItems()
    {
        foreach (InventoryUI item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
