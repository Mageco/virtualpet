using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using MageApi.Models;
using MageApi;
using MageApi.Models.Request;
using MageApi.Models.Response;
using UnityEngine.UI;
using Mage.Models.Users;
using System.IO;
using Crs.Models.Request;
using Crs;
using Crs.Models.Response;
using Mage.Models.Application;
using Mage.Models.Game;

public class UserManager : MonoBehaviour
{

    public static UserManager instance;
	public InputField dataName;
	public InputField dataValue;
	public Text userInfo;
	string tmpURL;
    #region Load
    void Awake()
    {
        if (instance == null)
            instance = this;

    }
    // Use this for initialization
    void Start()
    {
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }

	public void LoginClick() {
		LoginRequest r = new LoginRequest (ApiSettings.LOGIN_DEVICE_UUID);

		//call to login api
		ApiHandler.instance.SendApi<LoginResponse>(
			ApiSettings.API_LOGIN, 
			r, 
			(result) => {
				Debug.Log("Success: Do something interest here");
				//do some other processing here
				Debug.Log("User token: " + result.Token);
				RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
				RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);
				this.userInfo.text = result.User.ToJson();
				Debug.Log("Loggin usserID: " + result.User.id);
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

	public void LoginUsernamePasswordClick() {
		LoginRequest r = new LoginRequest (ApiSettings.LOGIN_USERNAME_PASSWORD, "", "butanido", "123456");
		//call to login api
		ApiHandler.instance.SendApi<LoginResponse>(
			ApiSettings.API_LOGIN, 
			r, 
			(result) => {
				Debug.Log("Success: Do something interest here");
				//do some other processing here
				Debug.Log("User token: " + result.Token);
				RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
				RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);
				this.userInfo.text = result.User.ToJson();
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


	public void UpdateUserDataClick() {
		UpdateUserDataRequest r = new UpdateUserDataRequest ();
		UserData tmp = new UserData ("Magic", "15400", "MagicPoint");
		r.UserDatas.Add (tmp);
		tmp = new UserData ("Exp", "7310", "Experience");
		r.UserDatas.Add (tmp);
		tmp = new UserData ("Power", "200", "Power");
		r.UserDatas.Add (tmp);
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

	public void UpdateUserLeaderboardClick() {
		UpdateUserLeaderboardRequest r = new UpdateUserLeaderboardRequest ();
		UserData tmp = new UserData ("Magic", "15400", "MagicPoint");
		r.LeaderboardDatas.Add (tmp);
		tmp = new UserData ("Exp", "7310", "Experience");
		r.LeaderboardDatas.Add (tmp);
		tmp = new UserData ("Power", "200", "Power");
		r.LeaderboardDatas.Add (tmp);
		//call to login api
		ApiHandler.instance.SendApi<UpdateUserLeaderboardResponse>(
			ApiSettings.API_UPDATE_USER_LEADER_BOARD,
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

	public void DeleteUserDataClick() {
		DeleteUserDataRequest r = new DeleteUserDataRequest ();
		UserData tmp = new UserData ("Magic");
		r.UserDatas.Add (tmp);

		//call to login api
		ApiHandler.instance.SendApi<DeleteUserDataResponse>(
			ApiSettings.API_DELETE_USER_DATA,
			r, 
			(result) => {
				Debug.Log("Success: Delete user data");
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

	public void UploadFileClick() {
		UploadFileRequest r = new UploadFileRequest ();

		string imagePath = Application.dataPath + "/Images/f-m-7.png";

		r.SetUploadFile (File.ReadAllBytes(imagePath));

		//call to login api
		ApiHandler.instance.UploadFile<UploadFileResponse>(
			r, 
			(result) => {
				Debug.Log("Success: Upload file successfully");
				Debug.Log("Upload URL: " + result.UploadedURL);
				tmpURL = result.UploadedURL;
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

	public void UpdateProfileClick() {
		UpdateProfileRequest r = new UpdateProfileRequest ();

		r.Avatar = tmpURL;

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

	public void GetLeaderBoardClick() {
		GetLeaderBoardRequest r = new GetLeaderBoardRequest ("Power");

		//call to login api
		ApiHandler.instance.SendApi<GetLeaderBoardResponse>(
			ApiSettings.API_GET_LEADER_BOARD,
			r, 
			(result) => {
				Debug.Log("Success: get leaderboard successfully");
				Debug.Log("Leaderboard result: " + result.ToJson());
                
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

	public void GetUserMessagesClick() {
		GetUserMessagesRequest r = new GetUserMessagesRequest ();

		//call to login api
		ApiHandler.instance.SendApi<GetUserMessagesResponse>(
			ApiSettings.API_GET_USER_MESSAGE,
			r, 
			(result) => {
				Debug.Log("Success: get messages successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void GetUserProfileClick() {
		GetUserProfileRequest r = new GetUserProfileRequest ("12");

		//call to login api
		ApiHandler.instance.SendApi<GetUserProfileResponse>(
			ApiSettings.API_GET_USER_PROFILE,
			r, 
			(result) => {
				Debug.Log("Success: get user profile successfully");
				Debug.Log("Profile result: " + result.ToJson());
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

	public void SendMessageClick() {
		SendMessageRequest r = new SendMessageRequest ("4", MessageType.PushNotification, "test from unity", "no title");

		//call to login api
		ApiHandler.instance.SendApi<SendMessageResponse>(
			ApiSettings.API_SEND_MESSAGE,
			r, 
			(result) => {
				Debug.Log("Success: send message successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void SendUserEventClick() {
		SendUserEventRequest r = new SendUserEventRequest ("Open_Main_Screen");

		//call to login api
		ApiHandler.instance.SendApi<SendUserEventResponse>(
			ApiSettings.API_SEND_USER_EVENT,
			r, 
			(result) => {
				Debug.Log("Success: send event successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void GetApplicationDataClick() {
		GetApplicationDataRequest r = new GetApplicationDataRequest ();

		//call to login api
		ApiHandler.instance.SendApi<GetApplicationDataResponse>(
			ApiSettings.API_GET_APPLICATION_DATA,
			r, 
			(result) => {
				Debug.Log("Success: send event successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void LinkFacebookAccountClick() {
		LinkFacebookAccountRequest r = new LinkFacebookAccountRequest ();
		r.FacebookId = "new facebook id";
		//call to login api
		ApiHandler.instance.SendApi<LinkFacebookAccountResponse>(
			ApiSettings.API_LINK_FB_ACCOUNT,
			r, 
			(result) => {
				Debug.Log("No existing account");
				Debug.Log("Messages result: " + result.ToJson());
			},
			(errorStatus) => {
				Debug.Log("Account has been linked");
				Debug.Log("Error: " + errorStatus);
				//do some other processing here
			},
			() => {
				//timeout handler here
				Debug.Log("Api call is timeout");
			}
		);
	}

	public void ConfirmLinkFacebookAccountClick() {
		ConfirmLinkFacebookAccountRequest r = new ConfirmLinkFacebookAccountRequest ();
		r.FacebookId = "new facebook id";
		//call to login api
		ApiHandler.instance.SendApi<ConfirmLinkFacebookAccountResponse>(
			ApiSettings.API_CONFIRM_LINK_FB_ACCOUNT,
			r, 
			(result) => {
				Debug.Log("Success: link successful");
				Debug.Log("Messages result: " + result.ToJson());
				RuntimeParameters.GetInstance().SetParam(ApiSettings.SESSION_LOGIN_TOKEN, result.Token);
				RuntimeParameters.GetInstance().SetParam(ApiSettings.LOGGED_IN_USER, result.User);
				this.userInfo.text = result.User.ToJson();

				//do all things like login
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

	public void SendQuestionAnswerRecordClick() {
		SendQuestionAnswerRecordRequest r = new SendQuestionAnswerRecordRequest (1, 1, 1, 1, CrsApiSettings.QUESTION_STATUS_CORRECT, 0, 10, DateTime.Now, DateTime.Now);
		Debug.Log("now: " + r.StartTime);
		//call to login api
		ApiHandler.instance.SendApi<SendQuestionAnswerRecordResponse>(
			CrsApiSettings.API_SEND_QUESTION_ANSWER_RECORD,
			r, 
			(result) => {
				Debug.Log("Success: send record successful");
				//do all things like login
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

	public void GetAudioFromTextClick() {
		GetAudioFromTextRequest r = new GetAudioFromTextRequest (
												ApiSettings.GetVoiceCharacter(VoiceCharacterOptions.vi_VN_Wavenet_A), 
												"Cún cưng",
												ApiSettings.GetLanguageOption(LanguageOptions.vi_VN),  
												"<speak>Cún cưng</speak>");  
		
		//call to login api
		ApiHandler.instance.SendApi<GetAudioFromTextResponse>(
			ApiSettings.API_GET_AUDIO_FROM_TEXT,
			r, 
			(result) => {
				Debug.Log("Get audio successful");
				//do all things like login
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

	public void GetApplicationAudioResourcesClick() {
		GetApplicationAudioResourcesRequest r = new GetApplicationAudioResourcesRequest(1);  // English wil be EN_en
		
		//call to login api
		ApiHandler.instance.SendApi<GetApplicationAudioResourcesResponse>(
			ApiSettings.API_GET_APPLICATION_AUDIO_RESOURCES,
			r, 
			(result) => {
				Debug.Log("Get audio successful");
				//do all things like login
				//Debug.Log("Result: " + result.Resources.ToString());

				foreach (Script i in result.Resources) {
					Debug.Log("item:" + i.ToJson());
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

	public void CheckSubscriptionClick() {
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

	public void ValdiateActivationCodeClick() {
		ValidateActivationCodeRequest r = new ValidateActivationCodeRequest("CG509DA4F6F3", "", "0908026022", "" );  // English wil be EN_en
		
		//call to login api
		ApiHandler.instance.SendApi<ValidateActivationCodeResponse>(
			ApiSettings.API_VALIDATE_ACTIVATION_CODE,
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
					case 100:
						Debug.Log("User has used this code before!");
						break;
					case 101:
						Debug.Log("Code has been fully used! Reach max number of users allowed.");
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

	public void ActivateByProductClick() {
		ActivateByProductRequest r = new ActivateByProductRequest("GB-IAP-6M", "", "012345678", "" );  // English wil be EN_en
		
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

	public void CancelByProductClick() {
		CancelByProductRequest r = new CancelByProductRequest("INAPP");  // English wil be EN_en
		
		//call to login api
		ApiHandler.instance.SendApi<CancelByProductResponse>(
			ApiSettings.API_CANCEL_BY_PRODUCT,
			r, 
			(result) => {
				Debug.Log("Cancel subscription successful");
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

	public void GetApplicationMaterialsClick() {
		GetApplicationMaterialsRequest r = new GetApplicationMaterialsRequest ();

		//call to login api
		ApiHandler.instance.SendApi<GetApplicationMaterialsResponse>(
			ApiSettings.API_GET_APPLICATION_MATERIALS,
			r, 
			(result) => {
				Debug.Log("Success: materials successfully");
				Debug.Log("Messages result: " + result.ToJson());
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


	//example for update Public profile and Online status for Scall
	public void ExampleForScall_001() {

		UpdateUserDataRequest r = new UpdateUserDataRequest ();
		//r.UserId = "5901";
		
		r.UserDatas.Add(new UserData("ShowPublic", "1", "PublicInfo"));

		//send online time as ticks
		r.UserDatas.Add(new UserData("Online", "" + DateTime.Now.ToString("yyyyMMddHHmmss") , "PublicInfo"));

		//call to login api
		ApiHandler.instance.SendApi<UpdateUserDataResponse>(
			ApiSettings.API_UPDATE_USER_DATA,
			r, 
			(result) => {
				Debug.Log("Success: update user data successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

		// now inquiry
		GetUsersByUserDataRequest r2 = new GetUsersByUserDataRequest();
		r2.SearchDatas.Add(new SearchData("ShowPublic", SearchOperator.Equal, "1"));
		// condition to search who is online within last 5 minutes 
		r2.SearchDatas.Add(new SearchData("Online", SearchOperator.GreaterOrEqual, (DateTime.Now.Add(new TimeSpan(0, -5, 0)).ToString("yyyyMMddHHmmss"))));

		//call to login api
		ApiHandler.instance.SendApi<GetUsersByUserDataResponse>(
			ApiSettings.API_GET_USERS_BY_USER_DATAS,
			r2, 
			(result) => {
				Debug.Log("Success: update user data successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void AddGameCharacterClick() {
		AddGameCharacterRequest r = new AddGameCharacterRequest ("Cún cưng", "Shiba");

		//call to login api
		ApiHandler.instance.SendApi<AddGameCharacterResponse>(
			ApiSettings.API_ADD_GAME_CHARACTER,
			r, 
			(result) => {
				Debug.Log("Success: add game character successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void UpdateGameCharacterNameClick() {
		UpdateGameCharacterNameRequest r = new UpdateGameCharacterNameRequest ();

		//call to login api
		ApiHandler.instance.SendApi<UpdateGameCharacterNameResponse>(
			ApiSettings.API_UPDATE_GAME_CHARACTER_NAME,
			r, 
			(result) => {
				Debug.Log("Success: update game character name successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void UpdateGameCharacterDataClick() {
		UpdateGameCharacterDataRequest r = new UpdateGameCharacterDataRequest ("1");

		r.CharacterDatas = new List<CharacterData>() {
			new CharacterData("Coin", "200000", "Currency"),
			new CharacterData("Diamond", "200000", "Currency"),
			new CharacterData("2", "true", "Item"),
			new CharacterData("3", "used", "Item"),
		};

		//call to login api
		ApiHandler.instance.SendApi<UpdateGameCharacterDataResponse>(
			ApiSettings.API_UPDATE_GAME_CHARACTER_DATAS,
			r, 
			(result) => {
				Debug.Log("Success: update game character name successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void GetAvailableGameCharacterItemsClick() {
		GetAvailableGameCharacterItemsRequest r = new GetAvailableGameCharacterItemsRequest();

		//call to login api
		ApiHandler.instance.SendApi<GetAvailableGameCharacterItemsResponse>(
			ApiSettings.API_GET_AVAILABLE_GAME_CHARACTER_ITEMS,
			r, 
			(result) => {
				Debug.Log("Success: update game character name successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void BuyGameCharacterItemClick() {
		BuyGameCharacterItemRequest r = new BuyGameCharacterItemRequest("Hat_01");

		//call to login api
		ApiHandler.instance.SendApi<BuyGameCharacterItemResponse>(
			ApiSettings.API_BUY_GAME_CHARACTER_ITEM,
			r, 
			(result) => {
				Debug.Log("Success: update game character name successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

	public void UpdateGameCharacterItemStatusClick() {
		
		UpdateGameCharacterItemStatusRequest r = new UpdateGameCharacterItemStatusRequest("1", "1", CharacterItemStatus.AVAILABLE);

		//call to login api
		ApiHandler.instance.SendApi<UpdateGameCharacterItemStatusResponse>(
			ApiSettings.API_UPDATE_GAME_CHARACTER_ITEM_STATUS,
			r, 
			(result) => {
				Debug.Log("Success: update game character name successfully");
				Debug.Log("Messages result: " + result.ToJson());
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

}

