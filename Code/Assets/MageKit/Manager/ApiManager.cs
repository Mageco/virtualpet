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
				CheckSubscription();
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

		user.user_datas.Add (new UserData ("Coin", "100", ""));
		user.user_datas.Add (new UserData ("Live", "5", ""));
		user.user_datas.Add (new UserData ("AvatarId", "0", ""));
		user.SetUserData (new UserData ("Avatar0", "true", "Item"));

		SaveUserData ();
		UpdateUserData ();
		UpdateUserProfile ();
	}

	void CreateExistingUser(User u)
	{
		user = u;
		bool isSave = false;
			
		if (user.GetUserData ("Coin") == null) {
			isSave = true;
			user.user_datas.Add (new UserData ("Coin", "100", ""));
		}
		if (user.GetUserData ("Live") == null) {
			isSave = true;
			user.user_datas.Add (new UserData ("Live", "5", ""));
		}
		if (user.GetUserData ("AvatarId") == null) {
			isSave = true;
			user.user_datas.Add (new UserData ("AvatarId", "0", ""));
		}
		if (user.GetUserData ("Avatar0") == null) {
			isSave = true;
			user.user_datas.Add (new UserData ("Avatar0", "true", ""));
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


	#region product
	public void CheckSubscription() {
		CheckSubscriptionRequest r = new CheckSubscriptionRequest();  // English wil be EN_en

		//call to login api
		ApiHandler.instance.SendApi<CheckSubscriptionResponse>(
			ApiSettings.API_CHECK_SUBSCRIPTION,
			r, 
			(result) => {
				Debug.Log("Check subscription");
				//do all things like login
				//Debug.Log("Result: " + result.Resources.ToString());

				foreach (string i in result.LicenseItems) {
					Debug.Log("license item:" + i);
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

	public void ValdiateActivationCode(string activationCode,string phone) {
		ValidateActivationCodeRequest r = new ValidateActivationCodeRequest(activationCode, "", phone, "" );  // English wil be EN_en

		//call to login api
		ApiHandler.instance.SendApi<ValidateActivationCodeResponse>(
			ApiSettings.API_VALIDATE_ACTIVATION_CODE,
			r, 
			(result) => {
				Debug.Log("Validate activation code successful");
				MageManager.instance.OnNotificationPopup("Chú ý","Kích hoạt thành công");
				foreach (string i in result.LicenseItems) {
					
					Debug.Log("license item:" + i);
				}
				MageManager.instance.OffWaiting ();
			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
				switch(errorStatus) {
				case 100:
					MageManager.instance.OnNotificationPopup("Chú ý","Thiết bị đã được kích hoạt rồi");
					Debug.Log("User has used this code before!");
					break;
				case 101:
					MageManager.instance.OnNotificationPopup("Chú ý","Đã quá số lượng thiết bị kích hoạt");
					Debug.Log("Code has been fully used! Reach max number of users allowed.");
					break;
				case 102:
					MageManager.instance.OnNotificationPopup("Chú ý","Mã kích hoạt không đúng");
					Debug.Log("Code is invalid");
					break;
				}
				MageManager.instance.OffWaiting ();
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
				MageManager.instance.OffWaiting ();
			}
		);
	}

	public void ActivateByProduct(string productCode,string phone) {
		ActivateByProductRequest r = new ActivateByProductRequest(productCode, "", phone, "" );  // English wil be EN_en

		//call to login api
		ApiHandler.instance.SendApi<ActivateByProductResponse>(
			ApiSettings.API_ACTIVATE_BY_PRODUCT,
			r, 
			(result) => {
				Debug.Log("Validate activation code successful");
				foreach (string i in result.LicenseItems) {
					Debug.Log("license item:" + i);
				}

			},
			(errorStatus) => {
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
				switch(errorStatus) {
				case 103:
					Debug.Log("Wrong Product code!");
					break;
				case 104:
					Debug.Log("Activation code generated failed.");
					break;
				case 102:
					Debug.Log("Code is invalid");
					break;
				}
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

	public void BuyItem(string itemId,int price,PriceType type)
	{
		if(type == PriceType.Coin){
			if (price > GetCoin ()) {
				if(MageManager.instance.GetLanguageName () == "English") 
					MageManager.instance.OnNotificationPopup ("Warning", "You have not enough Coin");
				else
					MageManager.instance.OnNotificationPopup ("Chú ý", "Bạn không đủ vàng để mua sản phẩm này");
				return;
			}
			AddCoin (-price);
			AddItem (itemId);
		}else if(type == PriceType.Diamond){
			if (price > GetDiamond ()) {
				if(MageManager.instance.GetLanguageName () == "English") 
					MageManager.instance.OnNotificationPopup ("Warning", "You have not enough Ruby");
				else
					MageManager.instance.OnNotificationPopup ("Chú ý", "Bạn không đủ ngọc để mua sản phẩm này");
				return;
			}
			AddDiamond (-price);
			AddItem (itemId);
		}
	}


	public void AddItem(string itemId)
	{
		user.SetUserData (new UserData (itemId, "true", "Item"));
		SaveUserData ();
		UpdateUserData ();
	}

	public bool HaveItem(string itemId)
	{
		if (user.GetUserData (itemId) == "true")
			return true;
		else
			return false;
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


	public int GetAvatarId()
	{
		if (user.GetUserData ("AvatarId") != null)
			return int.Parse (user.GetUserData ("AvatarId"));
		else
			return 0;
	}

	public void SetAvatarId(int id)
	{
		user.SetUserData (new UserData ("AvatarId", id.ToString (), ""));
		SaveUserData ();
		UpdateUserData ();
	}



	#endregion
}


