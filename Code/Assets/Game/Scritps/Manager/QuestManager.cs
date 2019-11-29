﻿using System.Collections;
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
    float maxReplayTime = 15;

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
            delayTime = 16;
        }else if(GameManager.instance.questId == 4){
        }else if(GameManager.instance.questId == 5){
        }else if(GameManager.instance.questId == 6){
        }else if(GameManager.instance.questId == 7){
            GameManager.instance.GetPet(0).pee = 100; 
            delayTime = 3;
        }

        //isReplay = DataHolder.GetQuest(GameManager.instance.questId).isReplay;
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
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
        }else if(GameManager.instance.questId == 1){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Bed));
            GameManager.instance.GetPet(0).sleep = 40;
            yield return new WaitForSeconds(2);
            GameManager.instance.SetCameraTarget(GameManager.instance.GetPetObject(0).gameObject);
        }else if(GameManager.instance.questId == 2){
            
        }else if(GameManager.instance.questId == 3){
            GameManager.instance.GetPet(0).food = 10;  
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 4){
            GameManager.instance.GetPet(0).water = 10;        
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Drink));  
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();
        }else if(GameManager.instance.questId == 5){
            GameManager.instance.GetPet(0).dirty = 70;  
        }else if(GameManager.instance.questId == 6){
        }else if(GameManager.instance.questId == 7){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Clean));   
            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetCameraTarget();  
        }



        // if (playTimeLine == null && DataHolder.Quest(GameManager.instance.questId).prefabName != "")
        // {
        //     MageManager.instance.ScreenFadeOut(fadeDuration);
        //     yield return new WaitForSeconds(fadeDuration);
        //     MageManager.instance.ScreenFadeIn(fadeDuration);
        //     OnQuestNotification();
        //     string url = DataHolder.GetQuest(GameManager.instance.questId).prefabName.Replace("Assets/Game/Resources/", "");
        //     url = url.Replace(".prefab", "");
        //     url = DataHolder.Quests().GetPrefabPath() + url;
        //     GameObject go = Instantiate((Resources.Load(url) as GameObject), new Vector3(0, 0, -200), Quaternion.identity) as GameObject;
        //     playTimeLine = go.GetComponent<PlayableDirector>();
        // }
        // else
        // {
            
        //}
        if (playTimeLine != null)
        {
            // playTimeLine.gameObject.SetActive(true);
            // playTimeLine.Play();
            // Debug.Log(playTimeLine.duration);
            // yield return new WaitForSeconds((float)playTimeLine.duration - fadeDuration);
            // MageManager.instance.ScreenFadeOut(fadeDuration);
            // yield return new WaitForSeconds(fadeDuration);
            // MageManager.instance.ScreenFadeIn(fadeDuration);
            // playTimeLine.Stop();
            // playTimeLine.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(3);
        }

        if (tipUI != null)
            tipUI.Close();

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
           ApiManager.GetInstance().AddItem(DataHolder.Quest(GameManager.instance.questId).itemId);
           ApiManager.GetInstance().EquipItem(DataHolder.Quest(GameManager.instance.questId).itemId);
            ItemManager.instance.LoadItems();
        }

       ApiManager.GetInstance().AddCoin(DataHolder.Quest(GameManager.instance.questId).coinValue);
       ApiManager.GetInstance().AddDiamond(DataHolder.Quest(GameManager.instance.questId).diamondValue);
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
            if(GameManager.instance.GetPet(0).sleep >= 90){
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 2){
            if(GameManager.instance.GetPet(0).level >= 2){
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 3){
            if(ItemManager.instance.GetItem(ItemType.Food) != null && ItemManager.instance.GetItem(ItemType.Food).GetComponentInChildren<FoodBowlItem>().foodAmount > 90){
                GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        } else if(GameManager.instance.questId == 4){
            if(ItemManager.instance.GetItem(ItemType.Drink) != null && ItemManager.instance.GetItem(ItemType.Drink).GetComponentInChildren<DrinkBowlItem>().foodAmount > 90){
                GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        }else if(GameManager.instance.questId == 5){
            if(GameManager.instance.GetPet(0).Dirty <= 50){
                GameManager.instance.ResetCameraTarget();
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
