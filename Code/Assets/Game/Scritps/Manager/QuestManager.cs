using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public bool isStartQuest = false;
    public bool isEndQuest = false;
    bool isTimeline = true;
    System.DateTime startTime;
    float time;
    float maxTimeCheck = 0.2f;
    NotificationPopup tipUI;
    float delayTime = 0;
    bool isActive = true;

    float replayTime = 0;
    float maxReplayTime = 30;

    float fadeDuration = 1f;

    bool isReplay = true;

    QuestPanel questPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void StartQuest()
    {
        LoadQuestObject();
        PlayTip();
        isStartQuest = true;
        startTime = System.DateTime.Now;
    }

    void LoadQuestObject()
    {
        if(GameManager.instance.questId == 0){
        }else if(GameManager.instance.questId == 1){
        }else if(GameManager.instance.questId == 2){
        }else if(GameManager.instance.questId == 3){
        }else if(GameManager.instance.questId == 4){
        }else if(GameManager.instance.questId == 5){
        }else if(GameManager.instance.questId == 6){
        }else if(GameManager.instance.questId == 7){
        }
        delayTime = 5;
    }

    void PlayTip()
    {
        StartCoroutine(PlayTimeline());
    }

    IEnumerator PlayTimeline()
    {
        yield return new WaitForSeconds(delayTime);
        
        OnQuestNotification();
        if(GameManager.instance.questId == 0){
            GameManager.instance.GetPet(0).Food = 30;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 1){
            GameManager.instance.GetPet(0).Water = 30;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Drink));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 2){
            GameManager.instance.GetPet(0).Pee = 65;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Toilet));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 3){
            GameManager.instance.GetPet(0).Dirty = 65;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bath));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 4){
            GameManager.instance.GetPet(0).Sleep = 35;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bed));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 5){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Clean));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 6){
        }else if(GameManager.instance.questId == 8){
        }else if(GameManager.instance.questId == 9){
        }else if(GameManager.instance.questId == 10){
        }else if(GameManager.instance.questId == 11){
        }else if(GameManager.instance.questId == 12){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.MedicineBox));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 13){
        }else if(GameManager.instance.questId == 14){
        }else if(GameManager.instance.questId == 15){
        }

        isTimeline = false;
    }

    public void ReplayQuest()
    {
        replayTime = 0;
        isTimeline = true;
        PlayTip();
    }

    public void StartCompleteQuest()
    {
        
        if(questPanel == null && !isEndQuest){
            questPanel = UIManager.instance.OnQuestCompletePopup();
            questPanel.Load(GameManager.instance.questId);
            Debug.Log("Quest Complete");
        }
        isEndQuest = true;

    }

    public void EndCompleteQuest()
    {
        if (DataHolder.Quest(GameManager.instance.questId).haveItem)
        {
           GameManager.instance.AddItem(DataHolder.Quest(GameManager.instance.questId).itemId);
           GameManager.instance.EquipItem(DataHolder.Quest(GameManager.instance.questId).itemId);
            ItemManager.instance.LoadItems();
        }

       GameManager.instance.AddCoin(DataHolder.Quest(GameManager.instance.questId).coinValue);
       GameManager.instance.AddDiamond(DataHolder.Quest(GameManager.instance.questId).diamondValue);
        GameManager.instance.GetPet(0).Exp += DataHolder.Quest(GameManager.instance.questId).expValue;

        Debug.Log("Exp " + GameManager.instance.GetPet(0).Exp);
           

        GameManager.instance.questId++;
        isTimeline = false;
        isStartQuest = false;
        isEndQuest = false;
        isTimeline = true;
        delayTime = 0;
        replayTime = 0;

    }

    public void CheckQuest()
    {

        if (isTimeline)
            return;
        bool isComplete = false;

        if(GameManager.instance.questId == 0){
            if(GameManager.instance.GetPet(0).food > 90){
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 1){
            if(GameManager.instance.GetPet(0).water >= 90){
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 2){
            if(GameManager.instance.GetAchivement(4) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 3){
            if(GameManager.instance.GetAchivement(5) >= 1)
            {
                isComplete = true;
            }
        } else if(GameManager.instance.questId == 4){
            if(GameManager.instance.GetAchivement(3) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 5){
            if(GameManager.instance.GetAchivement(19) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 6){
            if(GameManager.instance.GetAchivement(18) >= 3)
            {
                isComplete = true;
            }
        }
        else if(GameManager.instance.questId == 7){
            if(GameManager.instance.IsEquipItem(15))
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 8){
            if(GameManager.instance.GetAchivement(17) >= 3)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 9){
            if(GameManager.instance.myPlayer.minigameLevels[0] >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 10){
            if(GameManager.instance.GetAchivement(23) >= 20)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 11){
            if(GameManager.instance.GetPet(0).level >= 3)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 12){
            if(GameManager.instance.GetAchivement(8) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 13){
            if(GameManager.instance.GetAchivement(21) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 14){
            if(GameManager.instance.GetAchivementCollectTime() >= 5)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 15){
            if(GameManager.instance.GetPets().Count >= 2)
            {
                isComplete = true;
            }
        }                  
        
        if (isComplete)
        {
            StartCompleteQuest();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive)
            return;

        if (!isStartQuest)
        {
            if(GameManager.instance.questId >= DataHolder.Quests().GetDataCount()){
                isActive = false;
                return;
            }
                
            StartQuest();
        }
        else
        {
            if (!isEndQuest)
            {
                if(!isTimeline && isReplay){
                    if(replayTime > maxReplayTime){
                        ReplayQuest();
                    }else
                        replayTime += Time.deltaTime;
                }

                if (time > maxTimeCheck)
                {
                    CheckQuest();
                    time = 0;
                }
                else
                    time += Time.deltaTime;
            }
        }
    }

    public void OnQuestNotification()
    {
        tipUI = UIManager.instance.OnQuestNotificationPopup(DataHolder.Quest(GameManager.instance.questId).GetDescription(0));
    }
}
