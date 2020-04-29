using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryPanel : MonoBehaviour
{
    public ScrollRect scroll;
    public ScrollRect scrollItem;
    public Transform anchor;
    List<AccessoryUI> items = new List<AccessoryUI>();
    public GameObject itemUIPrefab;
    int currentTab = 0;
    [HideInInspector]
    public List<Toggle> toggles = new List<Toggle>();
    public Transform toogleAnchor;
    List<int> petIds = new List<int>();
    public GameObject tooglePrefab;

    // Start is called before the first frame update

    void Awake()
    {
        if (ES2.Exists("AccessoryToggle"))
        {
            currentTab = ES2.Load<int>("AccessoryToggle");
        }



        foreach(PlayerPet pet in GameManager.instance.GetPets())
        {
            petIds.Add(pet.iD);
        }

        for (int i = 0; i < petIds.Count; i++)
        {
            GameObject go = GameObject.Instantiate(tooglePrefab) as GameObject;
            go.transform.parent = toogleAnchor;
            go.transform.localScale = Vector3.one;
            int id = i;
            Toggle t = go.GetComponent<Toggle>();
            toggles.Add(t);
            t.group = toogleAnchor.GetComponent<ToggleGroup>();
            Pet d = DataHolder.GetPet(petIds[i]);
            string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            t.targetGraphic.GetComponent<Image>().sprite = Resources.Load<Sprite>(url) as Sprite;
            t.graphic.GetComponent<Image>().sprite = Resources.Load<Sprite>(url) as Sprite;
            t.onValueChanged.AddListener(delegate { OnTab(id); });
        }

        if (currentTab > toggles.Count)
            currentTab = 0;

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

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTab(int id)
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        currentTab = id;

        ES2.Save(id, "AccessoryToggle");
        Load(petIds[id]);
    }

    public void Load(int petId)
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        ClearItems();

        List<Accessory> items = DataHolder.GetAccessories(petId);
        items.Sort((p1, p2) => (p1.levelRequire).CompareTo(p2.levelRequire));
        Debug.Log("Accessories Number " + items.Count);
        foreach (Accessory a in items)
        {
            LoadItem(a);
        }
    }

    public void ScrollToItem(int id)
    {
        StartCoroutine(ScrollToItemCoroutine(id));
    }

    IEnumerator ScrollToItemCoroutine(int id)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (AccessoryUI item in items)
        {
            if (item.itemId == id)
            {
                float normalizePosition = (float)item.transform.GetSiblingIndex() / (float)scrollItem.content.transform.childCount;
                Debug.Log(normalizePosition);
                scrollItem.verticalNormalizedPosition = 1 - normalizePosition;

            }
        }
    }

    public AccessoryUI GetItem(int id)
    {
        foreach (AccessoryUI item in items)
        {
            if (item.itemId == id)
                return item;
        }
        return null;
    }

    void LoadItem(Accessory data)
    {
        if (!data.isAvailable)
            return;
        GameObject go = Instantiate(itemUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        AccessoryUI item = go.GetComponent<AccessoryUI>();
        items.Add(item);
        item.Load(data);
    }

    void ClearItems()
    {
        foreach (AccessoryUI item in items)
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

