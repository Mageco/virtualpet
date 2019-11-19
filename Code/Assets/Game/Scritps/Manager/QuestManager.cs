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
        if (DataHolder.Quest(questID).prefabName != "")
        {
            string url = DataHolder.GetQuest(questID).prefabName.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".prefab", "");
            url = DataHolder.Quests().GetPrefabPath() + url;
            GameObject go = Instantiate((Resources.Load(url) as GameObject), new Vector3(0, 0, -200), Quaternion.identity) as GameObject;
            playTimeLine = go.GetComponent<PlayableDirector>();
        }
    }

    void PlayTip()
    {
        OnQuestNotification();
        StartCoroutine(PlayTimeline());

    }

    IEnumerator PlayTimeline()
    {
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
        GameManager.instance.pets[0].Exp += DataHolder.Quest(questID).expValue;

        Debug.Log("Exp " + GameManager.instance.pets[0].Exp);
           

        if (playTimeLine != null)
            Destroy(playTimeLine.gameObject);

        questID++;
        isTimeline = false;
        isStartQuest = false;
        isEndQuest = false;
        isTimeline = true;

    }

    public void CheckQuest()
    {

        if (isTimeline)
            return;

        int count = 0;
        int check = DataHolder.Quest(questID).requirements.Length;

        for (int i = 0; i < DataHolder.Quest(questID).requirements.Length; i++)
        {
            if (DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Action)
            {
                List<ActionData> actions = ApiManager.instance.GetActionLogs(startTime);
                if (actions != null)
                {
                    foreach (ActionData a in actions)
                    {
                        if (a.actionType == DataHolder.Quest(questID).requirements[i].actionType)
                        {
                            count++;
                        }
                    }
                }
            }
            else if (DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Skill)
            {

            }
            else if (DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Interact)
            {

            }
            else if (DataHolder.Quest(questID).requirements[i].requireType == QuestRequirementType.Variable)
            {

            }
        }
        if (count >= check)
        {
            StartCompleteQuest();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isStartQuest)
        {
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
