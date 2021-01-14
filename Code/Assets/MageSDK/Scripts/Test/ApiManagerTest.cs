using System.Collections.Generic;
using UnityEngine;
using Mage.Models.Users;
using MageSDK.Client;
using System;
using MageSDK.Client.Helper;
using System.Collections;
using MageApi;
#if USE_FIREBASE
using Firebase.Messaging;
#endif

public class ApiManagerTest : MageEngine
{

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

    private static ApiManagerTest overrideInstance;
    public static ApiManagerTest GetThis()
    {
        return overrideInstance;
    }
    protected override void Load()
    {

    }

    protected override void OnLoginCompleteCallback()
    {
#if USE_FIREBASE
        SetupFirebaseMessaging();
#endif

    }

    public void ExampleOfGetRandomFriend()
    {
        GetRandomFriend((User u) =>
        {
            Debug.Log("Friend: " + u.ToJson());
        });
    }

    protected void Start()
    {
        if (overrideInstance == null)
            overrideInstance = this;
        DoLogin();
    }

    public void UpdateUserDataClick()
    {
        if (IsLogin())
        {
            UpdateUserData(new UserData() { attr_name = "Owner", attr_type = "Mage Co. Ltd" });
        }
    }

    protected override void OnHasNewUserMessagesCallback(List<Message> newMessages)
    {

        //sample only
        for (int i = 0; i < newMessages.Count; i++)
        {
            if (newMessages[i] != null)
            {
                Debug.Log("Update message: " + newMessages[i].id + " as read");
                if (newMessages[i].status == MessageStatus.New)
                {
                    string url = "";
#if UNITY_ANDROID
                    url = newMessages[i].action_android;
#elif UNITY_IOS
                url = newMessages[i].action_android;
#endif
                    UpdateMessageStatus(newMessages[i].id, MessageStatus.Read);
                }

            }

        }

    }


#if USE_FIREBASE
    protected override void OnNewFirebaseMessageCallback(object sender, MessageReceivedEventArgs e)
    {
        ConfirmationPopup confirm = MageManager.instance.OnConfirmationPopup("", e.Message.Notification.Body);
        if (e.Message.Notification.Title == "Update")
        {
            confirm.okButton.onClick.AddListener(delegate
            {
                OnUpdate();
            });
        }

        Debug.Log(e.Message.RawData);
    }
#endif

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

    public void MyTestFunction1(string name, Action<string> callback)
    {
        MageTaskQueueItem x = new MageTaskQueueItem();
        x.action = callback;
        x.parameters = new object[] { "This is set from api manager" };
        TestManager.instance.taskList.Add("action1", x);
    }

    public void MyTestFunction2(Action<string, int> callback)
    {
        MageTaskQueueItem x = new MageTaskQueueItem();
        x.action = callback;
        x.parameters = new object[] { "This is set from api manager", 50 };
        TestManager.instance.taskList.Add("action2", x);
    }

    public void MyTestScript()
    {
        MyTestFunction1("x", (t) =>
        {
            Debug.Log("My function 1 is trigger with: " + t);
            Debug.Log("My params user_id: " + GetUser().id);
        });

        MyTestFunction2((s, i) =>
        {
            Debug.Log("My function 2 is trigger with: " + s + " value: " + i);
            Debug.Log("My params user_id: " + GetUser().id);
        });
    }

    public void OtherFunction()
    {
        Debug.Log("this is other function in api manager");
        Debug.Log("last run app version: " + GetUser().last_run_app_version);
    }
}


