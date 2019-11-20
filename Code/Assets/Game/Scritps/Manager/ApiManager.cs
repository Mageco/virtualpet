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

	public List<ActionData> actions = new List<ActionData>();

	private Hashtable variables;

    //public bool isSaveDataLocal = false;
    //public bool isSaveDataOnline = false;

	public static ApiManager GetInstance() {
		return (ApiManager)MageEngine.instance;
	}

	#region Player Data
	public int GetCoin()
	{
		if (GetUser().GetUserData ("Coin") != null)
			return int.Parse (GetUser().GetUserData ("Coin"));
		else
			return 0;
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
		if (GetUser().GetUserData ("Diamond") != null)
			return int.Parse (GetUser().GetUserData ("Diamond"));
		else
			return 0;
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
/* 				if(MageManager.instance.GetLanguageName () == "English") 
					MageManager.instance.OnNotificationPopup ("Warning", "You have not enough Coin");
				else
					MageManager.instance.OnNotificationPopup ("Chú ý", "Bạn không đủ vàng để mua sản phẩm này"); */
				return false;
			}
			AddCoin (-price);
			AddItem (itemId);
			return true;
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
/* 				if(MageManager.instance.GetLanguageName () == "English") 
					MageManager.instance.OnNotificationPopup ("Warning", "You have not enough Ruby");
				else
					MageManager.instance.OnNotificationPopup ("Chú ý", "Bạn không đủ ngọc để mua sản phẩm này"); */
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
		GetUser().SetUserData (new UserData (itemId.ToString(),ItemState.Have.ToString(), "Item"));
	}

	public void RemoveItem(int itemId)
	{
		GetUser().SetUserData (new UserData (itemId.ToString(),ItemState.OnShop.ToString(), "Item"));
	}

	public void EquipItem(int itemId){
		//if(HaveItem(itemId)){
			List<int> items = GetEquipedItems();
			for(int i=0;i<items.Count;i++){
				if(DataHolder.GetItem(items[i]).itemType == DataHolder.GetItem(itemId).itemType && DataHolder.GetItem(items[i]).iD != itemId){
					GetUser().SetUserData (new UserData (DataHolder.GetItem(items[i]).iD.ToString(), ItemState.Have.ToString(), "Item"));
				}
			}
			GetUser().SetUserData (new UserData (itemId.ToString(), ItemState.Equiped.ToString(), "Item"));
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
		UpdateGameCharacterData(new CharacterData (petId.ToString(),ItemState.Have.ToString(), "Pet"));
	}

	public void EquipPet(int petId){
		//if(HavePet(petId)){
		UpdateGameCharacterData (new CharacterData (petId.ToString(), ItemState.Equiped.ToString(), "Pet"));
		//}
	}

	public bool IsHavePet(int petId)
	{
		if (GetActiveCharacter().GetCharacterData (petId.ToString()) == ItemState.Have.ToString() || GetActiveCharacter().GetCharacterData (petId.ToString()) == ItemState.Equiped.ToString() )
			return true;
		else
			return false;
	}

	public bool IsEquipPet(int petId)
	{
		if (GetActiveCharacter().GetCharacterData (petId.ToString()) == ItemState.Equiped.ToString())
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

	#region Actions
	public void LogAction(ActionType t){
		ActionData a = new ActionData();
		a.actionType = t;
		a.startTime = System.DateTime.Now;
		actions.Add(a);
		Debug.Log(a.actionType + "  " + a.startTime.ToShortTimeString());
		SaveAction();
	}

	public List<ActionData> GetActionLogs(System.DateTime t){
		List<ActionData> temp = new List<ActionData>();
		for(int i=0;i<actions.Count;i++){
			if(actions[i].startTime > t){
				temp.Add(actions[i]);
			}
		}
		return temp;
	}

	void SaveAction(){
		ES2.Save(actions,"ActionLog");
	}

	void LoadAction(){
		if(ES2.Exists("ActionLog")){
			ES2.LoadList<ActionData>("ActionLog");
		}
	}

	#endregion

	#region  Variable
	public void SaveVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();
		foreach (DictionaryEntry entry in variables) {
			keys.Add ((string)entry.Key);
			values.Add ((string)entry.Value);
		}
		ES2.Save(keys, "variablesKey");
		ES2.Save(values, "variablesValue");

	}

	public void LoadVariables()
	{
		List <string> keys = new List<string> ();
		List <string> values = new List<string> ();

		if(ES2.Exists("variablesKey"))
			keys = ES2.LoadList<string>("variablesKey");
		if(ES2.Exists("variablesValue"))
			values = ES2.LoadList<string>("variablesValue");

		for (int i = 0; i < keys.Count;i++) {
			if(i < values.Count)
				variables.Add (keys [i], values [i]);

			Debug.Log ("Load Variables " + keys [i] + " " + values [i]);
		}
	}


	#endregion
}


