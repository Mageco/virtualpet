using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;


public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public QuestState state = QuestState.None;
    float time;
    float maxTimeCheck = 0.2f;
    float delayTime = 0;
    public float replayTime = 0;
    public float maxReplayTime = 30;
    public bool isReplay = true;
    public Sprite[] coinIcons;
    public bool isReplaying = false;
    public int questId = 0;
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
        while (!ItemManager.instance.isLoad)
        {
            yield return new WaitForEndOfFrame();
        }

        if (!GameManager.instance.IsOldVersion())
            StartCoroutine(StartQuest());
    }    


    IEnumerator StartQuest()
    {
        state = QuestState.Ready;
        
        questId = GameManager.instance.myPlayer.questId;
        CharController petObject = GameManager.instance.GetActivePetObject();

        CheckQuestComplete();
        maxReplayTime = 30;
        if (petObject != null && state != QuestState.Complete && questId < GameManager.instance.questMax)
        {
            if (questId == 0)
            {
                isReplay = false;
                yield return new WaitForSeconds(1);
                if (!isReplaying)
                {
                    petObject.ResetData();
                    petObject.data.Food = petObject.data.MaxFood * 0.09f;
                }

                UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(187).GetName(MageManager.instance.GetLanguage()));
                yield return new WaitForSeconds(5);
                UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(196).GetName(MageManager.instance.GetLanguage()));
                yield return new WaitForSeconds(6);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
                OnQuestNotification();
            }
            else if (questId == 1)
            {
                yield return new WaitForSeconds(1);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 2)
            {
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                if (petObject.actionType != ActionType.Drink)
                    OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
                if(!isReplaying)
                    petObject.data.Water = petObject.data.MaxWater * 0.09f;
            }
            else if (questId == 3)
            {
                if (isReplaying)
                {
                    yield return new WaitForSeconds(1);
                    OnCompleteQuest();
                } 
                else
                {
                    yield return new WaitForSeconds(5);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                    MouseController mouse = FindObjectOfType<MouseController>();
                    if (mouse != null)
                        mouse.maxTimeSpawn = 0;
                }

            }
            else if (questId == 4)
            {
                isReplay = false;
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
                if (!isReplaying)
                {
                    petObject.ResetData();
                    petObject.data.Dirty = petObject.data.MaxDirty * 0.91f;
                }
            }
            else if (questId == 5)
            {
                if (isReplaying)
                    OnCompleteQuest();
                else
                {
                    yield return new WaitForSeconds(1);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    if (petObject.equipment == null || petObject.equipment.itemType != ItemType.Bath)
                        OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                    if (!isReplaying)
                    {
                        petObject.ResetData();
                        petObject.data.Dirty = petObject.data.MaxDirty * 0.91f;
                    }
                }
            }
            else if (questId == 6)
            {
                if (isReplaying)
                    OnCompleteQuest();
                else
                {
                    yield return new WaitForSeconds(6);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                    BeeController bee = FindObjectOfType<BeeController>();
                    if (bee != null)
                        bee.maxTimeSpawn = 0;
                }
            }
            else if (questId == 7)
            {
                if (!isReplaying)
                {
                    petObject.ResetData();
                    petObject.data.Toy = petObject.data.MaxToy * 0.25f;
                }

                isReplay = false;
                yield return new WaitForSeconds(2);
                petObject.OnCall();
                yield return new WaitForSeconds(3);
                petObject.OnCall();
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 8)
            {
                if (isReplaying)
                    OnCompleteQuest();
                else
                {
                    yield return new WaitForSeconds(1);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    if (petObject.equipment == null || petObject.equipment.itemType != ItemType.Toy)
                        OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                    if (!isReplaying)
                    {
                        petObject.ResetData();
                        petObject.data.Toy = petObject.data.MaxToy * 0.25f;
                    }
                }
            }
            else if (questId == 9)
            {

                if (isReplaying)
                {
                    yield return new WaitForSeconds(1);
                    OnCompleteQuest();
                }
                else
                {
                    yield return new WaitForSeconds(5);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                    MouseController mouse = FindObjectOfType<MouseController>();
                    if (mouse != null)
                        mouse.maxTimeSpawn = 0;
                }
                /*
                if (isReplaying)
                {
                    petObject.OnHealth(SickType.Sick, 100);
                    OnCompleteQuest();
                }
                else
                {
                    yield return new WaitForSeconds(10);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    OnQuestNotification();
                    ItemManager.instance.SetCameraTarget(ItemManager.instance.GetRandomItem(ItemType.MedicineBox).gameObject);
                    yield return new WaitForSeconds(1);
                    ItemManager.instance.ResetCameraTarget();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                    
                    petObject.data.Health = petObject.data.MaxHealth * 0.09f;
                    
                }

                yield return new WaitForSeconds(5);
                UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(189).GetName(MageManager.instance.GetLanguage()));
                */
            }
            else if (questId == 10)
            {

                if (isReplaying)
                {
                    OnCompleteQuest();
                }
                else
                {
                    CharCollector[] charCollectors = FindObjectsOfType<CharCollector>();
                    for(int i = 0; i < charCollectors.Length; i++)
                    {
                        if (charCollectors[i].petId == 1)
                            charCollectors[i].Active();
                    }
                    yield return new WaitForSeconds(5);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                }
                /*
                isReplay = false;
                yield return new WaitForSeconds(2);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();

                petObject.data.Shit = petObject.data.MaxShit * 0.7f;
                petObject.data.Pee = petObject.data.MaxPee * 0.7f;
                */
            }
            else if (questId == 11)
            {
                isReplay = false;
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 12)
            {
                isReplay = false;
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();

            }
            else if (questId == 13)
            {
                isReplay = false;
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                yield return new WaitForSeconds(4);
                if (UIManager.instance.questNotification != null)
                    UIManager.instance.questNotification.Close();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 14)
            {
                if (isReplaying)
                {
                    OnCompleteQuest();
                }
                else
                {
                    yield return new WaitForSeconds(10);
                    while (UIManager.instance.IsPopUpOpen())
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    OnQuestNotification();
                    ItemManager.instance.SetCameraTarget(GameObject.FindGameObjectWithTag("LuckySpin"));
                    yield return new WaitForSeconds(1);
                    ItemManager.instance.ResetCameraTarget();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                }
            }
            else if (questId == 15)
            {
                isReplay = false;
                yield return new WaitForSeconds(3);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 16)
            {
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                if (GameManager.instance.IsHaveItem(87))
                    UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(194).GetName(MageManager.instance.GetLanguage()));
                else
                {
                    OnQuestNotification();
                    if (TutorialManager.instance != null)
                        TutorialManager.instance.StartQuest();
                }

                
            }
            else if (questId == 17)
            {
                isReplay = false;
                yield return new WaitForSeconds(5);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 18)
            {
                isReplay = false;
                yield return new WaitForSeconds(10);
                while (UIManager.instance.IsPopUpOpen())
                {
                    yield return new WaitForEndOfFrame();
                }
                OnQuestNotification();
                ItemManager.instance.SetCameraTarget(FindObjectOfType<OnMapButton>().gameObject);
                yield return new WaitForSeconds(1);
                ItemManager.instance.ResetCameraTarget();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
        }
        if(state != QuestState.Complete)
            state = QuestState.Start;
    }

    public void ReplayQuest()
    {
        isReplaying = true;
        replayTime = 0;
        StartCoroutine(StartQuest());
    }

    IEnumerator StartCompleteQuest()
    {
        CharController petObject = GameManager.instance.GetActivePetObject();
        state = QuestState.Rewarded;
        yield return new WaitForSeconds(1);
        while (UIManager.instance.IsPopUpOpen())
        {
            yield return new WaitForEndOfFrame();
        }

        Quest quest = DataHolder.Quest(GameManager.instance.myPlayer.questId);
        if (quest.coinValue > 0)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(186).GetName(MageManager.instance.GetLanguage()));
            yield return new WaitForSeconds(1);
            UIManager.instance.OnSpinRewardPanel(coinIcons[0], quest.coinValue.ToString(), DataHolder.Dialog(6).GetName(MageManager.instance.GetLanguage()));
            GameManager.instance.AddCoin(quest.coinValue, GetKey());
            
        } else if (quest.diamondValue > 0)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(186).GetName(MageManager.instance.GetLanguage()));
            yield return new WaitForSeconds(1);
            UIManager.instance.OnSpinRewardPanel(coinIcons[1], quest.diamondValue.ToString(), DataHolder.Dialog(7).GetName(MageManager.instance.GetLanguage()));
            GameManager.instance.AddDiamond(quest.diamondValue, GetKey());
        }
        else if (quest.happyValue > 0)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(186).GetName(MageManager.instance.GetLanguage()));
            yield return new WaitForSeconds(1);
            UIManager.instance.OnSpinRewardPanel(coinIcons[2], quest.happyValue.ToString(), DataHolder.Dialog(8).GetName(MageManager.instance.GetLanguage()));
            GameManager.instance.AddHappy(quest.happyValue, GetKey());
        }
        else if (quest.expValue > 0)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(186).GetName(MageManager.instance.GetLanguage()));
            yield return new WaitForSeconds(1);
            UIManager.instance.OnSpinRewardPanel(coinIcons[3], quest.expValue.ToString(),"Exp");
            GameManager.instance.AddExp(quest.expValue, GetKey());
        }
        else if(quest.haveItem)
        {
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(186).GetName(MageManager.instance.GetLanguage()));
            yield return new WaitForSeconds(1);
            Item item = DataHolder.GetItem(quest.itemId);
            string url = item.iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url), quest.itemNumber.ToString(), item.GetName(MageManager.instance.GetLanguage()));
            int realId = GameManager.instance.AddItem(quest.itemId,quest.itemNumber, GetKey());
            GameManager.instance.EquipItem(realId);
            Debug.Log("Reward " + realId);
        }

        ItemManager.instance.SpawnStar(petObject.transform.position, 2);
        if (TutorialManager.instance != null)
            TutorialManager.instance.EndQuest();
        yield return new WaitForSeconds(2);
        if (UIManager.instance.spinRewardPanel != null)
            UIManager.instance.spinRewardPanel.Close();

        if (UIManager.instance.questNotification != null)
             UIManager.instance.questNotification.Close();

        if(GameManager.instance.myPlayer.questId == 1)
        {
            GameManager.instance.myPlayer.questId = 4;
        }else if(GameManager.instance.myPlayer.questId == 5)
        {
            GameManager.instance.myPlayer.questId = 7;
        }
        else if (GameManager.instance.myPlayer.questId == 5)
        {
            GameManager.instance.myPlayer.questId = 7;
        }
        else if (GameManager.instance.myPlayer.questId == 12)
        {
            GameManager.instance.myPlayer.questId = 20;
            yield return new WaitForSeconds(3);
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(142).GetName(MageManager.instance.GetLanguage()));
        }else
            GameManager.instance.myPlayer.questId++;

        EndCompleteQuest();
    }

    public void EndCompleteQuest()
    {
        state = QuestState.Ready;
        delayTime = 0;
        replayTime = 0;
        isReplaying = false;
        isReplay = true;
        GameManager.instance.myPlayer.questValue = 0;
        StartCoroutine(StartQuest());
    }

    public void OnCompleteQuest()
    {
        state = QuestState.Complete;
    }

    public void CheckQuestComplete()
    {
        questId = GameManager.instance.myPlayer.questId;

        if (!ItemManager.instance.isLoad)
            return;

        if (questId == 0) {
            if(GameManager.instance.IsEquipItem(41))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 1) {
            EatItem food = FindObjectOfType<EatItem>();
            if (food != null && food.foodAmount > food.maxfoodAmount - 10)
            {
                state = QuestState.Complete;
            }
        } 
        else if (questId == 2)
        {
            if (GameManager.instance.GetAchivement(2) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 3)
        {
            if (GameManager.instance.GetAchivement(18) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 4)
        {
            if (GameManager.instance.IsEquipItem(2))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 5)
        {
            if (GameManager.instance.GetAchivement(5) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 6)
        {
            if (GameManager.instance.IsHaveItem(232))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 7)
        {
            if (GameManager.instance.IsEquipItem(85))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 8)
        {
            if (GameManager.instance.GetAchivement(17) >= 1)
            {
                state = QuestState.Complete;
            }

        }
        else if (questId == 9) {
            if (GameManager.instance.GetAchivement(18) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 10)
        {
            if (GameManager.instance.IsHavePet(1))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 11)
        {
            if (GameManager.instance.myPlayer.level >= 2)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 12)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 13)
        {
            foreach(PlayerPet pet in GameManager.instance.myPlayer.petDatas)
            {
                Debug.Log(pet.level);
                if (pet.level >= 5 && pet.itemState == ItemState.Equiped)
                {
                    state = QuestState.Complete;
                    return;
                }
            }
        }
        else if (questId == 14)
        {
            if (GameManager.instance.myPlayer.spinedTime != "" && UIManager.instance.spinWheelPanel == null)
                state = QuestState.Complete;
        }
        else if (questId == 15)
        {
            if (GameManager.instance.myPlayer.level >= 3)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 16)
        {
            if (GameManager.instance.IsHaveItem(204))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 17)
        {
            if (GameManager.instance.myPlayer.level >= 4)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 18)
        {
            if (GameManager.instance.GetItemNumber(233) >= 1)
            {
                state = QuestState.Complete;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time > maxTimeCheck)
        {
            time = 0;
            if (state == QuestState.Start)
            {
                CheckQuestComplete();
            }
            else if (state == QuestState.Complete)
            {
                StartCoroutine(StartCompleteQuest());
            }
        }
        else
            time += Time.deltaTime;

        if(state == QuestState.Start && isReplay)
        {
            if (replayTime > maxReplayTime)
            {
                ReplayQuest();
            }
            else
                replayTime += Time.deltaTime;
        }


    }

    

    public void OnQuestNotification()
    {
        if(GameManager.instance.myPlayer.questId < DataHolder.Quests().GetDataCount())
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Quest(GameManager.instance.myPlayer.questId).GetName(MageManager.instance.GetLanguage()));
    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }

    public void OnGift()
    {
        UIManager.instance.OnSpinRewardPanel(coinIcons[1],"20", DataHolder.Dialog(7).GetName(MageManager.instance.GetLanguage()));
        GameManager.instance.AddDiamond(20, GetKey());
    }
}
