﻿using System.Collections;
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
    float replayTime = 0;
    float maxReplayTime = 30;
    bool isReplay = true;
    public Sprite[] coinIcons;

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
        if(!GameManager.instance.IsOldVersion())
            StartCoroutine(StartQuest());
    }    


    IEnumerator StartQuest()
    {
        state = QuestState.Ready;
        isReplay = false;
        int questId = GameManager.instance.myPlayer.questId;
        CharController petObject = GameManager.instance.GetActivePetObject();

        if (petObject != null)
        {
            if (questId == 0)
            {
                petObject.ResetData();
                yield return new WaitForSeconds(1);
                UIManager.instance.OnQuestNotificationPopup("Congratulation");
                yield return new WaitForSeconds(6);
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
                OnQuestNotification();
                petObject.data.Food = 0;
            }
            else if (questId == 1)
            {
                yield return new WaitForSeconds(1);
                OnQuestNotification();
                if (TutorialManager.instance != null)
                    TutorialManager.instance.StartQuest();
            }
            else if (questId == 2)
            {
            }
            else if (questId == 3)
            {
            }
            else if (questId == 4)
            {

            }
            else if (questId == 5)
            {
            }
            else if (questId == 6)
            {
            }
            else if (questId == 7)
            {
            }
            else if (questId == 8)
            {

            }
            else if (questId == 9)
            {
            }
            else if (questId == 10)
            {
            }
            else if (questId == 11)
            {
            }
            else if (questId == 12)
            {
            }
            else if (questId == 13)
            {
            }
            else if (questId == 14)
            {
            }
            else if (questId == 15)
            {
            }
            else if (questId == 16)
            {
            }
            else if (questId == 17)
            {
            }
            else if (questId == 18)
            {
            }
            else if (questId == 19)
            {
            }
            else if (questId == 20)
            {
            }
            else if (questId == 21)
            {
            }
            else if (questId == 22)
            {
            }
        }
        state = QuestState.Start;
    }

    public void ReplayQuest()
    {
        replayTime = 0;
        OnQuestNotification();
        if (TutorialManager.instance != null)
            TutorialManager.instance.StartQuest();
    }

    IEnumerator StartCompleteQuest()
    {
        
        if (UIManager.instance.questNotification != null)
            UIManager.instance.questNotification.Close();
        UIManager.instance.OnQuestNotificationPopup("Good Job");

        Quest quest = DataHolder.Quest(GameManager.instance.myPlayer.questId);
        if (quest.coinValue > 0)
        {
            UIManager.instance.OnSpinRewardPanel(coinIcons[0], quest.coinValue.ToString());
            GameManager.instance.AddCoin(quest.coinValue, GetKey());
        } else if (quest.diamondValue > 0)
        {
            UIManager.instance.OnSpinRewardPanel(coinIcons[1], quest.diamondValue.ToString());
            GameManager.instance.AddDiamond(quest.diamondValue, GetKey());
        }
        else if(quest.haveItem)
        {
            Item item = DataHolder.GetItem(quest.itemId);
            string url = item.iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            UIManager.instance.OnSpinRewardPanel(Resources.Load<Sprite>(url), quest.itemNumber.ToString());
            GameManager.instance.AddItem(quest.itemId,quest.itemNumber, GetKey());
        }
        
        if (TutorialManager.instance != null)
            TutorialManager.instance.EndQuest();
        yield return new WaitForSeconds(2);
        if (UIManager.instance.spinRewardPanel != null)
            UIManager.instance.spinRewardPanel.Close();

        GameManager.instance.myPlayer.questId++;
        state = QuestState.Rewarded;
    }

    public void EndCompleteQuest()
    {
        state = QuestState.Ready;
        delayTime = 0;
        replayTime = 0;
        GameManager.instance.myPlayer.questValue = 0;
        StartCoroutine(StartQuest());
    }

    public void CheckQuestComplete()
    {
        int questId = GameManager.instance.myPlayer.questId;

        if (!ItemManager.instance.isLoad)
            return;

        if (questId == 0) {
            if(GameManager.instance.IsEquipItem(41))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 1) {
            if (GameManager.instance.GetAchivement(1) >= 1)
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

                state = QuestState.Complete;
            
        }
        else if (questId == 4)
        {
            if (FindObjectOfType<ItemDirty>() == null)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 5)
        {
            if (GameManager.instance.GetAchivement(4) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 6)
        {

                state = QuestState.Complete;
            
        }
        else if (questId == 7)
        {
            if (UIManager.instance.petRequirementPanel != null)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 8)
        {
            if (GameManager.instance.IsHaveItem(1))
            {
                state = QuestState.Complete;
            }

        }
        else if (questId == 9) { 
            if (GameManager.instance.IsHavePet(1))
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 10)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 11)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
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
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 14)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 15)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 16)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 17)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 18)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 19)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 20)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 21)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
            {
                state = QuestState.Complete;
            }
        }
        else if (questId == 22)
        {
            if (GameManager.instance.GetAchivement(20) >= 1)
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
            else if (state == QuestState.Rewarded)
            {
                EndCompleteQuest();
            }
        }
        else
            time += Time.deltaTime;

        if(state == QuestState.Start)
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

}
