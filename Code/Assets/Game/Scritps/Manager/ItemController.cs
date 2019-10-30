﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
	public static ItemController instance;
	[HideInInspector]
	public CharController character;
	public FoodBowlItem foodBowl;
	public FoodBowlItem waterBowl;

	public List<ItemObject> items = new List<ItemObject>();

	void Awake()
	{
		if (instance == null)
			instance = this;

		character = GameObject.FindObjectOfType<CharController> ();
	}
    // Start is called before the first frame update
    void Start()
    {
		LoadItems();
    }

	public void LoadItems(){
		List<int> data = ApiManager.instance.GetUsedItems();
		for(int i=0;i<data.Count;i++){
			//Debug.Log(data[i]);
			AddItem(data[i]);
		}
	}

	void AddItem(int itemId){
		string url = DataHolder.GetItem(itemId).prefabName.Replace("Assets/Game/Resources/","");
		url = url.Replace(".prefab",""); 
		url = DataHolder.Items().GetPrefabPath() + url;
		//Debug.Log(url);
		GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject),Vector3.zero,Quaternion.identity) as GameObject;		
		ItemObject item = go.AddComponent<ItemObject>();
		item.itemType = DataHolder.GetItem(itemId).itemType;
		items.Add(item);
		go.transform.parent = this.transform;	
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public BathTubeItem GetBathTubeItem(){
		return GameObject.FindObjectOfType<BathTubeItem>();
	}

	public void UseItem(int itemId){
		for(int i=0;i<items.Count;i++){
			if(items[i].itemType == DataHolder.GetItem(itemId).itemType){
				GameObject.Destroy(items[i].gameObject);
				items.Remove(items[i]);
				break;
			}	
		}
		AddItem(itemId);
	}
}
