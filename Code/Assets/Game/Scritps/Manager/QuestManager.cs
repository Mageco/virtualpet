using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public bool isStartQuest = false;
    public bool isEndQuest = false;
    bool isTimeline = false;
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
    bool isReward = false;

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
    IEnumerator Start()
    {
        while (!GameManager.instance.isLoad)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckQuest();
    }

    void StartQuest()
    {
        CheckQuest();
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
        
        isReward = false;
        isReplay = false;
        delayTime = 2;
        if (GameManager.instance.myPlayer.questId == 0){
            delayTime = 0;
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty * 0.7f;
            GameManager.instance.GetActivePet().Food = 0.05f * GameManager.instance.GetActivePet().MaxFood;
            GameManager.instance.GetActivePet().Water = 0.05f * GameManager.instance.GetActivePet().MaxWater;
            ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().foodAmount = 0;
        }
        else if(GameManager.instance.myPlayer.questId == 1){
            delayTime = 0;
        }
        else if(GameManager.instance.myPlayer.questId == 2){
            GameManager.instance.GetActivePet().Water = 0.05f * GameManager.instance.GetActivePet().MaxWater;
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty * 0.7f;
        }
        else if(GameManager.instance.myPlayer.questId == 3){
            GameManager.instance.GetActivePet().Water = 0.05f * GameManager.instance.GetActivePet().MaxWater;
        }
        else if(GameManager.instance.myPlayer.questId == 4){
        }else if(GameManager.instance.myPlayer.questId == 5){
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty * 0.7f;
        }
        else if(GameManager.instance.myPlayer.questId == 6){
            GameManager.instance.GetActivePet().Dirty = GameManager.instance.GetActivePet().MaxDirty * 0.8f;
            GameManager.instance.GetActivePet().Pee = GameManager.instance.GetActivePet().MaxPee * 0.65f;
            GameManager.instance.GetActivePet().Shit = GameManager.instance.GetActivePet().MaxShit * 0.1f;
            isReplay = true;
        }
        else if(GameManager.instance.myPlayer.questId == 7){
            delayTime = 0;
            GameManager.instance.GetActivePet().Pee = GameManager.instance.GetActivePet().MaxPee * 0.95f;
        }
        else if (GameManager.instance.myPlayer.questId == 8)
        {

        }
        else if (GameManager.instance.myPlayer.questId == 9)
        {

        }
        else if (GameManager.instance.myPlayer.questId == 10)
        {

        }
        else if (GameManager.instance.myPlayer.questId == 11)
        {
            GameManager.instance.GetActivePet().Shit = GameManager.instance.GetActivePet().MaxShit * 0.7f;
        }
        else if (GameManager.instance.myPlayer.questId == 12)
        {
            GameManager.instance.GetActivePet().Shit = GameManager.instance.GetActivePet().MaxShit * 0.71f;
        }
        else if (GameManager.instance.myPlayer.questId == 13)
        {
            GameManager.instance.GetActivePet().Sleep = GameManager.instance.GetActivePet().MaxSleep * 0.1f;
        }
        else if (GameManager.instance.myPlayer.questId == 14)
        {
            GameManager.instance.GetActivePet().Sleep = GameManager.instance.GetActivePet().MaxSleep * 0.05f;
            GameManager.instance.GetActivePet().Food = 0.6f * GameManager.instance.GetActivePet().MaxFood;
            GameManager.instance.GetActivePet().Water = 0.6f * GameManager.instance.GetActivePet().MaxWater;
        }
        else if (GameManager.instance.myPlayer.questId == 15)
        {
            delayTime = 5;
        }
        else if (GameManager.instance.myPlayer.questId == 16)
        {

        }
        else if (GameManager.instance.myPlayer.questId == 17)
        {
            GameManager.instance.GetActivePet().Sleep = GameManager.instance.GetActivePet().MaxSleep * 0.95f;
        }
        else if (GameManager.instance.myPlayer.questId == 18)
        {

        }
        else if (GameManager.instance.myPlayer.questId == 19)
        {
            delayTime = 120;
        }
        else if (GameManager.instance.myPlayer.questId == 21)
        {
            isReward = true;
        }


    }

    void PlayTip()
    {
        StartCoroutine(PlayTimeline());
    }

    IEnumerator PlayTimeline()
    {
        isTimeline = true;
        yield return new WaitForSeconds(delayTime);

       // if(!isComplete)
        OnQuestNotification();
        if(GameManager.instance.myPlayer.questId != 0)
            yield return new WaitForSeconds(2);
        if (TutorialManager.instance != null)
            TutorialManager.instance.StartQuest();
        if(GameManager.instance.myPlayer.questId == 0)
        {
            ItemManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
            GameManager.instance.GetActivePet().character.OnEat();
            yield return new WaitForSeconds(1);
            ItemManager.instance.ResetCameraTarget();
        }
        else if(GameManager.instance.myPlayer.questId == 1){
            if(FindObjectOfType<HappyItem>() != null)
            {
                ItemManager.instance.SetCameraTarget(FindObjectOfType<HappyItem>().gameObject);
                yield return new WaitForSeconds(1);
                ItemManager.instance.ResetCameraTarget();
            }
        }
        else if(GameManager.instance.myPlayer.questId == 2){

        }else if(GameManager.instance.myPlayer.questId == 3){
            ItemManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Drink));
            GameManager.instance.GetActivePet().character.OnDrink();
            yield return new WaitForSeconds(1);
            ItemManager.instance.ResetCameraTarget();
        }
        else if(GameManager.instance.myPlayer.questId == 4){

        }
        else if (GameManager.instance.myPlayer.questId == 5)
        {

        }
        else if (GameManager.instance.myPlayer.questId == 6)
        {
            if (guideItem == null)
                guideItem = ItemManager.instance.SpawnGuideArrow(ItemType.Bath);
            ItemManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bath));
            yield return new WaitForSeconds(0.1f);
            ItemManager.instance.ResetCameraTarget();
        }
        else if (GameManager.instance.myPlayer.questId == 19)
        {
            GameObject door = GameObject.FindGameObjectWithTag("Door");
            if (guideItem == null && door != null)
            {
                Vector3 pos = door.transform.position;
                pos.z = pos.y * 10;
                guideItem = ItemManager.instance.SpawnGuideArrow(door, pos);
            }
                
            GameManager.instance.GetActivePet().Sleep = GameManager.instance.GetActivePet().MaxSleep * 0.6f;
            GameManager.instance.GetActivePet().Food = 0.6f * GameManager.instance.GetActivePet().MaxFood;
            GameManager.instance.GetActivePet().Water = 0.6f * GameManager.instance.GetActivePet().MaxWater;
        }

        isTimeline = false;
    }

    public void ReplayQuest()
    {
        replayTime = 0;
        OnQuestNotification();
        if (TutorialManager.instance != null)
            TutorialManager.instance.StartQuest();
    }

    public void StartCompleteQuest()
    {
        if (UIManager.instance.questNotification != null)
            UIManager.instance.questNotification.Close();

        if (TutorialManager.instance != null)
            TutorialManager.instance.EndQuest();
        if (isReward)
        {
            if(questPanel == null && !isEndQuest){
                questPanel = UIManager.instance.OnQuestCompletePopup();
                questPanel.Load(GameManager.instance.myPlayer.questId);
                Debug.Log("Quest Complete");
            }
        }
        else
        {
            isEndQuest = true;
            EndCompleteQuest();
        }

    }

    public void EndCompleteQuest()
    {
        if (DataHolder.Quest(GameManager.instance.myPlayer.questId).haveItem)
        {
           GameManager.instance.AddItem(DataHolder.Quest(GameManager.instance.myPlayer.questId).itemId);
           GameManager.instance.EquipItem(DataHolder.Quest(GameManager.instance.myPlayer.questId).itemId);
           ItemManager.instance.LoadItems(true);
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
        isTimeline = false;
        isComplete = false;
        delayTime = 0;
        replayTime = 0;
        GameManager.instance.myPlayer.questValue = 0;

    }

    public void CheckQuest()
    {

        if (isTimeline)
            return;

        if (GameManager.instance.myPlayer.questId == 0) {
            if(GameManager.instance.GetAchivement(1) >= 1 || ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().foodAmount >= ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<FoodBowlItem>().maxfoodAmount - 2)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 1) {
            if (GameManager.instance.GetAchivement(23) >= 1)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 2) {
            if (GameManager.instance.IsEquipItem(58))
            {
                isComplete = true;
            }
        } else if (GameManager.instance.myPlayer.questId == 3) {
            if (GameManager.instance.GetAchivement(2) >= 1 || ItemManager.instance.GetItemChildObject(ItemType.Drink).GetComponent<DrinkBowlItem>().foodAmount >= ItemManager.instance.GetItemChildObject(ItemType.Drink).GetComponent<DrinkBowlItem>().maxfoodAmount - 2)
            {
                isComplete = true;
            }
        } else if (GameManager.instance.myPlayer.questId == 4) {
            if (GameManager.instance.GetAchivement(23) >= 2)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 5)
        {
            if (GameManager.instance.IsEquipItem(2))
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 6)
        {
            if (GameManager.instance.GetActivePet().dirty < 30)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 7)
        {
            if (GameManager.instance.GetAchivement(23) >= 8)
            {
                isComplete = true;
            }

        }
        else if (GameManager.instance.myPlayer.questId == 8)
        {
            if (GameManager.instance.GetActivePet().enviromentType == EnviromentType.Room && FindObjectOfType<ItemDirty>() != null)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 9)
        {
            if (GameManager.instance.IsEquipItem(59))
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 10)
        {
            if (FindObjectOfType<ItemDirty>() == null)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 11)
        {
            if (GameManager.instance.IsEquipItem(11))
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 12)
        {
            if (GameManager.instance.GetAchivement(4) >= 1)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 13)
        {
            if (GameManager.instance.IsEquipItem(4))
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 14)
        {
            if (GameManager.instance.GetActivePet().enviromentType == EnviromentType.Bed && GameManager.instance.GetActivePet().actionType == ActionType.Sleep)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 15)
        {
            if (UIManager.instance.petRequirementPanel != null)
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 16)
        {
            if (GameManager.instance.IsEquipItem(1) && GameManager.instance.IsEquipItem(8))
            {
                isComplete = true;
            }

        }
        else if (GameManager.instance.myPlayer.questId == 17)
        {
            if (GameManager.instance.GetHappy() >= DataHolder.GetPet(1).buyPrice)
            {
                isComplete = true;
            }
        } else if (GameManager.instance.myPlayer.questId == 18) { 
            if (GameManager.instance.IsEquipPet(1))
            {
                isComplete = true;
            }
        }
        else if (GameManager.instance.myPlayer.questId == 19)
        {
            isComplete = true;
        }else if (GameManager.instance.myPlayer.questId == 20)
        {
            isComplete = true;
        }
        else if (GameManager.instance.myPlayer.questId == 21)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                isComplete = true;
            }
        }

        if (isComplete)
        {
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.Quest, GameManager.instance.myPlayer.questId.ToString());
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
