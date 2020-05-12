﻿using System.Collections;
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
            if (GameManager.instance.IsEquipItem(41))
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
                    GameObject go = shop.toogleAnchor.GetChild(2).gameObject;
                    AddSorting(go);
                    step = 1;
                }
            }
        }
        //Tap to the food bowl
        else if (questId == 1)
        {
            StartCoroutine(TapToFoodBowl());
        }
        //Tap to dirty on floor
        else if (questId == 3)
        {
            if (step == 0)
            {

            }
        }
        //Name your pet
        else if (questId == 5)
        {
            if (step == 0)
            {

            }
        }
        //Take Bath
        else if (questId == 6)
        {
            if (step == 0)
            {
                StartCoroutine(HoldToBath());
            }

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
        //Talk with Cat
        else if (questId == 12)
        {
            CharCollector cat = ItemManager.instance.GetCharCollector(1);
            if (cat != null)
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
            else if (step == 1)
            {
                Destroy(UIManager.instance.shopPanel.toogleAnchor.GetChild(2).gameObject.GetComponent<Canvas>());
                UIManager.instance.shopPanel.ReLoadTab(2);
                UIManager.instance.shopPanel.ScrollToItem(41);
                ItemUI item = UIManager.instance.shopPanel.GetItem(41);
                AddSorting(item.buyButton.gameObject);
                step = 2;

            }
            else if (step == 2)
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
        }
        else if (questId == 1)
        {
            handClick.SetActive(false);
            FindObjectOfType<EatItem>().Fill();
            if (GameManager.instance.GetPetObject(0) != null)
                GameManager.instance.GetPetObject(0).OnEat();
        }
        else if (questId == 2)
        {

        }
        else if (questId == 7)
        {
            handClick.SetActive(false);
            blackScreen.SetActive(false);
            blackScreenButton.SetActive(false);
            Pet pet = DataHolder.GetPet(1);
            UIManager.instance.OnPetRequirementPanel(pet);
            ItemManager.instance.ResetCameraTarget();
        }
        else if (questId == 8)
        {

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
    }



    public void EndQuest()
    {
        if (questId == 0)
        {
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }
        else if (questId == 1)
        {
            Camera.main.GetComponent<CameraController>().ResetOrthographic();
            Camera.main.GetComponent<CameraController>().screenOffset = 0.7f;
        }
        else if (questId == 2)
        {
            if (FindObjectOfType<SoapItem>() != null && FindObjectOfType<BathShowerItem>() != null)
            {
                FindObjectOfType<SoapItem>().GetComponent<Animator>().Play("Idle", 0);
                FindObjectOfType<BathShowerItem>().GetComponent<Animator>().Play("Idle", 0);
            }
        }
        else if (questId == 7)
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

    IEnumerator TapToFoodBowl()
    {
        blackScreenUI.SetActive(true);
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);    
        EatItem item = FindObjectOfType<EatItem>();
        ItemManager.instance.SetCameraTarget(item.gameObject);
        if (item != null)
        {
            blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
            blackScreen.SetActive(true);
            handClick.SetActive(true);
            Camera.main.GetComponent<CameraController>().screenOffset = 0;
            blackScreenButton.SetActive(true);
            handClick.transform.position = item.transform.position + new Vector3(0, 0, -1000);
            handClick.GetComponent<Animator>().Play("Click", 0);
            yield return new WaitForSeconds(1);

        }
    }

    protected virtual IEnumerator HoldToBath()
    {
        blackScreen.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0);
        blackScreen.SetActive(true);
        CharController pet = GameManager.instance.GetPetObject(0);
        while (pet.charInteract.interactType != InteractType.None)
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (GameManager.instance.GetActivePetObject().equipment.itemType != ItemType.Bath)
        {
            pet.OnControl();
            handClick.SetActive(true);
            handClick.transform.position = pet.transform.position + new Vector3(0, 3, -10);
            handClick.GetComponent<Animator>().Play("Hold", 0);
            Vector3 holdPosition = ItemManager.instance.GetRandomItem(ItemType.Bath).transform.position + new Vector3(0, 10f, 0);
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
        blackScreen.SetActive(false);

    }



}
