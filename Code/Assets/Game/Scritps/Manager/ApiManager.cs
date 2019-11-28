using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MageApi.Models;
using MageApi;
using MageApi.Models.Request;
using MageApi.Models.Response;
using UnityEngine.UI;
using Mage.Models.Users;
using Mage.Models.Game;
using System.IO;
using Mage.Models.Application;
using MageSDK.Client;

public class ApiManager : MageEngine {

	private static ApiManager instance;
	[HideInInspector]
	public int testRound = 0;
	[HideInInspector]
	public int testSequence = 0;
	
	[HideInInspector]
	public string contactUrl = "";
	[HideInInspector]
	public string contactPhone = "";
	[HideInInspector]
	public int option = 0;


	protected override void Load()
	{
		if (instance == null)
			instance = this;
		else
			Destroy (this.gameObject);

		DontDestroyOnLoad (this.gameObject);
	}

	protected override void OnLoginCompleteCallback() {
		if(GameManager.instance != null)
			GameManager.instance.Start_UsingCallback();
	}

	public static ApiManager GetInstance() {
		return instance;
	}

	protected void Start() {
		Debug.Log("ApiManager: User = " + GetUser().ToJson());
	}


	#region Player Data
	public int GetCoin()
	{
		return GetUserDataInteger("Coin");
	}

	public void AddCoin(int c)
	{
		int c1 = GetCoin();
		if (c1 + c >= 0) {
			c1 += c;
			UpdateUserData (new UserData ("Coin", c1.ToString (), ""));

		} else {
			Debug.Log ("Not Enough Coin");
		}

	}

	public int GetDiamond()
	{
		return GetUserDataInteger("Diamond");
	}

	public void AddDiamond(int c)
	{
		int c1 = GetDiamond();
		if (c1 + c >= 0) {
			c1 += c;
			UpdateUserData (new UserData ("Diamond", c1.ToString (), ""));
		} else {
			Debug.Log ("Not Enough Diamond");
		}

	}
	#endregion


	#region Item
	public bool BuyItem(int itemId)
	{
		PriceType type = DataHolder.GetItem(itemId).priceType;
		int price = DataHolder.GetItem(itemId).buyPrice;
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
				MageManager.instance.OnNotificationPopup ("You have not enough Coin");
				return false;
			}
			AddCoin (-price);
			AddItem (itemId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
				MageManager.instance.OnNotificationPopup ("You have not enough Coin");
				return false;
			}
			AddDiamond (-price);
			AddItem (itemId);
			return true;
		}else
		{
			return false;
		}
	}


	public void AddItem(int itemId)
	{
		UpdateUserData (new UserData (itemId.ToString(),ItemState.Have.ToString(), "Item"));
	}

	public void RemoveItem(int itemId)
	{
		UpdateUserData (new UserData (itemId.ToString(),ItemState.OnShop.ToString(), "Item"));
	}

	public void EquipItem(int itemId){
		//if(HaveItem(itemId)){
			List<int> items = GetEquipedItems();
			List<UserData> itemDatas = new List<UserData>();
			for(int i=0;i<items.Count;i++){
				if(DataHolder.GetItem(items[i]).itemType == DataHolder.GetItem(itemId).itemType && DataHolder.GetItem(items[i]).iD != itemId){
					itemDatas.Add(new UserData (DataHolder.GetItem(items[i]).iD.ToString(), ItemState.Have.ToString(), "Item"));
				}
			}
			itemDatas.Add(new UserData (itemId.ToString(), ItemState.Equiped.ToString(), "Item"));
			UpdateUserData (itemDatas);
		//}
	}

	public bool IsHaveItem(int itemId)
	{
		if (GetUser().GetUserData (itemId.ToString()) == ItemState.Have.ToString() || GetUser().GetUserData (itemId.ToString()) == ItemState.Equiped.ToString())
			return true;
		else
			return false;
	}

	public bool IsEquipItem(int itemId)
	{
		if (GetUser().GetUserData (itemId.ToString()) == ItemState.Equiped.ToString())
			return true;
		else
			return false;
	}

	public List<int> GetBuyItems(){
		List<int> items = new List<int>();
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(IsHaveItem(DataHolder.Item(i).iD)){
				items.Add(DataHolder.Item(i).iD);
			}
		}
		return items;
	}

	public List<int> GetEquipedItems(){
		List<int> items = new List<int>();
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(IsEquipItem(DataHolder.Item(i).iD)){
				items.Add(DataHolder.Item(i).iD);
			}
		}
		return items;
	}
	#endregion

	#region Character
	public bool BuyPet(int petId)
	{
		PriceType type = DataHolder.GetPet(petId).priceType;
		int price = DataHolder.GetPet(petId).buyPrice;
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
				return false;
			}
			AddCoin (-price);
			AddPet (petId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
				return false;
			}
			AddDiamond (-price);
			AddPet (petId);
			return true;
		}else
		{
			return false;
		}
	}


	public void AddPet(int petId)
	{
		Character c = SetCharacter (new Character() {
			id = petId.ToString(),
			character_name = "New Pet",
			character_type = "0",
			status = ItemState.Have.ToString()
		});

		Debug.Log("Character Added: " + c.ToJson());
	}

	public void EquipPet(int petId){
		Character c = SetCharacter (new Character() {
			id = petId.ToString(),
			character_name = "New Pet",
			character_type = "0",
			status = ItemState.Equiped.ToString()
		});
		Debug.Log("Character Equiped: " + c.ToJson());
	}

	public bool IsHavePet(int petId)
	{
		if (null == GetCharacter(petId.ToString())) {
			return false;
		}
		if (GetCharacter(petId.ToString()).status == ItemState.Have.ToString() || GetCharacter(petId.ToString()).status == ItemState.Equiped.ToString() )
			return true;
		else
			return false;
	}

	public bool IsEquipPet(int petId)
	{
		if (null != GetCharacter(petId.ToString()) && GetCharacter(petId.ToString()).status == ItemState.Equiped.ToString())
			return true;
		else
			return false;
	}

	public List<int> GetBuyPets(){
		List<int> pets = new List<int>();
		for(int i=0;i<DataHolder.Pets().GetDataCount();i++){
			if(IsHavePet(DataHolder.Pet(i).iD)){
				pets.Add(DataHolder.Pet(i).iD);
			}
		}
		return pets;
	}

	public List<int> GetEquipedPets(){
		List<int> pets = new List<int>();
		for(int i=0;i<DataHolder.Pets().GetDataCount();i++){
			if(IsEquipPet(DataHolder.Pet(i).iD)){
				pets.Add(DataHolder.Pet(i).iD);
			}
		}
		return pets;
	}

	#endregion

	public string GetName()
	{
		return GetUser().fullname;
	}

	public void SetName(string n)
	{
		User u = GetUser();
		u.fullname = n;
		UpdateUserProfile (u);
	}

	
}


