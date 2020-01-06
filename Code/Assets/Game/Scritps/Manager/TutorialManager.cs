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
    public GameObject handClick;
    public Transform dropPosition;

    int step = 0;
    int questId = 0;

    void Awake()
	{
		if (instance == null)
			instance = this;
        blackScreenUI.SetActive(false);
        blackScreen.SetActive(false);
        handClickUI.SetActive(false);
        handClick.SetActive(false);
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
        //Tap to the food bowl
        if (questId == 0)
        {
            if (step == 0)
            {
                blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                blackScreen.SetActive(true);
                handClick.SetActive(true);
                Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<Animator>();
                anim.GetComponent<FoodBowlItem>().foodAmount = 0;
                Camera.main.GetComponent<CameraController>().screenOffset = 0;
                blackScreenButton.SetActive(true);
                handClick.transform.position = anim.transform.position + new Vector3(0, 0, -1000);
                handClick.GetComponent<Animator>().Play("Click", 0);

            }
        }
        //Collect heart
        else if (questId == 1)
        {
            if (step == 0)
            {
                if(FindObjectOfType<HappyItem>() != null)
                {
                    blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                    blackScreen.SetActive(true);
                    Animator anim = FindObjectOfType<HappyItem>().gameObject.GetComponent<Animator>();
                    anim.Play("Tutorial", 0);
                    Vector3 pos = anim.transform.position;
                    pos.z = -1200;
                    anim.transform.position = pos;
                    blackScreenButton.SetActive(true);

                }
            }
        }
        //Buy water bowl
        else if (questId == 2)
        {
            blackScreenUI.SetActive(true);
            GameObject go = UIManager.instance.shopButton;
            AddSorting(go);
        }
        //Tap to the water bowl
        else if (questId == 3)
        {
            if (step == 0)
            {
                blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                blackScreen.SetActive(true);
                handClick.SetActive(true);
                Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Drink).GetComponent<Animator>();
                anim.GetComponent<DrinkBowlItem>().foodAmount = 0;
                Camera.main.GetComponent<CameraController>().screenOffset = 0;
                blackScreenButton.SetActive(true);
                handClick.transform.position = anim.transform.position + new Vector3(0, 0, -1000);
                handClick.GetComponent<Animator>().Play("Click", 0);
            }
        }
        //Collect heart
        else if (questId == 4)
        {
            if (step == 0)
            {
                if (FindObjectOfType<HappyItem>() != null)
                {
                    blackScreen.SetActive(true);
                    Animator anim = FindObjectOfType<HappyItem>().gameObject.GetComponent<Animator>();
                    anim.Play("Tutorial", 0);
                    Vector3 pos = anim.transform.position;
                    pos.z = -1200;
                    anim.transform.position = pos;
                    blackScreenButton.SetActive(true);
                }
            }
        }
        //Buy bath
        else if (questId == 5)
        {
            if (step == 0)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.shopButton;
                AddSorting(go);
            }
        }
        //Take Bath
        else if (questId == 6)
        {
            if (step == 0)
            {
                StartCoroutine(Hold());
            }
        }
        //Take out of bath

        //Buy a broom
        else if (questId == 8)
        {
            if (step == 0)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.shopButton;
                AddSorting(go);
            }
        }
    }


    protected virtual IEnumerator Hold()
    {
        CharController pet = GameManager.instance.GetPetObject(0);
        pet.OnControl();
        handClick.SetActive(true);
        handClick.transform.parent = pet.transform;
        handClick.transform.localPosition = new Vector3(0, 0, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);

        pet.target = pet.transform.position;
        while (Vector2.Distance(pet.transform.position, dropPosition.position) > 1)
        {
            pet.target = Vector3.Lerp(pet.target,dropPosition.position,Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
        ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().enabled = true;
        ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().Play("Tutorial", 0);
        yield return new WaitForSeconds(6);
        ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().enabled = false;
        UIManager.instance.OnQuestNotificationPopup("Now try use soap and shower by yourself");
        if(FindObjectOfType<SoapItem>() != null && FindObjectOfType<BathShowerItem>() != null)
        {
            FindObjectOfType<SoapItem>().GetComponent<Animator>().Play("Tutorial", 0);
            FindObjectOfType<BathShowerItem>().GetComponent<Animator>().Play("Tutorial", 0);
        }
    }


    public void OnClick()
    {
        Debug.Log("OnClick");
        if (questId == 0)
        {
            handClick.SetActive(false);
            FindObjectOfType<FoodBowlItem>().Fill();
        }
        else if (questId == 1)
        {

        }
        else if (questId == 2)
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
            else if (step == 1)
            {
                Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(2).gameObject.GetComponent<Canvas>());
                UIManager.instance.shopPanel.ReLoadTab(2);
                ItemUI item = UIManager.instance.shopPanel.GetItem(58);
                AddSorting(item.buyButton.gameObject);
                step = 2;
            }
            else if (step == 2)
            {
                ItemUI item = UIManager.instance.shopPanel.GetItem(58);
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
                UIManager.instance.BuyItem(58);
            }
        }
        else if (questId == 3)
        {
            handClick.SetActive(false);
            FindObjectOfType<DrinkBowlItem>().Fill();

        }
        else if (questId == 4)
        {

        }
        else if (questId == 5)
        {
            if (step == 0)
            {
                Debug.Log("OnClick");
                Destroy(UIManager.instance.shopButton.GetComponent<Canvas>());
                ShopPanel shop = UIManager.instance.OnShopPanel();
                GameObject go = shop.toogleAnchor.GetChild(3).gameObject;
                AddSorting(go);
                step = 1;
            }
            else if (step == 1)
            {
                Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(3).gameObject.GetComponent<Canvas>());
                UIManager.instance.shopPanel.ReLoadTab(3);
                UIManager.instance.shopPanel.ScrollToItem(2);
                ItemUI item = UIManager.instance.shopPanel.GetItem(2);
                AddSorting(item.buyButton.gameObject);
                step = 2;
            }
            else if (step == 2)
            {
                
                ItemUI item = UIManager.instance.shopPanel.GetItem(2);
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
                UIManager.instance.BuyItem(2);
            }
        }
        else if (questId == 8)
        {
            if (step == 0)
            {
                Debug.Log("OnClick");
                Destroy(UIManager.instance.shopButton.GetComponent<Canvas>());
                ShopPanel shop = UIManager.instance.OnShopPanel();
                GameObject go = shop.toogleAnchor.GetChild(3).gameObject;
                AddSorting(go);
                step = 1;
            }
            else if (step == 1)
            {
                Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(3).gameObject.GetComponent<Canvas>());
                UIManager.instance.shopPanel.ReLoadTab(3);
                UIManager.instance.shopPanel.ScrollToItem(59);
                ItemUI item = UIManager.instance.shopPanel.GetItem(59);
                AddSorting(item.buyButton.gameObject);
                step = 2;
            }
            else if (step == 2)
            {

                ItemUI item = UIManager.instance.shopPanel.GetItem(59);
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
                UIManager.instance.BuyItem(59);
            }
        }
    }

    public void EndQuest()
    {
        if(questId == 0)
        {
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Food).GetComponent<Animator>();
            anim.Play("Idle", 0);
            anim.GetComponent<FoodBowlItem>().isSortingOrder = true;
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }
        else if(questId == 3)
        {
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Drink).GetComponent<Animator>();
            anim.Play("Idle", 0);
            anim.GetComponent<DrinkBowlItem>().isSortingOrder = true;
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }else if(questId == 6)
        {
            if (FindObjectOfType<SoapItem>() != null && FindObjectOfType<BathShowerItem>() != null)
            {
                FindObjectOfType<SoapItem>().GetComponent<Animator>().Play("Idle", 0);
                FindObjectOfType<BathShowerItem>().GetComponent<Animator>().Play("Idle", 0);
            }
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
