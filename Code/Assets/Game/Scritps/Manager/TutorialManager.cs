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
        //Tap to the water bowl
        else if (questId == 1)
        {
            if (step == 0)
            {
                DrinkBowlItem item = FindObjectOfType<DrinkBowlItem>();
                if (item != null && item.foodAmount < item.maxfoodAmount - 1)
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
        //Take Bath
        else if (questId == 2)
        {
            if (step == 0)
            {
                
                 StartCoroutine(HoldToBath());
            }
        }
        //Take out of bath
        else if (questId == 3)
        {
            if (step == 0)
            {

                StartCoroutine(HoldOutFromBath());
            }
        }

        //Clean
        else if (questId == 4)
        {
            if (step == 0)
            {
                if(FindObjectOfType<ItemDirty>() != null)
                    StartCoroutine(HoldBroom());
            }
        }
        //Go to toilet
        else if (questId == 5)
        {
            if (step == 0)
            {
                if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Toilet)
                    StartCoroutine(HoldToToilet());
            }
        }
        //Go to sleep
        else if (questId == 6)
        {
            if (step == 0)
            {
                if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Bed)
                    StartCoroutine(HoldToSleep());
            }
        }
        //Talk with Cat
        else if (questId == 7)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if(cat != null)
            {
                ItemManager.instance.SetCameraTarget(cat.gameObject);
                Camera.main.GetComponent<CameraController>().screenOffset = 0;
                cat.Load();
                blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
                blackScreen.SetActive(true);
                handClick.SetActive(true);
                blackScreenButton.SetActive(true);
                handClick.transform.position = cat.transform.position + new Vector3(0, 0, -1000);
                handClick.GetComponent<Animator>().Play("Click", 0);
            }
        }
        //On Buy equipment
        else if (questId == 8)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
            {
                cat.Load();
            }
            StartCoroutine(BuyToys());
        }
        //Get Cat
        else if (questId == 9)
        {

        }
        else if (questId == 10)
        {
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
                EventPanel eventPanel = UIManager.instance.OnEventPanel(0);
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


    IEnumerator BuyToys()
    {
        yield return new WaitForSeconds(3);
        if(UIManager.instance.petRequirementPanel != null)
        {
            UIManager.instance.petRequirementPanel.Close();
        }
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
                GameObject go = shop.toogleAnchor.GetChild(4).gameObject;
                AddSorting(go);
                step = 1;
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
            handClick.SetActive(false);
            FindObjectOfType<DrinkBowlItem>().Fill();

        }
        else if(questId == 2)
        {

        }
        else if (questId == 7)
        {
            handClick.SetActive(false);
            blackScreen.SetActive(false);
            blackScreenButton.SetActive(false);
            Pet pet = DataHolder.GetPet(1);
            UIManager.instance.OnPetRequirementPanel(pet);
        }
        else if (questId == 8)
        {
            ClickShopToy();
        }
        else if (questId == 10)
        {
            if(step == 0)
            {
                blackScreenUI.SetActive(true);
                handClick.SetActive(false);
                EventPanel eventPanel = UIManager.instance.OnEventPanel(0);
                GameObject go = eventPanel.playButton.gameObject;
                AddSorting(go);
                step = 1;
            }
            else if(step == 1)
            {
                blackScreenUI.SetActive(true);
                EventPanel eventPanel = UIManager.instance.OnEventPanel(0);
                if (eventPanel != null)
                    eventPanel.OnEvent(0);
            }

        }
    }

    

    //Buy Toy for quest id = 16
    void ClickShopToy()
    {
        if(!GameManager.instance.IsEquipItem(1))
        {
            if (step == 0)
            {
                Debug.Log("OnClick");
                Destroy(UIManager.instance.shopButton.GetComponent<Canvas>());
                ShopPanel shop = UIManager.instance.OnShopPanel();
                GameObject go = shop.toogleAnchor.GetChild(4).gameObject;
                AddSorting(go);
                step = 1;
            }
            else if (step == 1)
            {
                if (GameManager.instance.GetHappy() < DataHolder.GetItem(1).buyPrice)
                {
                    GameManager.instance.AddHappy(DataHolder.GetItem(1).buyPrice);
                }
                Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(4).gameObject.GetComponent<Canvas>());
                UIManager.instance.shopPanel.ReLoadTab(4);
                UIManager.instance.shopPanel.ScrollToItem(4);
                ItemUI item = UIManager.instance.shopPanel.GetItem(1);
                AddSorting(item.buyButton.gameObject);

                step = 2;
            }
            else if (step == 2)
            {

                ItemUI item = UIManager.instance.shopPanel.GetItem(1);
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
                UIManager.instance.BuyItem(1);
            }
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
        else if(questId == 1)
        {
            Animator anim = ItemManager.instance.GetItemChildObject(ItemType.Drink).GetComponent<Animator>();
            anim.Play("Idle", 0);
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }else if(questId == 2)
        {
            if (FindObjectOfType<SoapItem>() != null && FindObjectOfType<BathShowerItem>() != null)
            {
                FindObjectOfType<SoapItem>().GetComponent<Animator>().Play("Idle", 0);
                FindObjectOfType<BathShowerItem>().GetComponent<Animator>().Play("Idle", 0);
            }
        }else if(questId == 7)
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

    IEnumerator Quest0()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(4);
        UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(0).GetDescription(MageManager.instance.GetLanguage()));

        FoodBowlItem item = FindObjectOfType<FoodBowlItem>();
        if (GameManager.instance.myPlayer.questId == 0 && item != null && item.foodAmount < item.maxfoodAmount - 2)
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
        while(pet.charInteract.interactType != InteractType.None)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Bath)
        {
            pet.OnControl();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);
            Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Bath).transform.position + new Vector3(0, 16f, 0);
            pet.target = pet.transform.position;
            while (Vector2.Distance(pet.transform.position, holdPosition) > 2f)
            {
                pet.target = Vector3.Lerp(pet.target, dropPosition.position, Time.deltaTime);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            handClick.SetActive(false);
            yield return new WaitForEndOfFrame();
            pet.charInteract.interactType = InteractType.Drop;
        }
       
        if (pet.data.dirty > 50)
        {
            ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().enabled = true;
            ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().Play("Tutorial", 0);
            yield return new WaitForSeconds(6);
            ItemManager.instance.GetItem(ItemType.Bath).GetComponent<Animator>().enabled = false;
            UIManager.instance.OnQuestNotificationPopup(DataHolder.Dialog(1).GetDescription(MageManager.instance.GetLanguage()));
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
        while (pet.charInteract.interactType != InteractType.None)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Toilet)
        {
            pet.OnControl();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);

            pet.target = pet.transform.position;
            Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Toilet).transform.position + new Vector3(0, 16f, 0);
            while (Vector2.Distance(pet.transform.position, holdPosition) > 2)
            {
                pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            handClick.SetActive(false);
            pet.charInteract.interactType = InteractType.Drop;
            yield return new WaitForSeconds(3);
        }
    }

    protected virtual IEnumerator HoldToSleep()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetPetObject(0);
        while (pet.charInteract.interactType != InteractType.None)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        if (GameManager.instance.GetActivePet().enviromentType != EnviromentType.Bed)
        {
            pet.OnControl();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);

            pet.target = pet.transform.position;
            Vector3 holdPosition = ItemManager.instance.GetItemChildObject(ItemType.Bed).transform.position + new Vector3(0, 16f, 0);
            while (Vector2.Distance(pet.transform.position, holdPosition) > 2)
            {
                pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }

            handClick.SetActive(false);
            pet.charInteract.interactType = InteractType.Drop;
            yield return new WaitForSeconds(3);
        }
    }


    protected virtual IEnumerator HoldOutFromBath()
    {
        CharController pet = GameManager.instance.GetPetObject(0);
        float time = 0;
        while(time < 5 && questId == 8)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(pet.enviromentType == EnviromentType.Bath)
        {
            blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
            blackScreen.SetActive(true);

            pet.OnControl();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);

            pet.target = pet.transform.position;
            Vector3 holdPosition = pet.transform.position + new Vector3(0, 5 , 0);
            while (Vector2.Distance(pet.transform.position, holdPosition) > 2)
            {
                pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            holdPosition = ItemManager.instance.GetItem(ItemType.Bath).transform.position + new Vector3(0, -2, 0);
            while (Vector2.Distance(pet.transform.position, holdPosition) > 2)
            {
                pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            handClick.SetActive(false);
            pet.charInteract.interactType = InteractType.Drop;
            blackScreen.SetActive(false);
        }

    }


    protected virtual IEnumerator HoldBroom()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        GameObject broom = ItemManager.instance.GetItemChildObject(ItemType.Clean);


        handClick.SetActive(true);
        handClick.transform.position = broom.transform.position + new Vector3(0, 3, -10);

        ItemDirty dirty = FindObjectOfType<ItemDirty>();

        if (!broom.GetComponent<ItemDrag>().isDrag && dirty != null && !broom.GetComponent<ItemDrag>().isBusy)
        {

            handClick.GetComponent<Animator>().Play("Hold", 0);
            broom.GetComponent<ItemDrag>().isDragable = false;
            ItemManager.instance.SetCameraTarget(broom);
            Vector3 target = dirty.transform.position + new Vector3(0, 8, 0);
            while (Vector2.Distance(broom.transform.position, target) > 1 && !broom.GetComponent<ItemDrag>().isBusy)
            {
                broom.transform.position = Vector3.Lerp(broom.transform.position, target, Time.deltaTime);
                handClick.transform.position = broom.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(3);
            broom.GetComponent<ItemDrag>().ReturnOriginal();
            yield return new WaitForSeconds(1);
            //broom.GetComponent<Animator>().Play("Tutorial", 0);
            ItemManager.instance.ResetCameraTarget();
        }
        handClick.SetActive(false);
       
        broom.GetComponent<ItemDrag>().isDragable = true;
        blackScreen.SetActive(false);
    }


}
