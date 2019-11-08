using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public GameObject questUIPrefab;
    public GameObject questCompletePrefab;
    public static UIManager instance;
	[HideInInspector]
	public NotificationType notification = NotificationType.None;
	public Text coinText;
	public Text diamonText;
	MPopup questNotification;
    QuestPanel questComplete;

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

	public void OnNotify(NotificationType type){
		notification = type;
	}

	public void UpdateUI()
	{
		coinText.text = ApiManager.instance.GetCoin().ToString();
		diamonText.text = ApiManager.instance.GetDiamond().ToString();
	}

	public void BuyItem(int itemID){
	   ApiManager.instance.BuyItem(itemID);
	   notification = NotificationType.Shop;
	}

	public void UseItem(int itemID){
	   ApiManager.instance.UseItem(itemID);
       ItemController.instance.UseItem(itemID);
	   notification = NotificationType.Shop;
	}

	public void BuyPet(int itemID){
	   ApiManager.instance.BuyPet(itemID);
	   notification = NotificationType.Shop;
	}

	public void UsePet(int itemID){
	   ApiManager.instance.UsePet(itemID);
       ItemController.instance.UsePet(itemID);
	   notification = NotificationType.Shop;
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
            popup.transform.localScale = Vector3.zero;
            popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
            popup.GetComponent<Popup>().Open();
            questComplete = popup.GetComponent<QuestPanel>();
        }
        return questComplete;
    }


}

public enum NotificationType{None,Shop,Skill}
