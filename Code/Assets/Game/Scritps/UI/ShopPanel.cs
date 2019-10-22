using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    public Transform anchor;
    List<ItemUI> items = new List<ItemUI>();
    public GameObject itemUIPrefab;
    public ItemCategory category;
    // Start is called before the first frame update
    void Start()
    {
        OnTab(0);
    }

    void Load(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTab(int id){
        category = (ItemCategory)id;
        ClearItems();
        for(int i=0;i<GameManager.instance.datas.Length;i++){
            if(GameManager.instance.datas[i].itemCategory == category){
                LoadItem(GameManager.instance.datas[i]);
            }
        }
    }

    void LoadItem(ItemData data){
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

