using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager instance;
    public GameObject blackScreenUI;
    public GameObject blackScreen;
    public GameObject blackScreenButton;
    public GameObject handClickUI;

    int step = 0;
    int questId = 0;

    void Awake()
	{
		if (instance == null)
			instance = this;
        blackScreenUI.SetActive(false);
        blackScreen.SetActive(false);
        handClickUI.SetActive(false);
        blackScreenButton.SetActive(false);
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
        if (questId == 0)
        {
            if (step == 0)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.shopButton;
                AddSorting(go);
            }
        }
        else if (questId == 1)
        {
            blackScreen.SetActive(true);
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<Animator>();
            anim.Play("Tutorial", 0);
            anim.GetComponent<FoodBowlItem>().isSortingOrder = false;
            Camera.main.GetComponent<CameraController>().SetOrthographic(20);
            Camera.main.GetComponent<CameraController>().screenOffset = 0;
            anim.transform.position += new Vector3(0, 0, -1001);
            blackScreenButton.SetActive(true);
        }
        if (questId == 2)
        {
            if (step == 0)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.eventButton;
                AddSorting(go);
            }
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
                handClickUI.transform.SetParent(blackScreenUI.transform);
                UIManager.instance.confirmBuyShopPopup.Close();
                UIManager.instance.shopPanel.Close();
                blackScreenUI.SetActive(false);
                handClickUI.SetActive(false);
                UIManager.instance.BuyItem(41);
            }
        }else if(questId == 1)
        {

        }else if(questId == 2)
        {
            if(step == 0)
            {
                UIManager.instance.OnMinigame(0);
                step = 1;
            }
        }
    }

    public void EndQuest()
    {
        if(questId == 0)
        {

        }else if(questId == 1)
        {
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<Animator>();
            anim.Play("Idle", 0);
            anim.GetComponent<FoodBowlItem>().isSortingOrder = true;
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }

        step = 0;
        blackScreenUI.SetActive(false);
        blackScreen.SetActive(false);
        handClickUI.SetActive(false);
        blackScreenButton.SetActive(false);
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
