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


public class ApiManager : MonoBehaviour {

	[HideInInspector]
	public User user;
	public Character character;
	public static ApiManager instance;
	[HideInInspector]
	public int testRound = 0;
	[HideInInspector]
	public int testSequence = 0;
	[HideInInspector]
	public bool isLogin = false;

	[HideInInspector]
	public string contactUrl = "";
	[HideInInspector]
	public string contactPhone = "";
	[HideInInspector]
	public int option = 0;


	public List<ActionData> actions = new List<ActionData>();

	private Hashtable variables;

    public bool isSaveDataLocal = false;
    public bool isSaveDataOnline = false;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy (this.gameObject);

		DontDestroyOnLoad (this.gameObject);

		character = new Character();

		if (ES2.Exists ("User")) {
			user = ES2.Load<User> ("User");
        }
        else
		{
			user = new User();
            user.SetCharacter(character);
		}
	}


	void Start()
	{
		LoginWithDeviceID ();
	}


	#region User API
	public void LoginWithDeviceID() {
		LoginRequest r = new LoginRequest (ApiSettings.LOGIN_DEVICE_UUID);

		//call to login api
		ApiHandler.instance.SendApi<LoginResponse>(
			ApiSettings.API_LOGIN, 
			r, 
			(result) => {
				//do some other processing here
				Debug.Log("User token: " + result.Token);
				Debug.Log("Loggin usserID: " + result.User.id);
				isLogin = true;
				MageManager.instance.SetLocation(result.User.country_code);
				OnCompleteLogin (result);
				GetApplicationData ();
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			}, 
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}

	void OnCompleteLogin(LoginResponse result)
	{
		RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
		RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);
		Debug.Log(result.User.ToJson());
		if (result.User.status == "5") {
			CreateNewUser ();
		}else
			CreateExistingUser (result.User);
	}

	void CreateNewUser()
	{
		Debug.Log ("NEw user created");
        SaveUserData ();
		UpdateUserData ();
		UpdateUserProfile ();
    }

	void CreateExistingUser(User u)
	{
        #if UNITY_EDITOR
        if (!isSaveDataOnline)
            return;
        #endif

        user = u;
		UpdateUserData ();
		UpdateUserProfile ();
		SaveUserData ();
	}

	void SaveUserData()
	{
        #if UNITY_EDITOR
        if(isSaveDataLocal)
            ES2.Save<User> (user, "User");
        #else
        ES2.Save<User> (user, "User");
        #endif
    }

	public void GetApplicationAudioResourcesClick() {
		GetApplicationAudioResourcesRequest r = new GetApplicationAudioResourcesRequest(1);  // English wil be EN_en

		//call to login api
		ApiHandler.instance.SendApi<GetApplicationAudioResourcesResponse>(
			ApiSettings.API_GET_APPLICATION_AUDIO_RESOURCES,
			r, 
			(result) => {
				Debug.Log("Get audio successful");
				//do all things like login
				Debug.Log("Result: " + result.Resources.ToString());
				int n=0;
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}

	public void UpdateUserProfile() {

		if (!isLogin)
			return;

		UpdateProfileRequest r = new UpdateProfileRequest ();
		r.Fullname = user.fullname;

		//call to login api
		ApiHandler.instance.SendApi<UpdateProfileResponse>(
			ApiSettings.API_UPDATE_PROFILE,
			r, 
			(result) => {
				Debug.Log("Success: Upload profile successfully");
				Debug.Log("New Avatar: " + result.User.avatar);
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}

	public void UpdateUserData() {

		if (!isLogin)
			return;

	#if UNITY_EDITOR
			if (!isSaveDataOnline)
				return;
	#endif

        UpdateUserDataRequest r = new UpdateUserDataRequest ();
		r.UserDatas = user.user_datas;
		//call to login api
		ApiHandler.instance.SendApi<UpdateUserDataResponse>(
			ApiSettings.API_UPDATE_USER_DATA, 
			r, 
			(result) => {
				Debug.Log("Success: Update user data");
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}

	#endregion


	


	#region Event
	public void SendAppEvent(string eventName) {
		SendUserEventRequest r = new SendUserEventRequest (eventName);

		//call to login api
		ApiHandler.instance.SendApi<SendUserEventResponse>(
			ApiSettings.API_SEND_USER_EVENT,
			r, 
			(result) => {
				Debug.Log("Success: send event successfully");
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}

	public void GetApplicationData() {

		GetApplicationDataRequest r = new GetApplicationDataRequest ();

		//call to login api
		ApiHandler.instance.SendApi<GetApplicationDataResponse>(
			ApiSettings.API_GET_APPLICATION_DATA,
			r, 
			(result) => {
				Debug.Log("Success: send event successfully");
				Debug.Log("Messages result: " + result.ToJson());
				foreach(ApplicationData data in result.ApplicationDatas)
				{
					if(data.attr_name == "ContactPhone")
						contactPhone = data.attr_value;

					if(data.attr_name == "ContactWeb")
						contactUrl = data.attr_value;
				}
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}
	#endregion

	#region Player Data
	public int GetCoin()
	{
		if (user.GetUserData ("Coin") != null)
			return int.Parse (user.GetUserData ("Coin"));
		else
			return 0;
	}

	public void AddCoin(int c)
	{
		int c1 = GetCoin();
		if (c1 + c >= 0) {
			c1 += c;
			user.SetUserData (new UserData ("Coin", c1.ToString (), ""));
			SaveUserData ();
			UpdateUserData ();

		} else {
			Debug.Log ("Not Enough Coin");
		}

	}

	public int GetDiamond()
	{
		if (user.GetUserData ("Diamond") != null)
			return int.Parse (user.GetUserData ("Diamond"));
		else
			return 0;
	}

	public void AddDiamond(int c)
	{
		int c1 = GetDiamond();
		if (c1 + c >= 0) {
			c1 += c;
			user.SetUserData (new UserData ("Diamond", c1.ToString (), ""));
			SaveUserData ();
			UpdateUserData ();

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
		user.SetUserData (new UserData (itemId.ToString(),ItemState.Have.ToString(), "Item"));
		SaveUserData ();
		UpdateUserData ();
	}

	public void RemoveItem(int itemId)
	{
		user.SetUserData (new UserData (itemId.ToString(),ItemState.OnShop.ToString(), "Item"));
		SaveUserData ();
		UpdateUserData ();
	}

	public void EquipItem(int itemId){
		//if(HaveItem(itemId)){
			List<int> items = GetEquipedItems();
			for(int i=0;i<items.Count;i++){
				if(DataHolder.GetItem(items[i]).itemType == DataHolder.GetItem(itemId).itemType && DataHolder.GetItem(items[i]).iD != itemId){
					user.SetUserData (new UserData (DataHolder.GetItem(items[i]).iD.ToString(), ItemState.Have.ToString(), "Item"));
				}
			}
			user.SetUserData (new UserData (itemId.ToString(), ItemState.Equiped.ToString(), "Item"));
			SaveUserData ();
			UpdateUserData ();
		//}
	}

	public bool IsHaveItem(int itemId)
	{
		if (user.GetUserData (itemId.ToString()) == ItemState.Have.ToString() || user.GetUserData (itemId.ToString()) == ItemState.Equiped.ToString())
			return true;
		else
			return false;
	}

	public bool IsEquipItem(int itemId)
	{
		if (user.GetUserData (itemId.ToString()) == ItemState.Equiped.ToString())
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
		character.SetCharacterData (new CharacterData (petId.ToString(),ItemState.Have.ToString(), "Pet"));
		SaveUserData ();
		UpdateUserData ();
	}

	public void EquipPet(int petId){
		//if(HavePet(petId)){
			character.SetCharacterData (new CharacterData (petId.ToString(), ItemState.Equiped.ToString(), "Pet"));
			SaveUserData ();
			UpdateUserData ();
		//}
	}

	public bool IsHavePet(int petId)
	{
		if (character.GetCharacterData (petId.ToString()) == ItemState.Have.ToString() || character.GetCharacterData (petId.ToString()) == ItemState.Equiped.ToString() )
			return true;
		else
			return false;
	}

	public bool IsEquipPet(int petId)
	{
		if (character.GetCharacterData (petId.ToString()) == ItemState.Equiped.ToString())
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
		return user.fullname;
	}

	public void SetName(string n)
	{
		user.fullname = n;
		SaveUserData ();
		UpdateUserProfile ();
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


