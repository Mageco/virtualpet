using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public bool isStartQuest = false;
    public bool isEndQuest = false;
    public bool haveTimeline = false;
    PlayableDirector playTimeLine;
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

    bool isReplay = false;

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

        isReplay = DataHolder.GetQuest(GameManager.instance.questId).isReplay;
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
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 1){
            GameManager.instance.GetPet(0).Water = 30;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Drink));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 2){
            GameManager.instance.GetPet(0).Pee = 70;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Toilet));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 3){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Clean));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 4){
            GameManager.instance.GetPet(0).dirty = 70;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bath));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 5){
        }else if(GameManager.instance.questId == 6){
        }else if(GameManager.instance.questId == 8){
        }else if(GameManager.instance.questId == 9){
        }else if(GameManager.instance.questId == 10){
        }else if(GameManager.instance.questId == 11){
            GameManager.instance.GetPet(0).Health = 30;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.MedicineBox));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 12){
            GameManager.instance.GetPet(0).Damage = 70;
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.MedicineBox));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 13){

        }

        isTimeline = false;
    }

    public void ReplayQuest()
    {
        replayTime = 0;
        isTimeline = true;
        if(playTimeLine != null)
            playTimeLine.time = 0;
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
           

        if (playTimeLine != null)
            Destroy(playTimeLine.gameObject);

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
            if(GameManager.instance.GetPet(0).sleep >= 90){
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 3){
            if(ItemManager.instance.GetItem(ItemType.Food) != null && ItemManager.instance.GetItem(ItemType.Food).GetComponentInChildren<FoodBowlItem>().foodAmount > 90){
                //GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        } else if(GameManager.instance.questId == 4){
            if(ItemManager.instance.GetItem(ItemType.Drink) != null && ItemManager.instance.GetItem(ItemType.Drink).GetComponentInChildren<DrinkBowlItem>().foodAmount > 90){
                //GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 5){
            if(GameManager.instance.GetPet(0).Dirty <= 50){
                //GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 6){
            if(GameManager.instance.GetPet(0).level >=  5){
                isComplete = true;
            }
        }
        else if(GameManager.instance.questId == 7){
            if(GameObject.FindObjectOfType<ItemDirty>() == null){
                GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 8){
            if(GameManager.instance.GetPet(0).level >=  6){
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
        tipUI = UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(DataHolder.Quest(GameManager.instance.questId).dialogId).GetDescription(0));
    }
}
