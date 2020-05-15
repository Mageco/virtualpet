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
        //Buy the food bowl
        if (questId == 0)
        {
            OnShop(2, 41);
        }
        //Tap to the food bowl
        else if (questId == 1)
        {
            StartCoroutine(TapToFoodBowl());
        }
        //Buy Bath
        else if (questId == 4)
        {
            OnShop(3, 2);
        }
        //Take Bath
        else if (questId == 5)
        {
            if (step == 0)
            {
                StartCoroutine(HoldToBath());
            }
        }
        //Buy Toy
        else if (questId == 7)
        {
            OnShop(4, 85);
        }
        //Chicken defend
        else if (questId == 11)
        {
            if (UIManager.instance.eventPanel == null)
            {
                MinigameOpen[] minigames = FindObjectsOfType<MinigameOpen>();
                for (int i = 0; i < minigames.Length; i++)
                {
                    if (minigames[i].gameId == 0)
                    {
                        blackScreenUI.SetActive(true);
                        blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0f);
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
                blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
                EventPanel eventPanel = UIManager.instance.OnEventPanel(0);
                GameObject go = eventPanel.playButton.gameObject;
                AddSorting(go);
                step = 1;
            }
        }
        //Talk with Cat
        else if (questId == 12)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
            {
                blackScreenUI.SetActive(true);
                blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0f);
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
        //Upgrade pet
        else if (questId == 13)
        {
            if (UIManager.instance.profilePanel == null)
            {
                blackScreenUI.SetActive(true);
                GameObject go = UIManager.instance.petButton;
                AddSorting(go);
            }
            else
            {
                blackScreenUI.SetActive(true);
                blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
                UIManager.instance.OnProfilePanel();
                GameObject go = UIManager.instance.profilePanel.items[0].upgradeButton.gameObject;
                AddSorting(go);
                step = 1;
            }
        }
        //Do lucky spin
        else if(questId == 14)
        {

        }
        //Go to the forest
        else if (questId == 18)
        {

        }
    }

    



    public void OnClick()
    {
        Debug.Log("OnClick");
        if (questId == 0)
        {
            ClickOnShop(2, 41);
        }
        else if (questId == 1)
        {
            QuestManager.instance.OnCompleteQuest();
            blackScreenUI.SetActive(false);
            blackScreen.SetActive(false);
            handClick.SetActive(false);
            FindObjectOfType<EatItem>().Fill();
            if (GameManager.instance.GetActivePetObject() != null)
                GameManager.instance.GetActivePetObject().OnEat();
        }
        else if (questId == 4)
        {
            ClickOnShop(3, 2);
        }
        else if (questId == 7)
        {
            ClickOnShop(4, 85);
        }
        else if (questId == 11)
        {
            if (step == 0)
            {
                blackScreenUI.SetActive(true);
                handClick.SetActive(false);
                EventPanel eventPanel = UIManager.instance.OnEventPanel(0);
                GameObject go = eventPanel.playButton.gameObject;
                AddSorting(go);
                step = 1;
            }
            else if (step == 1)
            {
                blackScreenUI.SetActive(true);
                EventPanel eventPanel = UIManager.instance.OnEventPanel(0);
                if (eventPanel != null)
                    eventPanel.OnEvent(0);
            }
        }
        else if (questId == 12)
        {
            blackScreenUI.SetActive(false);
            handClick.SetActive(false);
            blackScreen.SetActive(false);
            blackScreenButton.SetActive(false);
            Pet pet = DataHolder.GetPet(1);
            UIManager.instance.OnPetRequirementPanel(pet);
            ItemManager.instance.ResetCameraTarget();
        }
        else if (questId == 13)
        {
            if (step == 0)
            {
                Destroy(UIManager.instance.petButton.GetComponent<Canvas>());
                blackScreenUI.SetActive(true);
                blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
                UIManager.instance.OnProfilePanel();
                GameObject go = UIManager.instance.profilePanel.items[0].upgradeButton.gameObject;
                AddSorting(go);
                step = 1;
            }
            else if (step < 5)
            {
                blackScreenUI.SetActive(true);

                if (UIManager.instance.profilePanel != null && UIManager.instance.profilePanel.items.Count > 0)
                    UIManager.instance.profilePanel.items[0].Upgrade();
                step++;
                if(step == 5)
                {
                    handClickUI.transform.SetParent(blackScreenUI.transform);
                    blackScreenUI.SetActive(false);
                    handClickUI.SetActive(false);
                    UIManager.instance.profilePanel.Close();
                }
            }
        }
        else if (questId == 18)
        {
            blackScreen.SetActive(false);
            blackScreenUI.SetActive(false);
            handClick.SetActive(false);
            UIManager.instance.OnMapRequirement(MapType.Forest);
            ItemManager.instance.ResetCameraTarget();
        }
    }



    public void EndQuest()
    {
        ItemManager.instance.ResetCameraTarget();
        step = 0;
        blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
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

    void OnShop(int tabID, int itemId)
    {
        if (step == 0)
        {
            if (GameManager.instance.GetCoin() < DataHolder.GetItem(itemId).buyPrice)
                GameManager.instance.AddCoin(DataHolder.GetItem(itemId).buyPrice - GameManager.instance.GetCoin(), GetKey());
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


    void ClickOnShop(int tabId, int itemId)
    {
        if (step == 0)
        {
            Debug.Log("OnClick");
            Destroy(UIManager.instance.shopButton.GetComponent<Canvas>());
            ShopPanel shop = UIManager.instance.OnShopPanel();
            GameObject go = shop.toogleAnchor.GetChild(tabId).gameObject;
            AddSorting(go);
            step = 1;
        }
        else if (step == 1)
        {
            Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(tabId).gameObject.GetComponent<Canvas>());
            UIManager.instance.shopPanel.ReLoadTab(tabId);
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
            int realId = UIManager.instance.BuyItem(itemId);
            step = 4;
        }
    }

    IEnumerator TapToFoodBowl()
    {
        blackScreenUI.SetActive(true);
        blackScreenUI.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        blackScreen.SetActive(true);    
        EatItem item = FindObjectOfType<EatItem>();
        int price = (int)((item.maxfoodAmount - item.foodAmount) / 10);
        ItemManager.instance.SetCameraTarget(item.gameObject);
        if (GameManager.instance.GetCoin() < price)
            GameManager.instance.AddCoin(price, GetKey());

        if (item != null)
        {
            blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
            blackScreen.SetActive(true);
            handClick.SetActive(true);
            //Camera.main.GetComponent<CameraController>().screenOffset = 0;
            blackScreenButton.SetActive(true);
            handClick.transform.position = item.transform.position + new Vector3(0, 0, -1000);
            handClick.GetComponent<Animator>().Play("Click", 0);
            yield return new WaitForSeconds(1);

        }
    }

    protected virtual IEnumerator HoldToBath()
    {
        //blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        //blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetActivePetObject();
        while (pet != null && pet.charInteract.interactType != InteractType.None)
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (pet != null && (pet.equipment == null || pet.equipment.itemType != ItemType.Bath) && GameManager.instance.GetAchivement(5) == 0)
        {
            pet.OnControl();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);
            Vector3 holdPosition = pet.charScale.scalePosition + new Vector3(0, 10, 0);
            pet.target = pet.transform.position;
            while (Vector2.Distance(pet.transform.position, holdPosition) > 0.5f)
            {
                pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime* 2);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            holdPosition = ItemManager.instance.GetRandomItem(ItemType.Bath).transform.position + new Vector3(0, 10f, 0);
            while (Vector2.Distance(pet.transform.position, holdPosition) > 0.5f)
            {
                pet.target = Vector3.Lerp(pet.target, holdPosition, Time.deltaTime * 2);
                handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
                yield return new WaitForEndOfFrame();
            }
            pet.transform.position = holdPosition;
            handClick.SetActive(false);
            yield return new WaitForEndOfFrame();
            pet.charInteract.interactType = InteractType.Drop;
        }
        //blackScreen.SetActive(false);

    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }

}
