using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public Transform anchor;
    List<ItemUI> items = new List<ItemUI>();
    public GameObject itemUIPrefab;
    public int currentTab;
    // Start is called before the first frame update
    void Start()
    {
        OnTab(currentTab);
    }

    void Load(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTab(int id){
        currentTab = id;
        ClearItems();
        for(int i=0;i<DataHolder.Items().GetDataCount();i++){
            if(currentTab == 0)
            {
                if(DataHolder.Item(i).itemType == ItemType.Coin || DataHolder.Item(i).itemType == ItemType.Diamond){
                    LoadItem(DataHolder.Item(i));
                }
            }else if(currentTab == 1){
                 if(DataHolder.Item(i).itemType == ItemType.Toy){
                    LoadItem(DataHolder.Item(i));
                }               
            }else if(currentTab == 2){
                 if(DataHolder.Item(i).itemType == ItemType.Dog){
                    LoadItem(DataHolder.Item(i));
                }               
            }else if(currentTab == 3){
                 if(DataHolder.Item(i).itemType == ItemType.Room){
                    LoadItem(DataHolder.Item(i));
                }               
            }else if(currentTab == 4){
                 if(DataHolder.Item(i).itemType == ItemType.Bed){
                    LoadItem(DataHolder.Item(i));
                }               
            }else if(currentTab == 5){
                 if(DataHolder.Item(i).itemType == ItemType.Bath || DataHolder.Item(i).itemType == ItemType.Toilet || DataHolder.Item(i).itemType == ItemType.Cleaner){
                    LoadItem(DataHolder.Item(i));
                }               
            }else if(currentTab == 6){
                 if(DataHolder.Item(i).itemType == ItemType.Picture || DataHolder.Item(i).itemType == ItemType.Table){
                    LoadItem(DataHolder.Item(i));
                }               
            }

        }
    }

    void LoadItem(Item data){
        GameObject go = GameObject.Instantiate(itemUIPrefab);
       
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        ItemUI item = go.GetComponent<ItemUI>();
        items.Add(item);
        item.Load(data);
    }

    void ClearItems(){
        foreach(ItemUI item in items){
            GameObject.Destroy(item.gameObject);
        }
        items.Clear();
    }

}

