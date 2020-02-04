using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBuyShopPopup : MonoBehaviour
{
    int itemId = 0;
    int orderId = 0;
    int itemReplaceId = 0;
    bool isCharacter = false;
    public Image icon;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public GameObject happyIcon;
    public Text priceText;
    public Text question;
    public GameObject replacePanel;
    public GameObject replaceText;
    public Image replaceIcon;
    ItemState state = ItemState.OnShop;
    public GameObject okButton;
    
    bool isBuy = false;
    bool isReplace = false;

    public void Load(Item d,bool isBuy){

        itemId = d.iD;
        this.isBuy = isBuy;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        replaceText.GetComponent<Text>().text = DataHolder.Dialog(14).GetName(MageManager.instance.GetLanguage()) + " ";

        if (isBuy){
            priceText.text = d.buyPrice.ToString();
            replacePanel.SetActive(false);
            question.text = DataHolder.Dialog(3).GetDescription(MageManager.instance.GetLanguage()) + " ";
        }
        else{
            question.text = DataHolder.Dialog(4).GetDescription(MageManager.instance.GetLanguage()) + " ";
            replacePanel.SetActive(false);
            priceText.text = (d.buyPrice/2).ToString();
        }        

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(true);
        }
    }


    public void Load(Pet d,bool isBuy,int colorId = 0){
        isCharacter = true;
        this.isBuy = isBuy;
        itemId = d.iD;
        orderId = colorId;
        string url = d.petColors[colorId].iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        replaceText.GetComponent<Text>().text = DataHolder.Dialog(14).GetName(MageManager.instance.GetLanguage()) + " ";

        if (isBuy)
        {
            priceText.text = d.petColors[colorId].buyPrice.ToString();
            replacePanel.SetActive(false);
            question.text = DataHolder.Dialog(3).GetDescription(MageManager.instance.GetLanguage()) + " ";
        }
        else
        {
            question.text = DataHolder.Dialog(4).GetDescription(MageManager.instance.GetLanguage()) + " ";
            replacePanel.SetActive(false);
            priceText.text = (d.petColors[colorId].buyPrice / 2).ToString();
        }

        if (d.petColors[colorId].priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.petColors[colorId].priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.petColors[colorId].priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
            happyIcon.SetActive(false);
        }
        else if (d.petColors[colorId].priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(true);
        }
    }

    public void Confirm(){
        if(isCharacter){
            if(isBuy){
                UIManager.instance.BuyPetColor(itemId,orderId);
            }else
                UIManager.instance.SellPet(itemId);
        }else{
            if(isBuy){
                if(isReplace)
                    UIManager.instance.SellItem(itemReplaceId);
                UIManager.instance.BuyItem(itemId);
            }else
                UIManager.instance.SellItem(itemId);            
        }
        this.Close();
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }
}
