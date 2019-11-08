using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
	public static ItemController instance;

	public List<ItemObject> items = new List<ItemObject>();
	public List<CharController> pets = new List<CharController>();
	
	public CharAge age = CharAge.Big;
	void Awake()
	{
		if (instance == null)
			instance = this;
	}
    // Start is called before the first frame update
    void Start()
    {
        ApiManager.instance.AddItem(56);
        ApiManager.instance.UseItem(56);
        LoadItems();
    }

	public void LoadItems(){
		List<int> data = ApiManager.instance.GetEquipedItems();
		for(int i=0;i<data.Count;i++){
			Debug.Log(data[i]);
			AddItem(data[i]);
		}
	}

	void AddItem(int itemId){
		string url = DataHolder.GetItem(itemId).prefabName.Replace("Assets/Game/Resources/","");
		url = url.Replace(".prefab",""); 
		url = DataHolder.Items().GetPrefabPath() + url;
		GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject),Vector3.zero,Quaternion.identity) as GameObject;		
		ItemObject item = go.AddComponent<ItemObject>();
		item.itemType = DataHolder.GetItem(itemId).itemType;
		items.Add(item);
		go.transform.parent = this.transform;	
	}


	public void LoadPets(){
		List<int> data = ApiManager.instance.GetEquipedPets();
		for(int i=0;i<data.Count;i++){
			Debug.Log(data[i]);
			AddPet(data[i]);
		}
	}

	void AddPet(int itemId){

		string url = "";
		if(age == CharAge.Big)
		{
			url = DataHolder.GetPet(itemId).petBig.Replace("Assets/Game/Resources/","");
		}else if(age == CharAge.Middle){
			url = DataHolder.GetPet(itemId).petMiddle.Replace("Assets/Game/Resources/","");
		}else if(age == CharAge.Small){
			url = DataHolder.GetPet(itemId).petSmall.Replace("Assets/Game/Resources/","");
		}

		url = url.Replace(".prefab",""); 
		url = DataHolder.Pets().GetPrefabPath() + url;
		GameObject go = GameObject.Instantiate((Resources.Load(url) as GameObject),Vector3.zero,Quaternion.identity) as GameObject;		
		CharController pet = go.GetComponentInChildren<CharController>();
		pets.Add(pet);
		go.transform.parent = this.transform;	
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public BathTubeItem GetBathTubeItem(){
		return GameObject.FindObjectOfType<BathTubeItem>();
	}

	public FoodBowlItem FoodItem(){
		return GameObject.FindObjectOfType<FoodBowlItem>();
	}

	public DrinkBowlItem DrinkItem(){
		return GameObject.FindObjectOfType<DrinkBowlItem>();
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

	public void UsePet(int itemId){
		AddPet(itemId);
	}
}
