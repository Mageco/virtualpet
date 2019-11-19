using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public GameObject questUIPrefab;
    public GameObject questCompletePrefab;
    public GameObject shopUIPrefab;
    public static UIManager instance;
	[HideInInspector]
	public Text coinText;
	public Text diamonText;
	MPopup questNotification;
    QuestPanel questComplete;
    ShopPanel shopPanel;

	void Awake()
	{
		if (instance == null)
			instance = this;
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
    }

	public void OnCall()
	{
		InputController.instance.OnCall ();
	}


	public void UpdateUI()
	{
		coinText.text = ApiManager.instance.GetCoin().ToString();
		diamonText.text = ApiManager.instance.GetDiamond().ToString();
	}

	public void BuyItem(int itemID){
	   ApiManager.instance.BuyItem(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
    }

	public void UseItem(int itemID){
       shopPanel.Close();
	   ApiManager.instance.EquipItem(itemID);
       ItemManager.instance.EquipItem();
	}

	public void BuyPet(int itemID){
	   ApiManager.instance.BuyPet(itemID);
        if (shopPanel != null)
            shopPanel.ReLoad();
	}

	public void UsePet(int itemID){
       shopPanel.Close();
       ApiManager.instance.EquipPet(itemID);
       GameManager.instance.EquipPet(itemID);
	}

	public MPopup OnQuestNotificationPopup(string description)
	{
		if (questNotification == null) {
			var popup = Instantiate (questUIPrefab) as GameObject;
			popup.SetActive (true);
			popup.transform.localScale = Vector3.zero;
			popup.transform.SetParent (GameObject.Find ("Canvas").transform, false);
			popup.GetComponent<Popup> ().Open ();
			questNotification = popup.GetComponent<MPopup> ();
			questNotification.texts[0].text = description;
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


}

public enum NotificationType{None,Shop,Skill}
