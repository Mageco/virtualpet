using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager instance;
    public GameObject blackScreenUI;
    public GameObject blackScreen;
    public GameObject handClickUI;
    public GameObject handClick;
    int step = 0;
    int questId = 0;

    void Awake()
	{
		if (instance == null)
			instance = this;
        blackScreenUI.SetActive(false);
        blackScreen.SetActive(false);
        handClickUI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartQuest()
    {
        step = 0;
        questId = GameManager.instance.myPlayer.questId;
        CheckQuest();
    }

    public void CheckQuest()
	{
        if (questId == 0)
        {
            if (step == 0)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.shopButton;
                AddSorting(go);
            }
        }else if(questId == 1)
        {
            blackScreen.SetActive(true);
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<Animator>();
            anim.Play("Tutorial", 0);
            anim.transform.parent.transform.position += new Vector3(0, 0, -1001);
        }
    }


    public void OnClick()
    {
        Debug.Log("OnClick");
        if (questId == 0)
        {
            if (step == 0)
            {
                Debug.Log("OnClick");
                Destroy(UIManager.instance.shopButton.GetComponent<Canvas>());
                ShopPanel shop = UIManager.instance.OnShopPanel();
                GameObject go = shop.toogleAnchor.GetChild(2).gameObject;
                AddSorting(go);
                step = 1;
            }
            else if(step == 1)
            {
                Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(2).gameObject.GetComponent<Canvas>());
                UIManager.instance.shopPanel.OnTab(2);
                ItemUI item = UIManager.instance.shopPanel.GetItem(41);
                AddSorting(item.buyButton.gameObject);
                step = 2;
            }
            else if(step == 2)
            {
                ItemUI item = UIManager.instance.shopPanel.GetItem(41);
                Destroy(item.buyButton.gameObject.GetComponent<Canvas>());
                item.OnBuy();
                AddSorting(UIManager.instance.confirmBuyShopPopup.okButton.gameObject);
                step = 3;
            }
            else if (step == 3)
            {
                UIManager.instance.confirmBuyShopPopup.Close();
                UIManager.instance.shopPanel.Close();
                blackScreenUI.SetActive(false);
                handClickUI.SetActive(false);
                UIManager.instance.BuyItem(41);
            }
        }else if(questId == 1)
        {

        }
    }

    public void EndQuest()
    {
        if(questId == 0)
        {

        }else if(questId == 1)
        {

        }

        step = 0;
        blackScreenUI.SetActive(false);
        blackScreen.SetActive(false);
        handClickUI.SetActive(false);
    }

    void AddSorting(GameObject go)
    {
        go.AddComponent<Canvas>();
        go.GetComponent<Canvas>().overrideSorting = true;
        go.GetComponent<Canvas>().sortingOrder = 2;
        handClickUI.transform.SetParent(go.transform);
        handClickUI.transform.localScale = Vector3.one;
        handClickUI.transform.localPosition = Vector3.zero;
        handClickUI.SetActive(true);
    }




}
