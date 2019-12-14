using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBuyShopPopup : MonoBehaviour
{
    int itemId = 0;
    int itemReplaceId = 0;
    bool isCharacter = false;
    public Image icon;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public Text priceText;
    public Text question;
    public GameObject replacePanel;
    public GameObject replaceText;
    public Image replaceIcon;
    ItemState state = ItemState.OnShop;
    
    bool isBuy = false;
    bool isReplace = false;

    public void Load(Item d,bool isBuy){

        itemId = d.iD;
        this.isBuy = isBuy;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        
        if(isBuy){
            Item replaceItem = GameManager.instance.GetEquipedItem(d.itemType);
            question.text = "Buy this Item with ";
            priceText.text = d.buyPrice.ToString();
            if(replaceItem == null)
            {
                replacePanel.SetActive(false);
            }else{
                itemReplaceId = replaceItem.iD;
                isReplace = true;
                replacePanel.SetActive(true);
                string url1 = replaceItem.iconUrl.Replace("Assets/Game/Resources/", "");
                url1 = url1.Replace(".png", "");
                replaceIcon.sprite = Resources.Load<Sprite>(url1) as Sprite;
            }
            

        }else{
            question.text = "Sell this Item with";
            replacePanel.SetActive(false);
            priceText.text = d.buyPrice.ToString();
        }        

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }
    }


    public void Load(Pet d,bool isBuy){
        isCharacter = true;
        this.isBuy = isBuy;
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        replacePanel.SetActive(false);
        if(isBuy){
            question.text = "Buy this Item with ";
            priceText.text = (d.buyPrice).ToString();
        }else{
            question.text = "Sell this Item with ";
            priceText.text = d.buyPrice.ToString();
        }        

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }
    }

    public void Confirm(){
        if(isCharacter){
            if(isBuy){
                UIManager.instance.BuyPet(itemId);
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
