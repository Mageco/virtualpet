using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public Transform anchor;
    List<InventoryUI> items = new List<InventoryUI>();
    public GameObject itemUIPrefabs;
   
    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    void Load(){
        ClearItems();
        List<int> itemData = ApiManager.instance.GetBuyItems();
        for(int i=0;i<itemData.Count;i++){
            if(DataHolder.GetItem(itemData[i]).itemType == ItemType.Food || DataHolder.GetItem(itemData[i]).itemType == ItemType.Drink
            || DataHolder.GetItem(itemData[i]).itemType == ItemType.Toy){
                LoadItem(DataHolder.GetItem(itemData[i]));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(UIManager.instance.notification == NotificationType.Shop){
            Load();
            UIManager.instance.notification = NotificationType.None;
        }
    }

    

    void LoadItem(Item data){
        GameObject go = GameObject.Instantiate(itemUIPrefabs);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        InventoryUI item = go.GetComponent<InventoryUI>();
        items.Add(item);
        item.Load(data);
    }

    void ClearItems(){
        foreach(InventoryUI s in items){
            GameObject.Destroy(s.gameObject);
        }
        items.Clear();
    }

}

