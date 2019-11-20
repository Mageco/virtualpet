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
    public int questID = 0;
    PlayableDirector playTimeLine;
    bool isTimeline = true;
    System.DateTime startTime;
    float time;
    float maxTimeCheck = 0.2f;
    MPopup tipUI;
    float delayTime = 0;
    bool isActive = true;

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
        if(questID == 0){
        }else if(questID == 1){
        }else if(questID == 2){
        }else if(questID == 3){
            delayTime = 16;
        }else if(questID == 4){
        }else if(questID == 5){
        }else if(questID == 6){
        }else if(questID == 7){
        }
    }

    void PlayTip()
    {
       
        StartCoroutine(PlayTimeline());

    }

    IEnumerator PlayTimeline()
    {
        yield return new WaitForSeconds(delayTime);
         OnQuestNotification();

        if(questID == 0){

        }else if(questID == 1){

        }else if(questID == 2){
            GameManager.instance.GetPet(0).sleep = 0;
        }else if(questID == 3){
            GameManager.instance.GetPet(0).food = 10;  
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Food));
        }else if(questID == 4){
            GameManager.instance.GetPet(0).water = 10;        
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Drink));  
        }else if(questID == 5){
            ItemManager.instance.SetExpireSkillTime(1000);
            GameManager.instance.GetPet(0).pee = 100;        
        }else if(questID == 6){
            GameManager.instance.GetPet(0).dirty = 70;        
        }else if(questID == 7){
            GameManager.instance.SetCameraTarget(ItemManager.instance.GetItemChildObject(ItemType.Clean));         
        }

        if (DataHolder.Quest(questID).prefabName != "")
        {
            string url = DataHolder.GetQuest(questID).prefabName.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".prefab", "");
            url = DataHolder.Quests().GetPrefabPath() + url;
            GameObject go = Instantiate((Resources.Load(url) as GameObject), new Vector3(0, 0, -200), Quaternion.identity) as GameObject;
            playTimeLine = go.GetComponent<PlayableDirector>();
        }
        if (playTimeLine != null)
        {
            playTimeLine.gameObject.SetActive(true);
            playTimeLine.Play();
            Debug.Log(playTimeLine.duration);
            yield return new WaitForSeconds((float)playTimeLine.duration);
            playTimeLine.Stop();
            playTimeLine.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(3);
        }

        if (tipUI != null)
            tipUI.Close();

        
        isTimeline = false;
    }

    public void ResetQuest()
    {
        playTimeLine.time = 0;
        PlayTip();
    }

    public void StartCompleteQuest()
    {
        isEndQuest = true;
        QuestPanel questPanel = UIManager.instance.OnQuestCompletePopup();
        questPanel.Load(questID);
        Debug.Log("Quest Complete");
    }

    public void EndCompleteQuest()
    {
        if (DataHolder.Quest(questID).haveItem)
        {
            ApiManager.instance.AddItem(DataHolder.Quest(questID).itemId);
            ApiManager.instance.EquipItem(DataHolder.Quest(questID).itemId);
            ItemManager.instance.LoadItems();
        }

        ApiManager.instance.AddCoin(DataHolder.Quest(questID).coinValue);
        ApiManager.instance.AddDiamond(DataHolder.Quest(questID).diamondValue);
        GameManager.instance.GetPet(0).Exp += DataHolder.Quest(questID).expValue;

        Debug.Log("Exp " + GameManager.instance.GetPet(0).Exp);
           

        if (playTimeLine != null)
            Destroy(playTimeLine.gameObject);

        questID++;
        isTimeline = false;
        isStartQuest = false;
        isEndQuest = false;
        isTimeline = true;
        delayTime = 0;

    }

    public void CheckQuest()
    {

        if (isTimeline)
            return;
        bool isComplete = false;

        if(questID == 0){
            if(GameManager.instance.GetPetObject(0).actionType == ActionType.OnBed){
                isComplete = true;
            }
        }else if(questID == 1){
            if(GameManager.instance.GetPet(0).food > 90){
                isComplete = true;
            }
        }else if(questID == 2){
            if(GameManager.instance.GetPet(0).sleep >= 90){
                isComplete = true;
            }
        }else if(questID == 3){
            if(GameManager.instance.GetPet(0).food > 90){
                GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        } else if(questID == 4){
            if(GameManager.instance.GetPet(0).water > 90){
                GameManager.instance.ResetCameraTarget();
                isComplete = true;
            }
        }else if(questID == 5){
            if(GameManager.instance.GetPet(0).GetSkillProgress(SkillType.Pee) > 0){
                ItemManager.instance.ResetExpireSkillTime();
                isComplete = true;
            }
        }else if(questID == 6){
            if(GameManager.instance.GetPet(0).dirty < 10){
                isComplete = true;
            }
        }else if(questID == 7){
            if(GameObject.FindObjectOfType<ItemDirty>() == null){
                GameManager.instance.ResetCameraTarget();
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
            if(questID >= DataHolder.Quests().GetDataCount()){
                isActive = false;
                return;
            }
                
            StartQuest();
        }
        else
        {
            if (!isEndQuest)
            {
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
        tipUI = UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(DataHolder.Quest(questID).dialogId).GetDescription(0));
    }
}
