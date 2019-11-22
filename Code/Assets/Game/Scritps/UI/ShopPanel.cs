using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public Transform anchor;
    List<ItemUI> items = new List<ItemUI>();
    public GameObject itemUIPrefab;
    int currentTab = 0;
    List<Toggle> toggles = new List<Toggle>();
    public Transform toogleAnchor;
    
    // Start is called before the first frame update

    void Awake(){
        if(ES2.Exists("ShopToggle")){
            currentTab = ES2.Load<int>("ShopToggle");
        }

    }
    void Start()
    {
        for(int i=0;i<toogleAnchor.childCount;i++){
            int id = i;
            Toggle t = toogleAnchor.GetChild(i).GetComponent<Toggle>();
            toggles.Add(t);
            t.onValueChanged.AddListener (delegate {OnTab(id);});
        }
        
        if(toggles[currentTab].isOn){
            OnTab(currentTab);
        }else{
            toggles[currentTab].isOn = true; 
        }
           
    }

    public void ReLoad(){
        OnTab(currentTab);
    }

    public void ReLoadTab(int id){
        currentTab = id;
        OnTab(currentTab);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTab(int id){
        currentTab = id;

        ES2.Save(id,"ShopToggle");
        ClearItems();

        if(currentTab == 1){
            for (int i = 0; i < DataHolder.Pets().GetDataCount(); i++)
            {
                LoadItem(DataHolder.Pet(i));
            }
        }
        else{
            for(int i=0;i<DataHolder.Items().GetDataCount();i++){
                if((int)DataHolder.Item(i).itemType == currentTab){
                    LoadItem(DataHolder.Item(i));      
                }   
            }   
        }

        
    }

    void LoadItem(Item data){
        GameObject go = Instantiate(itemUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        ItemUI item = go.GetComponent<ItemUI>();
        items.Add(item);
        item.Load(data);
    }

    void LoadItem(Pet data){
        GameObject go = Instantiate(itemUIPrefab);
       
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        ItemUI item = go.GetComponent<ItemUI>();
        items.Add(item);
        item.Load(data);
    }

    void ClearItems(){
        foreach(ItemUI item in items){
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }

}

