﻿using System.Collections;
using System.Collections.Generic;
using Mage.Models.Users;
using MageSDK.Client;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public GameObject questUIPrefab;
    public GameObject rateUsUIPrefab;
    public GameObject shopUIPrefab;
    public GameObject eventUIPrefab;
    public GameObject achivementPrefab;
    public GameObject confirmBuyShopPrefab;
    public GameObject treatmentPopupPrefab;
    public GameObject PetRequirementPanelPrefab;
    public GameObject confirmBuyPetPrefab;
    public GameObject mapPanelPrefab;
    public GameObject rewardItemPrefab;
    public GameObject mapRequirementPrefab;
    public GameObject levelUpPrefab;
    public GameObject welcomeBackPrefab;
    public GameObject servicePanelPrefab;
    public GameObject settingPanelPrefab;
    public GameObject dailyBonusPanelPrefab;
    public GameObject rewardDiamondPanelPrefab;
    public GameObject commomChestPanelPrefab;
    public GameObject rareChestPanelPrefab;
    public GameObject epicChestPanelPrefab;
    public GameObject profilePanelPrefab;
    public GameObject houseNamePanelPrefab;
    public GameObject takePhotoUIPrefab;
    public GameObject leaderBoardPanelPrefab;
    public GameObject remotePlayerPanellPrefab;
    public GameObject spinWheelPanelPrefab;
    public GameObject spinRewardPanelPrefab;
    public GameObject dailyQuestPanelPrefab;

    public static UIManager instance;
	public Text coinText;
	public Text diamonText;
    public Text heartText;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject heartIcon;
    public Text levelText;
    public Text exp;
    public Image expProgress;
    [HideInInspector]
	public NotificationPopup questNotification;
    [HideInInspector]
    public QuestPanel questComplete;
    [HideInInspector]
    public ShopPanel shopPanel;
    [HideInInspector]
    public EventPanel eventPanel;
    [HideInInspector]
    public ConfirmBuyShopPopup confirmBuyShopPopup;
    [HideInInspector]
    public RatingWindow ratingWindow;
    [HideInInspector]
    public PetRequirementPanel petRequirementPanel;
    [HideInInspector]
    public ConfirmBuyPetPopup confirmBuyPetPopup;
    [HideInInspector]
    public RewardItemPanel rewardItemPanel;
    [HideInInspector]
    public MapRequirementPanel mapRequirementPanel;
    [HideInInspector]
    public MapPanel mapPanel;
    [HideInInspector]
    public LevelUpPanel levelUpPanel;
    [HideInInspector]
    public WelcomeBackPanel welcomeBackPanel;
    [HideInInspector]
    public ServicePanel servicePanel;
    [HideInInspector]
    public SettingPopUp settingPanel;
    [HideInInspector]
    public DailyBonusPanel dailyBonusPanel;
    [HideInInspector]
    public RewardDiamondPanel rewardDiamondPanel;
    [HideInInspector]
    public ProfilePanel profilePanel;
    [HideInInspector]
    public HouseNamePanel houseNamePanel;
    [HideInInspector]
    public TakePhotoUI takePhotoUI;
    [HideInInspector]
    public ChestSalePanel chestSalePanel;
    [HideInInspector]
    public LeaderBoardPanel leaderBoardPanel;
    [HideInInspector]
    public RemotePlayerPanel remotePlayerPanel;
    [HideInInspector]
    public SpinWheelPanel spinWheelPanel;
    [HideInInspector]
    public SpinRewardPanel spinRewardPanel;
    [HideInInspector]
    public DailyQuestPanel dailyQuestPanel;

    public GameObject achivementNotification;
    public GameObject giftNotification;

    [HideInInspector]
    public AchivementPanel achivementPanel;


    ItemInfoUI itemInfoUI;
    TreatmentPopup treatmentPopup;

    public GameObject homeUI;
    public GameObject toolUI;
    public GameObject profilePrefab;
    public GameObject notificationIcon;
    
    //public List<string> notificationText = new List<string>();

    public GameObject eventButton;
    public GameObject achivementButton;
    public GameObject shopButton;
    public GameObject callButton;
    public GameObject remotePanel;

    public AvatarUI avatarUI;
    public AvatarUI remoteAvatarUI;

    float coin = 0;
    float happy = 0;
    float diamond = 0;

    float timeUpdate;
    float maxTimeUpdate = 1f;

    //Sale
    public GameObject saleButton;
    RareType rareType = RareType.Common;
    float timeSale = 0;
    float maxTimeSale = 15;
    bool isSale = false;

    void Awake()
	{
		if (instance == null)
			instance = this;
        else 
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (ItemManager.instance != null && !ItemManager.instance.isLoad)
        {
            yield return new WaitForEndOfFrame();
        }
        Load();
    }

    public void Load()
    {
        toolUI.gameObject.SetActive(true);
        UpdateUI();
        coin = GameManager.instance.GetCoin();
        happy = GameManager.instance.GetHappy();
        diamond = GameManager.instance.GetDiamond();
        remotePanel.SetActive(false);
        OnSale();
        MageEngine.instance.LoadAvatar(
            (texture2D) =>
            {
                if(texture2D != null)
                    UIManager.instance.avatarUI.LoadAvatar(Utils.instance.CreateSprite(texture2D));
            });
                
        

        if (callButton != null)
        {
            if (GameManager.instance.myPlayer.questId > 7)
                callButton.SetActive(true);
            else
                callButton.SetActive(false);
        }

        if (timeUpdate > maxTimeUpdate)
        {
            timeUpdate = 0;
 
            //Check Achivement Notification
            if (GameManager.instance.IsCollectAchivement())
            {
                UIManager.instance.achivementNotification.SetActive(true);
            }
            else
                UIManager.instance.achivementNotification.SetActive(false);

            //Check Gift Notification
            if (GameManager.instance.IsCollectDailyGift())
            {
                UIManager.instance.giftNotification.SetActive(true);
            }
            else
                UIManager.instance.giftNotification.SetActive(false);

            if (GameManager.instance.gameTime > 400 && !ES2.Exists("RateUs") && (int)GameManager.instance.gameTime % 400 == 0)
            {
                UIManager.instance.OnRatingPopup();
            }
        }
        else
        {
            timeUpdate += Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        timeUpdate = 0;

        if (GameManager.instance.myPlayer.questId >= DataHolder.Quests().GetDataCount() && notificationIcon != null)
            notificationIcon.SetActive(false);




        if (isSale)
        {
            if (timeSale > maxTimeSale)
            {
                OffSale();
                timeSale = 0;
            }
            else
                timeSale += Time.deltaTime;
        }
    }



	public void UpdateUI()
	{
        if (GameManager.instance.isLoad)
        {
            coin = Mathf.Lerp(coin, GameManager.instance.GetCoin(), Time.deltaTime * 5);
            happy = Mathf.Lerp(happy, GameManager.instance.GetHappy(), Time.deltaTime * 5);
            diamond = Mathf.Lerp(diamond, GameManager.instance.GetDiamond(), Time.deltaTime * 5);

            if (coinText != null)
                coinText.text = coin.ToString("F0");
            if (diamonText != null)
                diamonText.text = diamond.ToString("F0");
            if (heartText != null)
                heartText.text = happy.ToString("F0");
            if (levelText != null)
                levelText.text = GameManager.instance.myPlayer.level.ToString();

            int level = GameManager.instance.myPlayer.level;
            float e = 20 * level + 20 * level * level;
            float e1 = 20 * (level - 1) + 20 * (level - 1) * (level - 1);
            int n = GameManager.instance.myPlayer.exp;
            exp.text = (n - e1).ToString("F0") + "/" + (e - e1).ToString("F0");
            expProgress.fillAmount = (n - e1) / (e - e1);
        }
    }

	public void BuyItem(int itemID){
        MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.CheckoutIAPWindow ,DataHolder.GetItem(itemID).GetName(MageManager.instance.GetLanguage()));
        GameManager.instance.BuyItem(itemID);
      GameManager.instance.EquipItem(itemID);
      if(ItemManager.instance != null)
            ItemManager.instance.EquipItem();
      GameManager.instance.LogAchivement(AchivementType.Buy_Item);
        if (shopPanel != null)
            shopPanel.ReLoad();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();
    }

    public void EquipItem(int itemId)
    {
        GameManager.instance.EquipItem(itemId);
        if (shopPanel != null)
            shopPanel.ReLoad();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();
    }

    public void UnEquipItem(int itemId)
    {
        GameManager.instance.UnEquipItem(itemId);
        if (shopPanel != null)
            shopPanel.ReLoad();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();
    }

    public void SellItem(int itemID){
        GameManager.instance.SellItem(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();
    }

	public void UseItem(int itemID){
        shopPanel.Close();
	    GameManager.instance.EquipItem(itemID);
        if(ItemManager.instance != null)
            ItemManager.instance.EquipItem();
	}

	public void BuyPet(int itemID){
        MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.CheckOutItem, DataHolder.GetPet(itemID).GetName(MageManager.instance.GetLanguage()));

        GameManager.instance.BuyPet(itemID);
        if(ItemManager.instance != null)
            GameManager.instance.EquipPet(itemID);
        if (shopPanel != null)
            shopPanel.Close();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();

        //OnPetCollectionPanel();
        //petCollectionPanel.OnActive(itemID);

        if (ItemManager.instance != null && ItemManager.instance.GetCharCollector(itemID) != null)
            ItemManager.instance.GetCharCollector(itemID).DeActive();
      
	}



    public void SellPet(int itemID){
	  GameManager.instance.SellPet(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();
    }

    public void UnEquipPet(int itemID)
    {
        GameManager.instance.UnEquipPet(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
        if (petRequirementPanel != null)
            petRequirementPanel.ReLoad();
    }

    public void UsePet(int itemID){
      
       GameManager.instance.EquipPet(itemID);
        shopPanel.ReLoad();
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
        //if(!notificationText.Contains(description))
        //    notificationText.Add(description);
        if (questNotification == null)
        {
            var popup = Instantiate(questUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.transform.localScale = Vector3.one;
            //popup.GetComponent<Popup> ().Open ();
            questNotification = popup.GetComponent<NotificationPopup>();
            questNotification.Load("", description);
        }
        else
            questNotification.description.text = description;
		return questNotification;
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
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Shop");
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


    public EventPanel OnEventPanel(int id)
    {
        if (eventPanel == null)
        {
            var popup = Instantiate(eventUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            eventPanel = popup.GetComponent<EventPanel>();
            eventPanel.Load(id);
            
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
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Minigame");
        }
    }


    public void OnPetRequirementPanel(Pet pet)
    {
        if (petRequirementPanel == null)
        {
            var popup = Instantiate(PetRequirementPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            petRequirementPanel = popup.GetComponent<PetRequirementPanel>();
            petRequirementPanel.Load(pet);
        }
    }

    public void OnConfirmBuyPetPopup(Pet pet)
    {
        if (confirmBuyPetPopup == null)
        {
            var popup = Instantiate(confirmBuyPetPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            confirmBuyPetPopup = popup.GetComponent<ConfirmBuyPetPopup>();
            confirmBuyPetPopup.Load(pet);
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
            MageEngine.instance.OnEvent(Mage.Models.Application.MageEventType.OpenStore, "Achivement");
        }
    }


    public void OnConfirmationShopPanel(int itemid,bool isCharacter,bool isBuy,int colorId = 0)
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


    public void OnRewardItemPanel(RewardType rewardType,ChestItem item)
    {
        if (rewardItemPanel == null)
        {
            var popup = Instantiate(rewardItemPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            rewardItemPanel = popup.GetComponent<RewardItemPanel>();
            rewardItemPanel.Load(rewardType,item);
        }
    }

    public void OnMapPanel()
    {
        if (mapPanel == null)
        {
            var popup = Instantiate(mapPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            mapPanel = popup.GetComponent<MapPanel>();
        }
    }


    public void OnMapRequirement(MapType mapType)
    {
        if (mapRequirementPanel == null)
        {
            var popup = Instantiate(mapRequirementPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            mapRequirementPanel = popup.GetComponent<MapRequirementPanel>();
            mapRequirementPanel.Load(mapType);
        }
    }

    public void OnLevelUpPanel()
    {
        if (levelUpPanel == null)
        {
            var popup = Instantiate(levelUpPrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            levelUpPanel = popup.GetComponent<LevelUpPanel>();
            levelUpPanel.Load();
        }
    }

    public void OnWelcomeBack(int c,int h)
    {
        if (welcomeBackPanel == null)
        {
            var popup = Instantiate(welcomeBackPrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            welcomeBackPanel = popup.GetComponent<WelcomeBackPanel>();
            welcomeBackPanel.Load(c,h);
        }
    }

    public void OnServicePanel(ServiceType type)
    {
        if (servicePanel == null)
        {
            var popup = Instantiate(servicePanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            servicePanel = popup.GetComponent<ServicePanel>();
            servicePanel.Load(type);
        }
    }

    public void OnSettingPanel()
    {
        if (settingPanel == null)
        {
            var popup = Instantiate(settingPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            settingPanel = popup.GetComponent<SettingPopUp>();
        }
    }

    public void OnDailyBonusPanel()
    {
        if (dailyBonusPanel == null)
        {
            var popup = Instantiate(dailyBonusPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            dailyBonusPanel = popup.GetComponent<DailyBonusPanel>();
        }
    }

    public void OnRewardDiamondPanel(ForestDiamondItem item)
    {
        if (rewardDiamondPanel == null)
        {
            var popup = Instantiate(rewardDiamondPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            rewardDiamondPanel = popup.GetComponent<RewardDiamondPanel>();
            rewardDiamondPanel.Load(item);
        }
    }

    public void OnProfilePanel()
    {
        if (profilePanel == null)
        {
            var popup = Instantiate(profilePanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            profilePanel = popup.GetComponent<ProfilePanel>();
        }
    }

    public void OnHouseNamePanel()
    {
        if (houseNamePanel == null)
        {
            var popup = Instantiate(houseNamePanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            houseNamePanel = popup.GetComponent<HouseNamePanel>();
        }
    }

    public void OnTakePhotoPanel()
    {
        if (takePhotoUI == null)
        {
            var popup = Instantiate(takePhotoUIPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            takePhotoUI = popup.GetComponent<TakePhotoUI>();
            homeUI.gameObject.SetActive(false);
        }
    }

    public void OffTakePhotoPanel()
    {
        if (takePhotoUI != null)
            Destroy(takePhotoUI.gameObject);
        homeUI.gameObject.SetActive(true);
    }

    public void OnLeaderBoardPanel()
    {
        if (leaderBoardPanel == null)
        {
            var popup = Instantiate(leaderBoardPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            leaderBoardPanel = popup.GetComponent<LeaderBoardPanel>();
        }
    }

    public void OnLeaderBoardPanel(int id)
    {
        if (leaderBoardPanel == null)
        {
            var popup = Instantiate(leaderBoardPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            leaderBoardPanel = popup.GetComponent<LeaderBoardPanel>();
            leaderBoardPanel.ReLoadTab(id);
        }
    }

    public void OnRemotePlayerPanel(Sprite s)
    {
        if (remotePlayerPanel == null)
        {
            var popup = Instantiate(remotePlayerPanellPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            remotePlayerPanel = popup.GetComponent<RemotePlayerPanel>();
            remotePlayerPanel.Load(s);
        }
    }

    public void OnSpinWheelPanel()
    {
        if (spinWheelPanel == null)
        {
            var popup = Instantiate(spinWheelPanelPrefab) as GameObject;
            popup.SetActive(true);
            //popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            spinWheelPanel = popup.GetComponent<SpinWheelPanel>();
        }
    }

    public void OnSpinRewardPanel(Sprite s, string t)
    {
        if (spinRewardPanel == null)
        {
            var popup = Instantiate(spinRewardPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            spinRewardPanel = popup.GetComponent<SpinRewardPanel>();
            spinRewardPanel.Load(s, t);
        }
    }

    public void OnDailyQuestPanel()
    {
        if (dailyQuestPanel == null)
        {
            var popup = Instantiate(dailyQuestPanelPrefab) as GameObject;
            popup.SetActive(true);
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            dailyQuestPanel = popup.GetComponent<DailyQuestPanel>();
            dailyQuestPanel.Load();
        }
    }


    public void OnChestSalePanel(RareType rareType)
    {
        if (chestSalePanel == null)
        {
            if(rareType == RareType.Common)
            {
                var popup = Instantiate(commomChestPanelPrefab) as GameObject;
                popup.SetActive(true);
                //popup.transform.localScale = Vector3.zero;
                popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
                popup.GetComponent<Popup>().Open();
                chestSalePanel = popup.GetComponent<ChestSalePanel>();
                chestSalePanel.Load(rareType);
            }
            else if (rareType == RareType.Rare)
            {
                var popup = Instantiate(rareChestPanelPrefab) as GameObject;
                popup.SetActive(true);
                //popup.transform.localScale = Vector3.zero;
                popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
                popup.GetComponent<Popup>().Open();
                chestSalePanel = popup.GetComponent<ChestSalePanel>();
                chestSalePanel.Load(rareType);
            }
            else if (rareType == RareType.Epic)
            {
                var popup = Instantiate(epicChestPanelPrefab) as GameObject;
                popup.SetActive(true);
                //popup.transform.localScale = Vector3.zero;
                popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
                popup.GetComponent<Popup>().Open();
                chestSalePanel = popup.GetComponent<ChestSalePanel>();
                chestSalePanel.Load(rareType);
            }
        }
    }

    public void OnHome(){
        MageManager.instance.PlaySound("BubbleButton", false);
        MageManager.instance.LoadSceneWithLoading("House");
        homeUI.SetActive(true);
     }




    public void OnMinigame(int id){
        RewardVideoAdManager.instance.HideBanner();
        if (GameManager.instance.myPlayer.minigameLevels.Length == 1)
        {
            for (int i = 0; i < 20; i++)
            {
                GameManager.instance.myPlayer.minigameLevels = ArrayHelper.Add(0, GameManager.instance.myPlayer.minigameLevels);
            }
        }
        MageManager.instance.LoadSceneWithLoading("Minigame" + id.ToString());
        homeUI.SetActive(false);
        GameManager.instance.petObjects.Clear();
    }

    public void OnMap(MapType type)
    {
        MageManager.instance.LoadSceneWithLoading(type.ToString());
        GameManager.instance.petObjects.Clear();
        if (type == MapType.House)
        {
            callButton.SetActive(true);
            RewardVideoAdManager.instance.HideBanner();
        }
        else
        {
            int n = Random.Range(0, 100);
            if(n > 80)
            {
                RewardVideoAdManager.instance.ShowIntetestial();
            }
            callButton.SetActive(false);
            RewardVideoAdManager.instance.ShowBanner();
        }
            

        if (type == MapType.Forest || type == MapType.Lake)
            homeUI.SetActive(true);
    }

    public void OnFriendHouse(string userId,Sprite avatar)
    {
        
        if (leaderBoardPanel != null)
            leaderBoardPanel.Close();

        if (Minigame.instance != null)
            Minigame.instance.winPanel.Close();

        MageEngine.instance.GetRandomFriend((User u) => {
            int n = Random.Range(0, 100);
            if (n > 70)
            {
                RewardVideoAdManager.instance.ShowIntetestial();
            }
            GameManager.instance.isGuest = true;
            toolUI.gameObject.SetActive(false);
            callButton.SetActive(false);
            remotePanel.SetActive(true);
            remoteAvatarUI.avatar.sprite = null;
            RewardVideoAdManager.instance.ShowBanner();
            GameManager.instance.RemoveAllPetObjects();
            GameManager.instance.guest = u.GetUserData<PlayerData>();
            MageManager.instance.LoadSceneWithLoading("House");
            Debug.Log("Friend: " + u.ToJson());
            MageEngine.instance.LoadImage(
            u.avatar,
            (texture2D) =>
            {
                if (texture2D != null && texture2D.width > 10 && avatar != null)
                {
                    remoteAvatarUI.LoadAvatar(Utils.instance.CreateSprite(texture2D));
                }

            });
            },userId);
    }

    public void OnMyHouse()
    {
        GameManager.instance.isGuest = false;
        toolUI.gameObject.SetActive(true);
        callButton.SetActive(true);
        RewardVideoAdManager.instance.HideBanner();
        GameManager.instance.RemoveAllPetObjects();
        remotePanel.SetActive(false);
        MageManager.instance.LoadSceneWithLoading("House");
    }

    public void OnCall()
    {
        MageManager.instance.PlaySound("whistle_slide_straight_tone_short_04", false);
        foreach (CharController pet in GameManager.instance.GetPetObjects())
        {
            pet.OnCall();
        }
    }

    #region Sale
    public void OnSale()
    {
        if(GameManager.instance.myPlayer.level >= 7 && GameManager.instance.GetItemNumber(130) == 0)
        {
            this.rareType = RareType.Epic;
            isSale = true;
            saleButton.SetActive(true);
        }
        else if (GameManager.instance.myPlayer.level >= 5 && GameManager.instance.GetItemNumber(129) == 0)
        {
            this.rareType = RareType.Rare;
            isSale = true;
            saleButton.SetActive(true);
        }
        else if (GameManager.instance.myPlayer.level >= 3 && GameManager.instance.GetItemNumber(128) < 3)
        {
            this.rareType = RareType.Common;
            isSale = true;
            saleButton.SetActive(true);
        }
        Debug.Log(rareType);
    }

    void OffSale()
    {
        isSale = false;
        saleButton.SetActive(false);
    }

    public void OnOpenSale()
    {
        UIManager.instance.OnChestSalePanel(rareType);
    }


    #endregion

}

public enum NotificationType{None,Shop,Skill}
