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
                FoodBowlItem item = FindObjectOfType<FoodBowlItem>();
                if(item != null && item.foodAmount < item.maxfoodAmount - 2)
                {
                    blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                    blackScreen.SetActive(true);
                    handClick.SetActive(true);
                    Camera.main.GetComponent<CameraController>().screenOffset = 0;
                    blackScreenButton.SetActive(true);
                    handClick.transform.position = item.transform.position + new Vector3(0, 0, -1000);
                    handClick.GetComponent<Animator>().Play("Click", 0);
                }
            }
        }
        //Collect heart
        else if (questId == 1)
        {
            CollectHappy();
        }
        //Buy water bowl
        else if (questId == 2)
        {
            OnShop(2);
        }
        //Tap to the water bowl
        else if (questId == 3)
        {
            if (step == 0)
            {
                DrinkBowlItem item = FindObjectOfType<DrinkBowlItem>();
                if (item != null && item.foodAmount < item.maxfoodAmount - 2)
                {
                    blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                    blackScreen.SetActive(true);
                    handClick.SetActive(true);
                    Camera.main.GetComponent<CameraController>().screenOffset = 0;
                    blackScreenButton.SetActive(true);
                    handClick.transform.position = item.transform.position + new Vector3(0, 0, -1000);
                    handClick.GetComponent<Animator>().Play("Click", 0);
                }
            }
        }
        //Collect heart
        else if (questId == 4)
        {
            CollectHappy();
        }
        //Buy bath
        else if (questId == 5)
        {
            OnShop(3);
        }
        //Take Bath
        else if (questId == 6)
        {
            if (step == 0)
            {
                StartCoroutine(HoldToBath());
            }
        }
        //Collect Heart
        else if (questId == 7)
        {
            CollectHappy();
        }
        //Take out of bath



        //Buy a broom
        else if (questId == 9)
        {
            OnShop(3);
        }
        //Clean
        else if (questId == 10)
        {
            if (step == 0)
            {
                StartCoroutine(HoldBroom());
            }
        }
        //Buy Toilet
        else if (questId == 11)
        {
            OnShop(3);
        }
        //Go to toilet
        else if (questId == 12)
        {
            if (step == 0)
            {
                StartCoroutine(HoldToToilet());
            }
        }
        //Buy Bed
        else if (questId == 13)
        {
            OnShop(3);
        }
        //Go to sleep
        else if (questId == 14)
        {
            if (step == 0)
            {
                StartCoroutine(HoldToSleep());
            }
        }
        //Talk with Cat
        else if (questId == 15)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if(cat != null)
            {
                GameManager.instance.SetCameraTarget(cat.gameObject);
                Camera.main.GetComponent<CameraController>().screenOffset = 0;
                cat.Active();
            }
        }
        //On Buy equipment
        else if (questId == 16)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
            {
                cat.Active();
            }

        }
        //Collect heart
        else if (questId == 17)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
            {
                cat.Active();
            }
        }
        //Get Cat
        else if (questId == 18)
        {

        }
        //Go Out
        else if (questId == 19)
        {

        }
    }


    void OnShop(int tabID)
    {
        if (step == 0)
        {
            if (UIManager.instance.shopPanel == null)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.shopButton;
                AddSorting(go);
            }
            else
            {
                EventPanel eventPanel = UIManager.instance.OnEventPanel();
                GameObject go = eventPanel.playButton;
                AddSorting(go);
                step = 1;
            }
        }
    }


    void CollectHappy()
    {
        if (step == 0)
        {
            if (FindObjectOfType<HappyItem>() != null)
            {
                blackScreen.SetActive(true);
                HappyItem[] happies = FindObjectsOfType<HappyItem>();
                for (int i = 0; i < happies.Length; i++)
                {
                    happies[i].GetComponent<Animator>().Play("Tutorial", 0);
                    Vector3 pos = happies[i].transform.position;
                    pos.z = -1200 + i;
                    happies[i].transform.position = pos;
                }


                blackScreenButton.SetActive(true);
            }
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
            ClickShopItem(58,2);
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
            ClickShopItem(2,3);
        }
        else if(questId == 8)
        {

        }
        else if (questId == 9)
        {
            ClickShopItem(59,3);
        }
        else if (questId == 11)
        {
            ClickShopItem(11,3);
        }
        else if (questId == 13)
        {
            ClickShopItem(4,3);
        }else if(questId == 17)
        {

        }
        else if (questId == 19)
        {

        }
    }

    void ClickShopItem(int itemId,int tabID)
    {
        
        if (step == 0)
        {
            Debug.Log("OnClick");
            Destroy(UIManager.instance.shopButton.GetComponent<Canvas>());
            ShopPanel shop = UIManager.instance.OnShopPanel();
            GameObject go = shop.toogleAnchor.GetChild(tabID).gameObject;
            AddSorting(go);
            step = 1;
        }
        else if (step == 1)
        {
            Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(tabID).gameObject.GetComponent<Canvas>());
            UIManager.instance.shopPanel.ReLoadTab(tabID);
            UIManager.instance.shopPanel.ScrollToItem(itemId);
            ItemUI item = UIManager.instance.shopPanel.GetItem(itemId);
            AddSorting(item.buyButton.gameObject);
            step = 2;
        }
        else if (step == 2)
        {

            ItemUI item = UIManager.instance.shopPanel.GetItem(itemId);
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
            UIManager.instance.BuyItem(itemId);
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
        }else if(questId == 15)
        {
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }

        step = 0;
        blackScreenUI.SetActive(false);
        blackScreen.SetActive(false);
        handClickUI.SetActive(false);
        handClick.SetActive(false);
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

    protected virtual IEnumerator HoldToBath()
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
            pet.target = Vector3.Lerp(pet.target, dropPosition.position, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
        ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().enabled = true;
        ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().Play("Tutorial", 0);
        yield return new WaitForSeconds(6);
        ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().enabled = false;
        UIManager.instance.OnQuestNotificationPopup("Now try use soap and shower by yourself");
        if (FindObjectOfType<SoapItem>() != null && FindObjectOfType<BathShowerItem>() != null)
        {
            FindObjectOfType<SoapItem>().GetComponent<Animator>().Play("Tutorial", 0);
            FindObjectOfType<BathShowerItem>().GetComponent<Animator>().Play("Tutorial", 0);
        }
    }

    protected virtual IEnumerator HoldToToilet()
    {
        CharController pet = GameManager.instance.GetPetObject(0);
        pet.OnControl();
        handClick.SetActive(true);
        handClick.transform.parent = pet.transform;
        handClick.transform.localPosition = new Vector3(0, 0, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);

        pet.target = pet.transform.position;
        Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Toilet).transform.position + new Vector3(0, 15, 0);
        while (Vector2.Distance(pet.transform.position, holdPosition) > 1)
        {
            pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
    }

    protected virtual IEnumerator HoldToSleep()
    {
        CharController pet = GameManager.instance.GetPetObject(0);
        pet.OnControl();
        handClick.SetActive(true);
        handClick.transform.parent = pet.transform;
        handClick.transform.localPosition = new Vector3(0, 0, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);

        pet.target = pet.transform.position;
        Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Bed).transform.position + new Vector3(0, 14.5f, 0);
        while (Vector2.Distance(pet.transform.position, holdPosition) > 1)
        {
            pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
    }

    protected virtual IEnumerator HoldBroom()
    {
        GameObject broom = ItemManager.instance.GetItemChildObject(ItemType.Clean);
        GameManager.instance.SetCameraTarget(broom);
        handClick.SetActive(true);
        handClick.transform.parent = broom.transform;
        handClick.transform.localPosition = new Vector3(0, 0, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);
        broom.GetComponent<ItemDrag>().isDragable = false;

        ItemDirty dirty = FindObjectOfType<ItemDirty>();

        if (dirty != null)
        {
            Vector3 target = dirty.transform.position + new Vector3(0, 8, 0);
            while (Vector2.Distance(broom.transform.position, target) > 1)
            {
                broom.transform.position = Vector3.Lerp(broom.transform.position, target, Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1);
            broom.GetComponent<ItemDrag>().Return();
            yield return new WaitForSeconds(1);
            //broom.GetComponent<Animator>().Play("Tutorial", 0);
        }
        handClick.SetActive(false);
        GameManager.instance.ResetCameraTarget();
        broom.GetComponent<ItemDrag>().isDragable = true;
    }



}
