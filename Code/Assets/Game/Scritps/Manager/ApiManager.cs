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




    void OnClick(string url)
    {
	    Application.OpenURL(url);
    }

}


