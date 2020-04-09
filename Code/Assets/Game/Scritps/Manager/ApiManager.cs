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
using Firebase.Messaging;

public class ApiManager : MageEngine {

	//private static ApiManager instance;
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
        /*
		if (instance == null)
			instance = this;
		else
			Destroy (this.gameObject);

		DontDestroyOnLoad (this.gameObject);*/
	}

	protected override void OnLoginCompleteCallback() {
        if (IsReloadRequired())
        {
            Debug.Log("Load data from server");
			
            GameManager.instance.myPlayer = GetUserData<PlayerData>();

			if (GameManager.instance.myPlayer.pets.Count > 0)
			{
				foreach (Pet p in GameManager.instance.myPlayer.pets)
				{
					PlayerPet pet = new PlayerPet(p.iD);
					pet.level = p.level;
					pet.itemState = p.itemState;
					pet.isNew = p.isNew;
					GameManager.instance.myPlayer.petDatas.Add(pet);
				}
				GameManager.instance.myPlayer.pets.Clear();
			}

			if (GetUser().last_run_app_version != ""  && string.Compare(GetUser().last_run_app_version, "1.08") <= 0) {
				Debug.Log("Set quest id 100");
				GameManager.instance.myPlayer.questId = 100;
				GameManager.instance.SavePlayer();
			}

            GameManager.instance.UnLoadPets();
            if (ItemManager.instance != null)
            {
                MageManager.instance.LoadSceneWithLoading("House");
            }

            if(GameManager.instance.myPlayer.playerName == "")
            {
				GameManager.instance.myPlayer.playerName = "Player" + Random.Range(100000, 1000000).ToString();
				User u = MageEngine.instance.GetUser();
				u.fullname = GameManager.instance.myPlayer.playerName;
				MageEngine.instance.UpdateUserProfile(u);
			}
        }

		SetupFirebaseMessaging();
    }

	public void ExampleOfGetRandomFriend() { 
		GetRandomFriend( (User u) => {
			Debug.Log("Friend: " + u.ToJson());
		} );
	}

	//public static ApiManager GetInstance() {
	//	return instance;
	//}

	protected void Start() {
		DoLogin();
	}


	protected override void OnHasNewUserMessagesCallback(List<Message> newMessages) {
       //sample only
	   for (int i = 0; i < newMessages.Count; i++) {
		   Debug.Log("Update message: " + newMessages[i].id + " as read");
			if (newMessages[i].status == MessageStatus.New)
			{
				ConfirmationPopup confirm = MageManager.instance.OnConfirmationPopup(newMessages[i].title, newMessages[i].message);
				string url = "";
                #if UNITY_ANDROID
				url = newMessages[i].action_android;
                #elif UNITY_IOS
                url = newMessages[i].action_android;
                #endif
				confirm.okButton.onClick.AddListener(delegate {
					OnClick(url);
				});
				UpdateMessageStatus(newMessages[i].id, MessageStatus.Read);
			}
		}

	}


    protected override void OnNewFirebaseMessageCallback(object sender, MessageReceivedEventArgs e)
    {
		ConfirmationPopup confirm = MageManager.instance.OnConfirmationPopup("",e.Message.Notification.Body);
        if(e.Message.Notification.Title == "Update")
        {
			confirm.okButton.onClick.AddListener(delegate {
				OnUpdate();
			});
		}

		Debug.Log(e.Message.RawData);
    }


    void OnClick(string url)
    {
	    Application.OpenURL(url);
    }

    void OnUpdate()
    {
#if UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/details?id=vn.com.mage.virtualpet");

#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/app/pet-house-little-friends/id1499945488?ls=1");
#endif
	}

}


