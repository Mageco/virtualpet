using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public static UIManager instance;
	[HideInInspector]
	public NotificationType notification = NotificationType.None;
	public Text coinText;
	public Text diamonText;

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
}

public enum NotificationType{None,Shop,Skill}
