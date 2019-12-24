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
        if(GameManager.instance.myPlayer.questId == 0){
            GameManager.instance.GetActivePet().Food = 0.3f* GameManager.instance.GetActivePet().MaxFood;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 1){
            GameManager.instance.GetActivePet().Water = 0.3f * GameManager.instance.GetActivePet().MaxWater;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Drink));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 2){
            GameManager.instance.GetActivePet().Pee = GameManager.instance.GetActivePet().MaxPee - 3;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Toilet));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 3){
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty - 2;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bath));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 4){
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
            yield return new WaitForSeconds(1);
            GameManager.instance.ResetCameraTarget();
            ItemDirty go = GameObject.FindObjectOfType<ItemDirty>();
            if(go != null){
                GameManager.instance.SetCameraTarget(go.gameObject);
                yield return new WaitForSeconds(1);
                GameManager.instance.ResetCameraTarget();
            }
        }else if(GameManager.instance.myPlayer.questId == 6){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivement(18);
        }
        else if (GameManager.instance.myPlayer.questId == 7)
        {

        }
        else if(GameManager.instance.myPlayer.questId == 8){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivement(17);
        }
        else if(GameManager.instance.myPlayer.questId == 9){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.myPlayer.minigameLevels[0];
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
            GameManager.instance.GetActivePet().Health = 0.3f * GameManager.instance.GetActivePet().MaxHealth;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.MedicineBox));
            yield return new WaitForEndOfFrame();
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.myPlayer.questId == 13){
            GameManager.instance.GetActivePet().Damage = 0.7f * GameManager.instance.GetActivePet().MaxDamage;
        }else if(GameManager.instance.myPlayer.questId == 14){
            if (GameManager.instance.myPlayer.questValue == 0)
                GameManager.instance.myPlayer.questValue = GameManager.instance.GetAchivementCollectTime();
        }
        else if(GameManager.instance.myPlayer.questId == 15){
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
            if(GameManager.instance.GetActivePet().food > 90){
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 1){
            if(GameManager.instance.GetActivePet().water >= 90){
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 2){
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
            if(GameManager.instance.GetActivePet().level >= 1 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 12){
            if(GameManager.instance.GetAchivement(8) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 13){
            if(GameManager.instance.GetAchivement(21) >= 1)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 14){
            if(GameManager.instance.GetAchivementCollectTime() >= 1 + GameManager.instance.myPlayer.questValue)
            {
                isComplete = true;
            }
        }else if(GameManager.instance.myPlayer.questId == 15){
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
