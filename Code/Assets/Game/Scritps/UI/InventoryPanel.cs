using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public Transform anchor;
    List<InventoryUI> items = new List<InventoryUI>();
    public GameObject InventoryUIPrefab;
    int currentCat = 0;
    [HideInInspector]
    public List<Toggle> catToggles = new List<Toggle>();
    List<ItemType> catType = new List<ItemType>();
    public Transform catToogleAnchor;
    public GameObject catTogglePrefab;
    // Start is called before the first frame update

    void Awake()
    {
        foreach (PlayerItem item in GameManager.instance.myPlayer.items)
        {
            if (item.state != ItemState.OnShop && !catType.Contains(item.itemType))
            {
                catType.Add(item.itemType);
            }
        }
    }

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }



    public void Load()
    {
        MageManager.instance.PlaySound("BubbleButton", false);
        ClearItems();

        foreach (ItemType t in catType)
        {
            bool isExisted = false;
            foreach (Toggle to in catToggles)
            {
                if (to.gameObject.name == t.ToString())
                {
                    isExisted = true;
                    break;
                }
            }

            if (!isExisted)
            {
                GameObject go = GameObject.Instantiate(catTogglePrefab);
                go.transform.SetParent(catToogleAnchor);
                go.transform.localScale = Vector3.one;
                go.name = t.ToString();
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.group = catToogleAnchor.GetComponent<ToggleGroup>();
                toggle.targetGraphic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/ItemType/" + t.ToString());
                toggle.graphic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/ItemType/" + t.ToString());
                catToggles.Add(toggle);
                int n = (int)t;
                toggle.onValueChanged.AddListener(delegate { OnType(n); });
            }
        }

        List<PlayerItem> temp = new List<PlayerItem>();

        foreach(PlayerItem item in GameManager.instance.myPlayer.items)
        {
            if(item.state != ItemState.OnShop)
            {
                if (currentCat == 0 || (int)item.itemType == currentCat)
                {
                    temp.Add(item);
                }
            }
        }

        temp.Sort((p1, p2) => ((int)p1.itemType).CompareTo((int)p2.itemType));

        foreach (PlayerItem item in temp)
        {
            LoadItem(item);
        }
    }

    public void OnType(int id)
    {
        currentCat = id;
        Load();
    }

    public InventoryUI GetItem(int id)
    {
        foreach (InventoryUI item in items)
        {
            if (item.realId == id)
                return item;
        }
        return null;
    }

    void LoadItem(PlayerItem data)
    {
        GameObject go = Instantiate(InventoryUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        InventoryUI item = go.GetComponent<InventoryUI>();
        items.Add(item);
        item.Load(data);
    }

    void ClearCat()
    {
        catType.Clear();
        foreach (Toggle t in catToggles)
        {
            GameObject.Destroy(t.gameObject);
        }
        catToggles.Clear();
    }

    void ClearItems()
    {
        foreach (InventoryUI item in items)
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
