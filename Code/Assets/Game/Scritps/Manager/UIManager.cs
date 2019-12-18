using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public GameObject questUIPrefab;
    public GameObject questCompletePrefab;
    public GameObject shopUIPrefab;
    public GameObject eventUIPrefab;

    public GameObject profileUIPrefab;
    public GameObject skillCompletePrefab;
    public GameObject achivementPrefab;
    public GameObject confirmBuyShopPrefab;
    public GameObject itemInfoPrefab;
    public static UIManager instance;
	public Text coinText;
	public Text diamonText;
    public Text heartText;
	NotificationPopup questNotification;
    QuestPanel questComplete;
    ShopPanel shopPanel;
    EventPanel eventPanel;
    SkillCompletePanel skillCompletePanel;
    ConfirmBuyShopPopup confirmBuyShopPopup;

    [HideInInspector]
    public AchivementPanel achivementPanel;

    ProfilePanel profilePanel;

    ItemInfoUI itemInfoUI;

    public GameObject homeUI;

    public GameObject profilePrefab;
    public Transform profileAnchor;
    List<ProfileUI> profiles = new List<ProfileUI>();
    public List<string> notificationText = new List<string>();

    public GameObject doubleClickEffect;
	GameObject doubleClick;

	void Awake()
	{
		if (instance == null)
			instance = this;
        else 
            GameObject.Destroy(this.gameObject);

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
	  GameManager.instance.BuyItem(itemID);
      GameManager.instance.EquipItem(itemID);
      ItemManager.instance.EquipItem();
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

    public void OnShopPanel()
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

    public void OnEventPanel()
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

    public void OnAchivementPanel()
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

     public void OnHome(){
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
