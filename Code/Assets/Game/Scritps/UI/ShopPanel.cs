using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public ScrollRect scroll;
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

        if(currentTab > toggles.Count)
            currentTab = 0;
        
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
        MageManager.instance.PlaySoundName("BubbleButton",false);
        currentTab = id;

        ES2.Save(id,"ShopToggle");
        ClearItems();

        List<Item> items = new List<Item>();
        List<Pet> pets = new List<Pet>();

        if(currentTab == 1){
            for(int i=0;i<DataHolder.Pets().GetDataCount();i++){
                pets.Add(DataHolder.Pet(i));
            }   
        }else if(currentTab == 0){
            for(int i=0;i<DataHolder.Items().GetDataCount();i++){
                if((int)DataHolder.Item(i).itemType == 0 || (int)DataHolder.Item(i).itemType == 1){
                    items.Add(DataHolder.Item(i));
                }   
            }               
        }
        else if(currentTab == 2){
            for(int i=0;i<DataHolder.Items().GetDataCount();i++){
                if((int)DataHolder.Item(i).itemType == (int)ItemType.Food || (int)DataHolder.Item(i).itemType == (int)ItemType.Drink){
                    items.Add(DataHolder.Item(i));
                }   
            }   
        }else if(currentTab == 3){
            for(int i=0;i<DataHolder.Items().GetDataCount();i++){
                if((int)DataHolder.Item(i).itemType == (int)ItemType.Bath || (int)DataHolder.Item(i).itemType == (int)ItemType.Bed
                || (int)DataHolder.Item(i).itemType == (int)ItemType.Clean || (int)DataHolder.Item(i).itemType == (int)ItemType.Clock
                || (int)DataHolder.Item(i).itemType == (int)ItemType.MedicineBox || (int)DataHolder.Item(i).itemType == (int)ItemType.Picture
                || (int)DataHolder.Item(i).itemType == (int)ItemType.Table || (int)DataHolder.Item(i).itemType == (int)ItemType.Toilet
                || (int)DataHolder.Item(i).itemType == (int)ItemType.Room){
                    items.Add(DataHolder.Item(i));
                }   
            }  
        }else if(currentTab == 4){
            for(int i=0;i<DataHolder.Items().GetDataCount();i++){
                if((int)DataHolder.Item(i).itemType == (int)ItemType.Toy){
                    items.Add(DataHolder.Item(i));
                }   
            }   
        }




        
 
        if(currentTab == 1){
            pets.Sort((p1,p2)=>(p1.shopOrder).CompareTo(p2.shopOrder));
            foreach (var item in pets)
            {
                LoadItem(item);
            }            
        }else{
            items.Sort((p1,p2)=>(p1.shopOrder).CompareTo(p2.shopOrder));
            foreach(Item item in items){
                LoadItem(item);
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

    public void OnLeft(){
        if(scroll.horizontalNormalizedPosition > 0)
            scroll.horizontalNormalizedPosition -= 0.1f; 
    }

    public void OnRight(){
        if(scroll.horizontalNormalizedPosition < 1)
            scroll.horizontalNormalizedPosition += 0.1f; 
    }

}

