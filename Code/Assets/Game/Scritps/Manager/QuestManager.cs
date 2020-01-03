using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public bool isStartQuest = false;
    public bool isEndQuest = false;
    bool isTimeline = true;
    bool isComplete = false;
    System.DateTime startTime;
    float time;
    float maxTimeCheck = 0.2f;
    NotificationPopup tipUI;
    float delayTime = 0;
    bool isActive = true;

    float replayTime = 0;
    float maxReplayTime = 60;

    float fadeDuration = 1f;

    bool isReplay = true;

    QuestPanel questPanel;
    float originalValue = 0;

    GameObject guideItem;
    GameObject guideItem1;
    GameObject guideItem2;

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
        CheckQuest();
    }

    void StartQuest()
    {
        if (!isComplete)
        {
            LoadQuestObject();
            PlayTip();
            isStartQuest = true;
            startTime = System.DateTime.Now;
        }
    }

    void LoadQuestObject()
    {
        delayTime = 1;
        if (GameManager.instance.myPlayer.questId == 0){
            GameManager.instance.GetActivePet().Food = 0.05f * GameManager.instance.GetActivePet().MaxFood;
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty * 0.65f;
        }
        else if(GameManager.instance.myPlayer.questId == 1){
        }
        else if(GameManager.instance.myPlayer.questId == 2){
            delayTime = 2;
        }
        else if(GameManager.instance.myPlayer.questId == 3){
        }else if(GameManager.instance.myPlayer.questId == 4){
        }else if(GameManager.instance.myPlayer.questId == 5){
        }else if(GameManager.instance.myPlayer.questId == 6){
        }else if(GameManager.instance.myPlayer.questId == 7){
        }
       
    }

    void PlayTip()
    {
        StartCoroutine(PlayTimeline());
    }

    IEnumerator PlayTimeline()
    {
        yield return new WaitForSeconds(delayTime);

       // if(!isComplete)
         OnQuestNotification();
        if (TutorialManager.instance != null)
            TutorialManager.instance.StartQuest();
        if(GameManager.instance.myPlayer.questId == 0){

        }
        else if(GameManager.instance.myPlayer.questId == 1){
            ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().foodAmount = 0;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
            yield return new WaitForSeconds(1);
            GameManager.instance.ResetCameraTarget();
        }
        else if(GameManager.instance.myPlayer.questId == 2){

        }else if(GameManager.instance.myPlayer.questId == 3){
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Bath);
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bath));
            yield return new WaitForSeconds(0.1f);
            GameManager.instance.ResetCameraTarget();
        }
        else if(GameManager.instance.myPlayer.questId == 4){

        }

        isTimeline = false;
    }

    public void ReplayQuest()
    {
        replayTime = 0;
        isTimeline = true;
        OnQuestNotification();
        if (TutorialManager.instance != null)
            TutorialManager.instance.StartQuest();
    }

    public void StartCompleteQuest()
    {
        if (TutorialManager.instance != null)
            TutorialManager.instance.EndQuest();
        if(questPanel == null && !isEndQuest){
            questPanel = UIManager.instance.OnQuestCompletePopup();
            questPanel.Load(GameManager.instance.myPlayer.questId);
            Debug.Log("Quest Complete");
        }
        isEndQuest = true;

    }

    public void EndCompleteQuest()
    {
        if (DataHolder.Quest(GameManager.instance.myPlayer.questId).haveItem)
        {
           GameManager.instance.AddItem(DataHolder.Quest(GameManager.instance.myPlayer.questId).itemId);
           GameManager.instance.EquipItem(DataHolder.Quest(GameManager.instance.myPlayer.questId).itemId);
            ItemManager.instance.LoadItems();
        }

        if(UIManager.instance.questNotification != null)
            UIManager.instance.questNotification.Close();

        GameManager.instance.AddCoin(DataHolder.Quest(GameManager.instance.myPlayer.questId).coinValue);
        GameManager.instance.AddDiamond(DataHolder.Quest(GameManager.instance.myPlayer.questId).diamondValue);
        GameManager.instance.AddExp(DataHolder.Quest(GameManager.instance.myPlayer.questId).expValue,GameManager.instance.GetActivePet().iD,false);

        if (guideItem != null)
            Destroy(guideItem);
        if (guideItem1 != null)
            Destroy(guideItem1);
        if (guideItem2 != null)
            Destroy(guideItem2);

        GameManager.instance.myPlayer.questId++;
        GameManager.instance.SavePlayer();
        isTimeline = false;
        isStartQuest = false;
        isEndQuest = false;
        isTimeline = true;
        isComplete = false;
        delayTime = 0;
        replayTime = 0;
        GameManager.instance.myPlayer.questValue = 0;

    }

    public void CheckQuest()
    {

       // if (isTimeline)
       //     return;

        if(GameManager.instance.myPlayer.questId == 0){
            if (GameManager.instance.IsEquipItem(41))
            {
                isComplete = true;
            }
        }
        else if(GameManager.instance.myPlayer.questId == 1){
            if (ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().foodAmount > ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().maxfoodAmount - 5)
            {
                isComplete = true;
            }
        }
        else if(GameManager.instance.myPlayer.questId == 2){
            if(GameManager.instance.GetAchivement(20) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 3){
            if(GameManager.instance.GetActivePet().Dirty < 40)
            {
                isComplete = true;
            }
        } else if(GameManager.instance.myPlayer.questId == 4){
            if (GameManager.instance.GetPets().Count >= 2)
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
            if(GameManager.instance.myPlayer.questId >= DataHolder.Quests().GetDataCount()){
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
                    if(replayTime > maxReplayTime && GameManager.instance.myPlayer.questId < DataHolder.Quests().GetDataCount() - 1)
                    {
                        ReplayQuest();
                        //OnQuestNotification();
                        replayTime = 0;
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
        if(GameManager.instance.myPlayer.questId < DataHolder.Quests().GetDataCount())
            tipUI = UIManager.instance.OnQuestNotificationPopup(DataHolder.Quest(GameManager.instance.myPlayer.questId).GetDescription(MageManager.instance.GetLanguage()));
    }
}
