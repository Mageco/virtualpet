using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryPanel : MonoBehaviour
{
    public ScrollRect scrollItem;
    public Transform anchor;
    List<AccessoryUI> items = new List<AccessoryUI>();
    public GameObject itemUIPrefab;


    // Start is called before the first frame update

    void Awake()
    {
        

    }
    void Start()
    {

    }
 

    // Update is called once per frame
    void Update()
    {

    }


    public void Load(int realId)
    {
        UIManager.instance.accessoryCamera.SetActive(true);
        UIManager.instance.homeUI.SetActive(false);
        if (UIManager.instance.profilePanel != null)
            UIManager.instance.profilePanel.Close();

        MageManager.instance.PlaySound("BubbleButton", false);
        ClearItems();
        PlayerPet pet = GameManager.instance.GetPet(realId);
        Debug.Log(pet.iD);
        List<Accessory> temp = DataHolder.GetAccessories(pet.iD);
        temp.Sort((p1, p2) => (p1.levelRequire).CompareTo(p2.levelRequire));
        Debug.Log("Accessories Number " + items.Count);
        for(int i=0;i< temp.Count;i++)
        {
            LoadItem(temp[i], pet);
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

    void LoadItem(Accessory data,PlayerPet pet)
    {
        if (!data.isAvailable)
            return;
        GameObject go = Instantiate(itemUIPrefab);
        go.transform.SetParent(this.anchor);
        go.transform.localScale = Vector3.one;
        AccessoryUI item = go.GetComponent<AccessoryUI>();
        items.Add(item);
        item.Load(data, pet);
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
        UIManager.instance.accessoryCamera.SetActive(false);
        UIManager.instance.homeUI.SetActive(true);
        UIManager.instance.OnProfilePanel();
        Destroy(this.gameObject);
    }

}

