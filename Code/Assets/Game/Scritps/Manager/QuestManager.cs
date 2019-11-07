using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{    
    public GameObject timeline1;
    public GameObject timeline2;


    public int questID = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void StartQuest(){
        UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(DataHolder.Quest(questID).dialogId).GetDescription(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnQuestNotification(){
        UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(DataHolder.Quest(questID).dialogId).GetDescription(0));
    }   
}
