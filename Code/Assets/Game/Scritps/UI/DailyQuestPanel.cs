using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestPanel : MonoBehaviour
{
    public Transform anchor;
    List<DailyQuestUI> items = new List<DailyQuestUI>();
    public GameObject dailyQuestUIPrefab;
    public GameObject colectActive;
    public GameObject collectDeActive;
    public Text completedText;

    // Start is called before the first frame update
    void Awake()
    {

    }

    public void Load()
    {
        LoadQuest();
        LoadUI();
    }

    void LoadQuest()
    {


        List<Achivement> achivements = new List<Achivement>();
        for(int i = 0; i < DataHolder.Achivements().GetDataCount(); i++)
        {
            if (DataHolder.Achivement(i).isAvailable)
            {
                achivements.Add(DataHolder.Achivement(i));
            }
        }

        List<int> ids = new List<int>();
        while (ids.Count < 3)
        {
            int id = Random.Range(0, achivements.Count);
            while (ids.Contains(id))
            {
                id = Random.Range(0, achivements.Count);
            }
            ids.Add(id);
        }

        if (GameManager.instance.myPlayer.dailyQuests.Count < 3)
        {
            for(int i= GameManager.instance.myPlayer.dailyQuests.Count; i < 3; i++)
            {
                DailyQuestData quest = new DailyQuestData();
                quest.achivementId = achivements[ids[i]].iD;
                GameManager.instance.myPlayer.dailyQuests.Add(quest);
                GameManager.instance.myPlayer.isCompleteDailyQuest = false;
            }
        }

        int count = 0;
        foreach(DailyQuestData quest in GameManager.instance.myPlayer.dailyQuests)
        {
            if (quest.timeCollected != "")
            {
                if (System.DateTime.Parse(quest.timeCollected).Year < MageEngine.instance.GetServerTimeStamp().Year || System.DateTime.Parse(quest.timeCollected).Month < MageEngine.instance.GetServerTimeStamp().Month || System.DateTime.Parse(quest.timeCollected).Day < MageEngine.instance.GetServerTimeStamp().Day)
                {
                    quest.achivementId = achivements[ids[count]].iD;
                    quest.state = DailyQuestState.None;
                    quest.timeCollected = MageEngine.instance.GetServerTimeStamp().ToString();
                    GameManager.instance.myPlayer.isCompleteDailyQuest = false;
                    count++;
                }
            }
        }

        foreach (DailyQuestData quest in GameManager.instance.myPlayer.dailyQuests)
        {
            if (quest.state == DailyQuestState.None)
            {
                quest.startValue = GameManager.instance.GetAchivement(quest.achivementId);
                int petNumber = GameManager.instance.GetPets().Count;
                quest.bonus = petNumber * 10;
                quest.state = DailyQuestState.Started;
                if (quest.achivementId == 3)
                    quest.requireValue = 3;
                else if(quest.achivementId == 7)
                    quest.requireValue = 3;
                else if (quest.achivementId == 8)
                    quest.requireValue = 1;
                else if (quest.achivementId == 21)
                    quest.requireValue = 1;
            }
        }

        foreach (DailyQuestData quest in GameManager.instance.myPlayer.dailyQuests)
        {
            if (quest.state == DailyQuestState.Started)
            {
                quest.value = GameManager.instance.GetAchivement(quest.achivementId) - quest.startValue;
                if(quest.value >= quest.requireValue)
                    quest.state = DailyQuestState.Ready;
            }
        }
    }


    public void LoadUI()
    {
        ClearItems();

        int count = 0;
        foreach (DailyQuestData quest in GameManager.instance.myPlayer.dailyQuests)
        {
            LoadItem(quest);
            if (quest.state == DailyQuestState.Collected)
            {
                count ++;
            }
        }

        if (count == 3)
        {
            collectDeActive.SetActive(false);
            if (!GameManager.instance.myPlayer.isCompleteDailyQuest)
            {
                colectActive.GetComponent<Button>().interactable = true;
                colectActive.SetActive(true);
            }
            else
            {
                colectActive.SetActive(true);
                colectActive.GetComponent<Button>().interactable = false;
            }
                

        }
        else
        {
            completedText.text = count.ToString() + "/3";
            collectDeActive.SetActive(true);
            colectActive.SetActive(false);
        }
    }


    void LoadItem(DailyQuestData data)
    {
        GameObject go = Instantiate(dailyQuestUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        DailyQuestUI item = go.GetComponent<DailyQuestUI>();
        items.Add(item);
        item.Load(data);
    }

    public void CollectAll()
    {
#if !UNITY_EDITOR
        if (!ApiManager.instance.IsLogin())
        {
            MageManager.instance.OnNotificationPopup("Network error");
            return;
        }
#endif

        GameManager.instance.myPlayer.isCompleteDailyQuest = true;
        GameManager.instance.AddDiamond(10, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        MageManager.instance.PlaySound("Collect_Achivement", false);
        LoadUI();
    }

    void ClearItems()
    {
        foreach (DailyQuestUI s in items)
        {
            Destroy(s.gameObject);
        }
        items.Clear();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
