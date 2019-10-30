using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MageApi.Models;
using MageApi;
using MageApi.Models.Request;
using MageApi.Models.Response;
using UnityEngine.UI;
using Mage.Models.Users;
using System.IO;
using Mage.Models.Application;


public class ApiManager : MonoBehaviour {

	[HideInInspector]
	public User user;
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


	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			GameObject.Destroy (this.gameObject);

		DontDestroyOnLoad (this.gameObject);

		if (ES2.Exists ("User")) {
			user = ES2.Load<User> ("User");
		}else
		{
			user = new User();
			user.SetUserData (new UserData ("Coin", "100000", ""));
			user.SetUserData (new UserData ("Diamond", "50000", ""));
			user.SetUserData (new UserData ("2", "used", "Item"));
			user.SetUserData (new UserData ("4", "used", "Item"));
			user.SetUserData (new UserData ("7", "used", "Item"));
			user.SetUserData (new UserData ("8", "used", "Item"));
			user.SetUserData (new UserData ("11", "used", "Item"));
			user.SetUserData (new UserData ("13", "used", "Item"));
			user.SetUserData (new UserData ("17", "used", "Item"));
			user.SetUserData (new UserData ("41", "used", "Item"));
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

		user.SetUserData (new UserData ("Coin", "100000", ""));
		user.SetUserData (new UserData ("Diamond", "50000", ""));
		user.SetUserData (new UserData ("2", "used", "Item"));
		user.SetUserData (new UserData ("4", "used", "Item"));
		user.SetUserData (new UserData ("7", "used", "Item"));
		user.SetUserData (new UserData ("8", "used", "Item"));
		user.SetUserData (new UserData ("11", "used", "Item"));
		user.SetUserData (new UserData ("13", "used", "Item"));
		user.SetUserData (new UserData ("17", "used", "Item"));
		user.SetUserData (new UserData ("41", "used", "Item"));

		SaveUserData ();
		UpdateUserData ();
		UpdateUserProfile ();
		ItemController.instance.LoadItems();
	}

	void CreateExistingUser(User u)
	{
		user = u;
		bool isSave = false;
			
		if (user.GetUserData ("Coin") == null) {
			isSave = true;
			user.user_datas.Add (new UserData ("Coin", "100000", ""));
		}
		if (user.GetUserData ("Diamond") == null) {
			isSave = true;
			user.user_datas.Add (new UserData ("Diamond", "50000", ""));
		}

		if (isSave) {
			UpdateUserData ();
			UpdateUserProfile ();
		}

		SaveUserData ();
	}

	void SaveUserData()
	{
		ES2.Save<User> (user, "User");
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
		user.SetUserData (new UserData (itemId.ToString(), "true", "Item"));
		SaveUserData ();
		UpdateUserData ();
	}

	public void UseItem(int itemId){
		Debug.LogWarning(itemId);
		//if(HaveItem(itemId)){
			List<int> items = GetUsedItems();
			for(int i=0;i<items.Count;i++){
				if(DataHolder.GetItem(items[i]).itemType == DataHolder.GetItem(itemId).itemType && DataHolder.GetItem(items[i]).iD != itemId){
					user.SetUserData (new UserData (DataHolder.GetItem(items[i]).iD.ToString(), "true", "Item"));
				}
			}
			user.SetUserData (new UserData (itemId.ToString(), "used", "Item"));
			SaveUserData ();
			UpdateUserData ();
		//}
	}

	public bool HaveItem(int itemId)
	{
		if (user.GetUserData (itemId.ToString()) == "true" || user.GetUserData (itemId.ToString()) == "used")
			return true;
		else
			return false;
	}

	public bool UsedItem(int itemId)
	{
		if (user.GetUserData (itemId.ToString()) == "used")
			return true;
		else
			return false;
	}

	public List<int> GetBuyItems(){
		List<int> items = new List<int>();
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(HaveItem(DataHolder.Item(i).iD)){
				items.Add(DataHolder.Item(i).iD);
			}
		}
		return items;
	}

	public List<int> GetUsedItems(){
		List<int> items = new List<int>();
		for(int i=0;i<DataHolder.Items().GetDataCount();i++){
			if(UsedItem(DataHolder.Item(i).iD)){
				items.Add(DataHolder.Item(i).iD);
			}
		}
		return items;
	}

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




	#endregion
}


