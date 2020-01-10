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

    }

	//public static ApiManager GetInstance() {
	//	return instance;
	//}

	protected void Start() {
		DoLogin();
	}
	
}


