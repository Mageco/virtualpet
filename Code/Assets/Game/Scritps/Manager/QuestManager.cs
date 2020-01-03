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
        if(GameManager.instance.myPlayer.questId == 0){
        }else if(GameManager.instance.myPlayer.questId == 1){
        }else if(GameManager.instance.myPlayer.questId == 2){
        }else if(GameManager.instance.myPlayer.questId == 3){
        }else if(GameManager.instance.myPlayer.questId == 4){
        }else if(GameManager.instance.myPlayer.questId == 5){
        }else if(GameManager.instance.myPlayer.questId == 6){
        }else if(GameManager.instance.myPlayer.questId == 7){
        }
        delayTime = 1;
    }

    void PlayTip()
    {
        StartCoroutine(PlayTimeline());
    }

    IEnumerator PlayTimeline()
    {
        yield return new WaitForSeconds(delayTime);
        
        OnQuestNotification();
        if (TutorialManager.instance != null)
            TutorialManager.instance.StartQuest();
        if(GameManager.instance.myPlayer.questId == 0){
            GameManager.instance.GetActivePet().Food = 0.05f* GameManager.instance.GetActivePet().MaxFood;
        }else if(GameManager.instance.myPlayer.questId == 1){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }
        else if(GameManager.instance.myPlayer.questId == 2){
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Toilet);

            GameManager.instance.GetActivePet().Pee = 0.7f*GameManager.instance.GetActivePet().MaxPee;
            GameManager.instance.GetActivePet().Shit = 0.7f * GameManager.instance.GetActivePet().MaxShit - 3;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Toilet));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 3){
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Bath);
            if (guideItem1 == null)
                guideItem1 = ItemManager.instance.SpawnGuideArrow(FindObjectOfType<SoapItem>().gameObject, FindObjectOfType<SoapItem>().gameObject.transform.position);
            if (guideItem2 == null)
                guideItem1 = ItemManager.instance.SpawnGuideArrow(FindObjectOfType<BathShowerItem>().gameObject, FindObjectOfType<BathShowerItem>().gameObject.transform.position + new Vector3(0,-20,0));
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bath));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 4){
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Bed);
            GameManager.instance.GetActivePet().Sleep = 0.3f * GameManager.instance.GetActivePet().MaxSleep;
            originalValue = GameManager.instance.GetActivePet().rateSleep;
            GameManager.instance.GetActivePet().rateSleep = 4;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bed));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 5){
            if(GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivement(19);
            ItemManager.instance.SpawnDirty();
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Clean));
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Clean);
            yield return new WaitForSeconds(1f);
            GameManager.instance.ResetCameraTarget();
            ItemDirty go = FindObjectOfType<ItemDirty>();
            if(go != null){
                GameManager.instance.SetCameraTarget(go.gameObject);
                yield return new WaitForSeconds(0.1f);
                GameManager.instance.ResetCameraTarget();
            }
        }else if(GameManager.instance.myPlayer.questId == 6){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivement(18);

        }
        else if (GameManager.instance.myPlayer.questId == 7)
        {
            yield return new WaitForSeconds(4);
            UIManager.instance.OnShopPanel(4);
        }
        else if(GameManager.instance.myPlayer.questId == 8){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivement(17);
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Toy);
        }
        else if(GameManager.instance.myPlayer.questId == 9){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.myPlayer.minigameLevels[0];
            yield return new WaitForSeconds(4);
            UIManager.instance.OnEventPanel();
        }
        else if(GameManager.instance.myPlayer.questId == 10){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivement(23);
        }
        else if(GameManager.instance.myPlayer.questId == 11){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetActivePet().level;
        }
        else if(GameManager.instance.myPlayer.questId == 12){
            GameManager.instance.myPlayer.questValue = GameManager.instance.GetPets().Count;
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
        delayTime = 0;
        replayTime = 0;
        GameManager.instance.myPlayer.questValue = 0;

    }

    public void CheckQuest()
    {

        if (isTimeline)
            return;
        bool isComplete = false;

        if(GameManager.instance.myPlayer.questId == 0){
            if (GameManager.instance.IsEquipItem(41))
            {
                isComplete = true;
            }
        }
        else if(GameManager.instance.myPlayer.questId == 1){
            if (ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().foodAmount > ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().maxfoodAmount - 1)
            {
                isComplete = true;
            }
        }
        else if(GameManager.instance.myPlayer.questId == 2){
            if(GameManager.instance.GetAchivement(4) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 3){
            if(GameManager.instance.GetAchivement(5) >= 1)
            {
                isComplete = true;
            }
        } else if(GameManager.instance.myPlayer.questId == 4){
            if(GameManager.instance.GetAchivement(3) >= 1)
            {
                isComplete = true;
                GameManager.instance.GetActivePet().rateSleep = originalValue;
            }
        }else if(GameManager.instance.myPlayer.questId == 5){
            if(GameManager.instance.GetAchivement(19) >= 1 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 6){
            if(GameManager.instance.GetAchivement(18) >= 3 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }
        else if(GameManager.instance.myPlayer.questId == 7){
            if(GameManager.instance.IsEquipItem(15))
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 8){
            if(GameManager.instance.GetAchivement(17) >= 3 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 9){
            if(GameManager.instance.myPlayer.minigameLevels[0] >= 1 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 10){
            if(GameManager.instance.GetAchivement(23) >= 5 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 11){
            if (GameManager.instance.GetActivePet().level >= 1 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 12){
            if(GameManager.instance.GetPets().Count >= 1 + GameManager.instance.myPlayer.questValue)
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
                        OnQuestNotification();
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
