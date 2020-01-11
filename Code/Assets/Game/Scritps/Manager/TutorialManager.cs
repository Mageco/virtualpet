using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager instance;
    public GameObject blackScreenUI;
    public GameObject blackScreen;
    public GameObject blackScreenButton;
    public GameObject handClickUI;
    public GameObject handClick;
    public Transform dropPosition;
    public Transform pointer;

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
            StartCoroutine(Quest0());

        }
        //Collect heart
        else if (questId == 1)
        {
            CollectHappy();
        }
        //Buy water bowl
        else if (questId == 2)
        {
            OnShop(2,58);
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
            OnShop(3,2);
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
            OnShop(3,59);
        }
        //Clean
        else if (questId == 10)
        {
            if (step == 0)
            {
                if(FindObjectOfType<ItemDirty>() != null)
                    StartCoroutine(HoldBroom());
            }
        }
        //Buy Toilet
        else if (questId == 11)
        {
            OnShop(3,11);
        }
        //Go to toilet
        else if (questId == 12)
        {
            if (step == 0)
            {
                if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Toilet)
                    StartCoroutine(HoldToToilet());
            }
        }
        //Buy Bed
        else if (questId == 13)
        {
            OnShop(3,4);
        }
        //Go to sleep
        else if (questId == 14)
        {
            if (step == 0)
            {
                if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Bed)
                    StartCoroutine(HoldToSleep());
            }
        }
        //Talk with Cat
        else if (questId == 15)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if(cat != null)
            {
                ItemManager.instance.SetCameraTarget(cat.gameObject);
                Camera.main.GetComponent<CameraController>().screenOffset = 0;
                cat.Load();
            }
        }
        //On Buy equipment
        else if (questId == 16)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
            {
                cat.Load();
            }

        }
        //Collect heart
        else if (questId == 17)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
            {
                cat.Load();
            }
        }
        //Get Cat
        else if (questId == 18)
        {

        }
        //Go Out
        else if (questId == 19)
        {
            if (step == 0)
            {
                if (GameManager.instance.GetActivePet().actionType != ActionType.OnGarden)
                    StartCoroutine(HoldToDoor());
            }
        }
        //Pan Camera
        else if(questId == 20)
        {
            if (GameManager.instance.myPlayer.gameType != GameType.Garden)
            {
                Vector3 pos = ItemManager.instance.GetActiveCamera().transform.position;
                pos.x = ItemManager.instance.GetActiveCamera().boundX.x;
                pointer.transform.position = pos;
                ItemManager.instance.GetActiveCamera().SetTarget(pointer.gameObject);
                blackScreenUI.SetActive(true);
                blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                StartCoroutine(OnGarden());
            }

        }
        else if (questId == 21)
        {
            if(GameManager.instance.myPlayer.gameType == GameType.House)
            {
                UIManager.instance.OnGarden();
            }
            if(UIManager.instance.eventPanel == null)
            {
                MinigameOpen[] minigames = FindObjectsOfType<MinigameOpen>();
                for (int i = 0; i < minigames.Length; i++)
                {
                    if (minigames[i].gameId == 0)
                    {
                        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                        blackScreen.SetActive(true);
                        handClick.SetActive(true);
                        ItemManager.instance.SetCameraTarget(minigames[i].gameObject);
                        Camera.main.GetComponent<CameraController>().screenOffset = 0;
                        blackScreenButton.SetActive(true);
                        handClick.transform.position = minigames[i].transform.position + new Vector3(0, 5, -1000);
                        handClick.GetComponent<Animator>().Play("Click", 0);

                    }
                }
            }
            else
            {
                blackScreenUI.SetActive(true);
                EventPanel eventPanel = UIManager.instance.OnEventPanel();
                GameObject go = eventPanel.playButton.gameObject;
                AddSorting(go);
                step = 1;
            }

        }
    }


    void OnShop(int tabID,int itemId)
    {
        if (GameManager.instance.IsEquipItem(itemId))
            return;

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
                blackScreenUI.SetActive(true);
                ShopPanel shop = UIManager.instance.OnShopPanel();
                GameObject go = shop.toogleAnchor.GetChild(tabID).gameObject;
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
        else if (questId == 20)
        {
            Destroy(UIManager.instance.gardenButton.GetComponent<Canvas>());
            UIManager.instance.OnGarden();
        }
        else if (questId == 21)
        {
            if(step == 0)
            {
                blackScreenUI.SetActive(true);
                handClick.SetActive(false);
                EventPanel eventPanel = UIManager.instance.OnEventPanel();
                GameObject go = eventPanel.playButton.gameObject;
                AddSorting(go);
                step = 1;
            }
            else if(step == 1)
            {
                blackScreenUI.SetActive(true);
                EventPanel eventPanel = UIManager.instance.OnEventPanel();
                if (eventPanel != null)
                    eventPanel.OnEvent(0);
            }

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
            if(GameManager.instance.GetHappy() < DataHolder.GetItem(itemId).buyPrice)
            {
                GameManager.instance.AddHappy(DataHolder.GetItem(itemId).buyPrice);
            }
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
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }
        else if(questId == 3)
        {
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Drink).GetComponent<Animator>();
            anim.Play("Idle", 0);
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
        }else if(questId == 20)
        {
            Destroy(UIManager.instance.gardenButton.GetComponent<Canvas>());
            ItemManager.instance.houseCamera.target = null;
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

    IEnumerator Quest0()
    {
        blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreenUI.SetActive(true);
        UIManager.instance.OnPetCollectionPanel();
        
        if (UIManager.instance.petCollectionPanel != null)
            UIManager.instance.petCollectionPanel.OnActive(0);

        yield return new WaitForSeconds(3);

        UIManager.instance.petCollectionPanel.Close();
        blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.3f);
        blackScreenUI.SetActive(false);

        UIManager.instance.OnQuestNotificationPopup("Oh Shiba is very hungry, tap to the food bowl to fill up.");

        FoodBowlItem item = FindObjectOfType<FoodBowlItem>();
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

    protected virtual IEnumerator HoldToBath()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetPetObject(0);
        if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Bath)
        {
            pet.OnControl();
            yield return new WaitForEndOfFrame();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);

            pet.target = pet.transform.position;
            while (Vector2.Distance(pet.transform.position, dropPosition.position) > 1)
            {
                pet.target = Vector3.Lerp(pet.target, dropPosition.position, Time.deltaTime);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            handClick.SetActive(false);
            pet.charInteract.interactType = InteractType.Drop;
        }
       
        if (pet.data.dirty > 50)
        {
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
        blackScreen.SetActive(false);

    }

    protected virtual IEnumerator HoldToToilet()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetPetObject(0);
        pet.OnControl();
        handClick.SetActive(true);
        handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);

        pet.target = pet.transform.position;
        Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Toilet).transform.position + new Vector3(0, 15, 0);
        while (Vector2.Distance(pet.transform.position, holdPosition) > 1)
        {
            pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
        yield return new WaitForSeconds(3);
    }

    protected virtual IEnumerator HoldToSleep()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetPetObject(0);
        pet.OnControl();
        handClick.SetActive(true);
        handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);

        pet.target = pet.transform.position;
        Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Bed).transform.position + new Vector3(0, 14.5f, 0);
        while (Vector2.Distance(pet.transform.position, holdPosition) > 1)
        {
            pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
        yield return new WaitForSeconds(3);
    }

    protected virtual IEnumerator HoldToDoor()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetPetObject(0);
        pet.OnControl();
        handClick.SetActive(true);
        handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);

        pet.target = pet.transform.position;
        Vector3 holdPosition = GameObject.FindGameObjectWithTag("Door").transform.position + new Vector3(0, 15, 0);
        while (Vector2.Distance(pet.transform.position, holdPosition) > 2)
        {
            pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            yield return new WaitForEndOfFrame();
        }
        handClick.SetActive(false);
        pet.charInteract.interactType = InteractType.Drop;
        blackScreen.SetActive(false);
        UIManager.instance.OnQuestNotificationPopup("Now continue to hold the cat to door");
    }

    protected virtual IEnumerator HoldBroom()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        GameObject broom = ItemManager.instance.GetItemChildObject(ItemType.Clean);
        ItemManager.instance.SetCameraTarget(broom);
        handClick.SetActive(true);
        handClick.transform.position = broom.transform.position + new Vector3(0, 3, -10);
        handClick.GetComponent<Animator>().Play("Hold", 0);
        broom.GetComponent<ItemDrag>().isDragable = false;

        ItemDirty dirty = FindObjectOfType<ItemDirty>();

        if (dirty != null)
        {
            Vector3 target = dirty.transform.position + new Vector3(0, 8, 0);
            while (Vector2.Distance(broom.transform.position, target) > 1)
            {
                broom.transform.position = Vector3.Lerp(broom.transform.position, target, Time.deltaTime);
                handClick.transform.position = broom.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1);
            broom.GetComponent<ItemDrag>().Return();
            yield return new WaitForSeconds(1);
            //broom.GetComponent<Animator>().Play("Tutorial", 0);
        }
        handClick.SetActive(false);
        ItemManager.instance.ResetCameraTarget();
        broom.GetComponent<ItemDrag>().isDragable = true;
        blackScreen.SetActive(false);
    }

    IEnumerator OnGarden()
    {
        yield return new WaitForSeconds(1f);
        
        GameObject go = UIManager.instance.gardenButton;
        go.SetActive(true);
        AddSorting(go);
    }

}
