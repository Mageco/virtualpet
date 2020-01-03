﻿using System.Collections;
using System.Collections.Generic;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public GameObject questUIPrefab;
    public GameObject rateUsUIPrefab;
    public GameObject questCompletePrefab;
    public GameObject shopUIPrefab;
    public GameObject eventUIPrefab;

    public GameObject profileUIPrefab;
    public GameObject skillCompletePrefab;
    public GameObject achivementPrefab;
    public GameObject confirmBuyShopPrefab;
    public GameObject itemInfoPrefab;
    public GameObject treatmentPopupPrefab;
    public GameObject treatmentConfirmPrefab;
    public static UIManager instance;
	public Text coinText;
	public Text diamonText;
    public Text heartText;
    [HideInInspector]
	public NotificationPopup questNotification;
    [HideInInspector]
    public QuestPanel questComplete;
    [HideInInspector]
    public ShopPanel shopPanel;
    [HideInInspector]
    public EventPanel eventPanel;
    [HideInInspector]
    public SkillCompletePanel skillCompletePanel;
    [HideInInspector]
    public ConfirmBuyShopPopup confirmBuyShopPopup;
    [HideInInspector]
    public RatingWindow ratingWindow;

    public GameObject achivementNotification;
    public GameObject eventNotification;

    [HideInInspector]
    public AchivementPanel achivementPanel;

    ProfilePanel profilePanel;

    ItemInfoUI itemInfoUI;
    TreatmentPopup treatmentPopup;
    TreatmentConfirmPopup treatmentConfirmPopup;

    public GameObject homeUI;

    public GameObject profilePrefab;
    public Transform profileAnchor;
    List<ProfileUI> profiles = new List<ProfileUI>();
    public List<string> notificationText = new List<string>();

    public GameObject doubleClickEffect;
	GameObject doubleClick;

    public GameObject eventButton;
    public GameObject achivementButton;
    public GameObject shopButton;

	void Awake()
	{
		if (instance == null)
			instance = this;
        else 
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
	}
    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        if(notificationText.Count > 0){
            OnQuestNotificationPopup(notificationText[0]);
        }
    }



	public void UpdateUI()
	{
        if(coinText != null)
		    coinText.text = GameManager.instance.GetCoin().ToString();
        if(diamonText != null)
		    diamonText.text = GameManager.instance.GetDiamond().ToString();
        if(heartText != null)
            heartText.text = GameManager.instance.GetHappy().ToString();
	}

	public void BuyItem(int itemID){
        MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.CheckOutItem ,DataHolder.GetItem(itemID).GetName(MageManager.instance.GetLanguage()));
        GameManager.instance.BuyItem(itemID);
      GameManager.instance.EquipItem(itemID);
      ItemManager.instance.EquipItem();
      GameManager.instance.LogAchivement(AchivementType.Buy_Item);
        if (shopPanel != null)
            shopPanel.Close();
    }

    public void SellItem(int itemID){
        GameManager.instance.SellItem(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
	}

	public void UseItem(int itemID){
       shopPanel.Close();
	  GameManager.instance.EquipItem(itemID);
       ItemManager.instance.EquipItem();
	}

	public void BuyPet(int itemID){
        MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.CheckOutItem, DataHolder.GetPet(itemID).GetName(MageManager.instance.GetLanguage()));
        GameManager.instance.BuyPet(itemID);
      GameManager.instance.EquipPet(itemID);
        if (shopPanel != null)
            shopPanel.Close();
      
	}

    public void SellPet(int itemID){
	  GameManager.instance.SellPet(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
	}

	public void UsePet(int itemID){
       shopPanel.Close();
       GameManager.instance.EquipPet(itemID);
	}

    public void OnQuestNotification()
    {
        if(QuestManager.instance != null && (GameManager.instance.myPlayer.questId < DataHolder.Quests().GetDataCount()))
            OnQuestNotificationPopup(DataHolder.Quest(GameManager.instance.myPlayer.questId).GetDescription(MageManager.instance.GetLanguage()));
        else
        {
            int id = Random.Range(0, DataHolder.Dialogs().GetDataCount());
            //if(DataHolder.Instance() != null)
            OnQuestNotificationPopup(DataHolder.Dialog(id).GetDescription(MageManager.instance.GetLanguage()));

        }
    }

	public NotificationPopup OnQuestNotificationPopup(string description)
	{
        if(!notificationText.Contains(description))
            notificationText.Add(description);
		if (questNotification == null) {
			var popup = Instantiate (questUIPrefab) as GameObject;
			popup.SetActive (true);
			popup.transform.SetParent (GameObject.Find ("Canvas").transform, false);
            popup.transform.localScale = Vector3.one;
			//popup.GetComponent<Popup> ().Open ();
			questNotification = popup.GetComponent<NotificationPopup> ();
			questNotification.Load("",description);
		}
		return questNotification;
	}

    public QuestPanel OnQuestCompletePopup()
    {
        if (questComplete == null)
        {
            var popup = Instantiate(questCompletePrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            questComplete = popup.GetComponent<QuestPanel>();
        }
        return questComplete;
    }

    public ShopPanel OnShopPanel()
    {
        if (shopPanel == null)
        {
            var popup = Instantiate(shopUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            shopPanel = popup.GetComponent<ShopPanel>();
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Shop");
        }
        return shopPanel;
     }

    public void OnShopPanelUI()
    {
        if (shopPanel == null)
        {
            var popup = Instantiate(shopUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            shopPanel = popup.GetComponent<ShopPanel>();
        }
    }

    public void OnShopPanel(int id)
    {
        if (shopPanel == null)
        {
            var popup = Instantiate(shopUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            shopPanel = popup.GetComponent<ShopPanel>();
            shopPanel.ReLoadTab(id);
        }
     }


    public EventPanel OnEventPanel()
    {
        if (eventPanel == null)
        {
            var popup = Instantiate(eventUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            eventPanel = popup.GetComponent<EventPanel>();
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Minigame");
        }
        return eventPanel;
     }

    public void OnEventPanelUI()
    {
        if (eventPanel == null)
        {
            var popup = Instantiate(eventUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            eventPanel = popup.GetComponent<EventPanel>();
        }
    }

    public void OnProfilePanel()
    {
        if (profilePanel == null)
        {
            var popup = Instantiate(profileUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            profilePanel = popup.GetComponent<ProfilePanel>();
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Profile");
        }
     }

    public void OnSkillCompletePanel(SkillType t)
    {
        if (skillCompletePanel == null)
        {
            var popup = Instantiate(skillCompletePrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            skillCompletePanel = popup.GetComponent<SkillCompletePanel>();
            skillCompletePanel.Load(t);
        }
     }

    public AchivementPanel OnAchivementPanel()
    {
        if (achivementPanel == null)
        {
            var popup = Instantiate(achivementPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            achivementPanel = popup.GetComponent<AchivementPanel>();
            achivementPanel.Load();
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Achivement");
        }
        return achivementPanel;
     }

    public void OnAchivementPanelUI()
    {
        if (achivementPanel == null)
        {
            var popup = Instantiate(achivementPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            achivementPanel = popup.GetComponent<AchivementPanel>();
            achivementPanel.Load();
        }
    }

    public void OnItemInfoPanel(int itemId,bool isPet)
    {
        if (itemInfoUI == null)
        {
            var popup = Instantiate(itemInfoPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            itemInfoUI = popup.GetComponent<ItemInfoUI>();
            if(isPet)
                itemInfoUI.Load(DataHolder.GetPet(itemId));
            else
                itemInfoUI.Load(DataHolder.GetItem(itemId));
        }
     }

    public void OnConfirmationShopPanel(int itemid,bool isCharacter,bool isBuy)
    {
        if (confirmBuyShopPopup == null)
        {
            var popup = Instantiate(confirmBuyShopPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            confirmBuyShopPopup = popup.GetComponent<ConfirmBuyShopPopup>();
            if(isCharacter)
                confirmBuyShopPopup.Load(DataHolder.GetPet(itemid),isBuy);
            else
                confirmBuyShopPopup.Load(DataHolder.GetItem(itemid),isBuy);
        }
     }

    public void OnTreatmentPopup(Pet p, SickType sickType)
    {
        if (treatmentPopup == null)
        {
            var popup = Instantiate(treatmentPopupPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            treatmentPopup = popup.GetComponent<TreatmentPopup>();
            treatmentPopup.Load(p, sickType);
        }
    }

    public void OnTreatmentConfirmPopup(Pet p, SickType sickType)
    {
        if (treatmentConfirmPopup == null)
        {
            var popup = Instantiate(treatmentConfirmPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            treatmentConfirmPopup = popup.GetComponent<TreatmentConfirmPopup>();
            treatmentConfirmPopup.Load(p, sickType);
        }
    }

    public void OnRatingPopup()
    {
        if (ratingWindow == null)
        {
            var popup = Instantiate(rateUsUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            ratingWindow = popup.GetComponent<RatingWindow>();
        }
    }

    public void OnHome(){
        MageManager.instance.PlaySoundName("BubbleButton", false);
        GameManager.instance.gameType = GameType.House;
        MageManager.instance.LoadSceneWithLoading("House");
        homeUI.SetActive(true);
     }




    public void OnMinigame(int id){
        //ES3AutoSaveMgr.Instance.Save();
        MageManager.instance.LoadSceneWithLoading("Minigame" + id.ToString());
        GameManager.instance.gameType = GameType.Minigame1;
        //GameManager.instance.GetActivePet().Load();
        homeUI.SetActive(false);
        
    }

}

public enum NotificationType{None,Shop,Skill}
