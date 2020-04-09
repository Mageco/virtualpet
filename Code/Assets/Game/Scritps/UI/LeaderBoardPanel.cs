using System.Collections;
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
    public GameObject itemUIPrefab;
    public List<Toggle> toggles = new List<Toggle>();
    int currentTab = 0;

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




    // Update is called once per frame
    void Update()
    {

    }

    public void OnTab(int id)
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        currentTab = id;

        ES2.Save(id, "ShopToggle");
        ClearItems();

        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "diamond",
        (leaderboardItems) => {
            foreach (LeaderBoardItem i in leaderboardItems)
            {
                Debug.Log(i.ToJson());
                LoadItem(i, id);
            }
        });

        /*
        MageEngine.instance.GetLeaderBoardFromObject(
        GameManager.instance.myPlayer,
        "minigameLevels",
        (leaderboardItems) => {
            foreach (LeaderBoardItem i in leaderboardItems)
            {
                Debug.Log(i.ToJson());
                LoadItem(i,id);
            }
        },
        0);*/
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
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
