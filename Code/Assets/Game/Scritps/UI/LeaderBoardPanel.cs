﻿using System.Collections;
using System.Collections.Generic;
using Mage.Models.Users;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardPanel : MonoBehaviour
{
    public Transform anchor;
    public Transform toogleAnchor;
    public ScrollRect scrollItem;
    List<LeaderUI> items = new List<LeaderUI>();
    List<LeaderBoardItem> list1 = new List<LeaderBoardItem>();
    List<LeaderBoardItem> list2 = new List<LeaderBoardItem>();
    List<LeaderBoardItem> list3 = new List<LeaderBoardItem>();
    List<LeaderBoardItem> list4 = new List<LeaderBoardItem>();
    List<LeaderBoardItem> list5 = new List<LeaderBoardItem>();
    List<LeaderBoardItem> list6 = new List<LeaderBoardItem>();
    List<LeaderBoardItem> list7 = new List<LeaderBoardItem>();
    public GameObject itemUIPrefab;
    public List<Toggle> toggles = new List<Toggle>();
    int currentTab = 0;
    bool isLoad = false;

    // Start is called before the first frame update

    void Awake()
    {
        if (ES2.Exists("LeaderBoardToggle"))
        {
            currentTab = ES2.Load<int>("LeaderBoardToggle");
        }

        for (int i = 0; i < toogleAnchor.childCount; i++)
        {
            int id = i;
            Toggle t = toogleAnchor.GetChild(i).GetComponent<Toggle>();
            toggles.Add(t);
            t.onValueChanged.AddListener(delegate { OnTab(id); });
        }

        if (currentTab > toggles.Count)
            currentTab = 0;

        LoadLocalData();
        LoadServerData();
    }

    public void ReLoad()
    {
        OnTab(currentTab);
    }

    public void ReLoadTab(int id)
    {
        currentTab = id;
        toggles[currentTab].isOn = true;
        Debug.Log(toggles[currentTab]);
        OnTab(currentTab);
    }

    void Start()
    {
        if (toggles[currentTab].isOn)
        {
            OnTab(currentTab);
        }
        else
        {
            toggles[currentTab].isOn = true;
        }
    }

    void LoadLocalData()
    {
        if (ES2.Exists("List1"))
            list1 = ES2.LoadList<LeaderBoardItem>("List1");
        if (ES2.Exists("List2"))
            list2 = ES2.LoadList<LeaderBoardItem>("List2");
        if (ES2.Exists("List3"))
            list3 = ES2.LoadList<LeaderBoardItem>("List3");
        if (ES2.Exists("List4"))
            list4 = ES2.LoadList<LeaderBoardItem>("List4");
        if (ES2.Exists("List5"))
            list5 = ES2.LoadList<LeaderBoardItem>("List5");
        if (ES2.Exists("List6"))
            list5 = ES2.LoadList<LeaderBoardItem>("List6");
        if (ES2.Exists("List7"))
            list5 = ES2.LoadList<LeaderBoardItem>("List7");
        OnTab(currentTab);
    }


    void LoadServerData()
    {
        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "collectedHappy",
        (leaderboardItems) => {
            if (UIManager.instance.leaderBoardPanel != null)
            {
                list1.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list1.Add(i);
                }
                ES2.Save(list1, "List1");
                if (currentTab == 0)
                    OnTab(currentTab);
            }
        });

        
        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            if(UIManager.instance.leaderBoardPanel != null)
            {
                list2.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list2.Add(i);
                }
                ES2.Save(list2, "List2");
                if (currentTab == 1)
                    OnTab(currentTab);
            }

        },
        0);

        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            if (UIManager.instance.leaderBoardPanel != null)
            {
                list3.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list3.Add(i);
                }
                ES2.Save(list3, "List3");
                if (currentTab == 2)
                    OnTab(currentTab);
            }
        },
        1);

        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            if (UIManager.instance.leaderBoardPanel != null)
            {
                list4.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list4.Add(i);
                }
                ES2.Save(list4, "List4");
                if (currentTab == 3)
                    OnTab(currentTab);
            }
        },
        2);

        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            if (UIManager.instance.leaderBoardPanel != null)
            {
                list5.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list5.Add(i);
                }
                ES2.Save(list5, "List5");
                if (currentTab == 4)
                    OnTab(currentTab);
            }
        },
        3);

        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            if (UIManager.instance.leaderBoardPanel != null)
            {
                list6.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list6.Add(i);
                }
                ES2.Save(list5, "List6");
                if (currentTab == 5)
                    OnTab(currentTab);
            }
        },
        4);

        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            if (UIManager.instance.leaderBoardPanel != null)
            {
                list7.Clear();
                foreach (LeaderBoardItem i in leaderboardItems)
                {
                    list7.Add(i);
                }
                ES2.Save(list7, "List7");
                if (currentTab == 6)
                    OnTab(currentTab);
            }
        },
        5);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTab(int id)
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        currentTab = id;

        ES2.Save(id, "LeaderBoardToggle");
        ClearItems();

        if (id == 0)
        {
            foreach (LeaderBoardItem item in list1)
            {
                LoadItem(item, id);
            }
        }
        else if (id == 1)
        {
            foreach (LeaderBoardItem item in list2)
            {
                LoadItem(item, id);
            }
        }
        else if (id == 2)
        {
            foreach (LeaderBoardItem item in list3)
            {
                LoadItem(item, id);
            }
        }
        else if (id == 3)
        {
            foreach (LeaderBoardItem item in list4)
            {
                LoadItem(item, id);
            }
        }
        else if (id == 4)
        {
            foreach (LeaderBoardItem item in list5)
            {
                LoadItem(item, id);
            }
        }
        else if (id == 5)
        {
            foreach (LeaderBoardItem item in list6)
            {
                LoadItem(item, id);
            }
        }
        else if (id == 6)
        {
            foreach (LeaderBoardItem item in list7)
            {
                LoadItem(item, id);
            }
        }
    }

    IEnumerator LoadItems(int id)
    {
        while (!isLoad)
        {
            yield return new WaitForEndOfFrame();
        }

        
    }

    public void ScrollToItem(int id)
    {
        StartCoroutine(ScrollToItemCoroutine(id));
    }

    IEnumerator ScrollToItemCoroutine(int id)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (LeaderUI item in items)
        {
            if (item.iD == id)
            {
                float normalizePosition = (float)item.transform.GetSiblingIndex() / (float)scrollItem.content.transform.childCount;
                Debug.Log(normalizePosition);
                scrollItem.verticalNormalizedPosition = 1 - normalizePosition;

            }
        }

    }

    void LoadItem(LeaderBoardItem data,int tabId)
    {
        GameObject go = Instantiate(itemUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        LeaderUI item = go.GetComponent<LeaderUI>();
        item.Load(data, tabId);
        items.Add(item);
    }

    void ClearItems()
    {
        foreach (LeaderUI item in items)
        {
            if(item != null)
            {
                item.OnClose();
                Destroy(item.gameObject);
            }
                
        }
        items.Clear();
    }

    public void Close()
    {
        ClearItems();
        Resources.UnloadUnusedAssets();
        this.GetComponent<Popup>().Close();
    }
}
