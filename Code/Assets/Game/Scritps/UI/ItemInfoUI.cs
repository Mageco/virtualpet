using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    int itemId = 0;
    public Image icon;
    public Text price;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public Text itemName;
    public Text description;

    public Button buyButton;
    public Text buttonText;

    bool isBusy = false;
    bool isCommingSoon = false;
    bool isCharacter = false;
    ItemState state = ItemState.OnShop;

    void Awake(){
        
    }

    // Start is called before the first frame update
    public void Load(Item d)
    {
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();
        itemName.text = DataHolder.GetItem(itemId).GetName(0);
        description.text = DataHolder.GetItem(itemId).GetDescription(0);

        if(!d.isAvailable){
            isCommingSoon = true;
        }

        if (GameManager.instance.IsEquipItem(d.iD))
        {
            state = ItemState.Equiped;
        }
        else if (GameManager.instance.IsHaveItem(d.iD))
        {
            state = ItemState.Have;
        }
        else
        {
            state = ItemState.OnShop;
        }

        if(isCommingSoon){
            buyButton.interactable = false;
            buttonText.text = "Locked";
            price.text = d.buyPrice.ToString();
        }else{
            if (state == ItemState.OnShop)
            {
               buyButton.interactable = true;
               buttonText.text = "Buy";
               price.text = d.buyPrice.ToString();
            }else if (state == ItemState.Equiped)
            {
                buyButton.interactable = true;
                buttonText.text = "Sell";
                price.text = (d.buyPrice/2).ToString();
                
                if(DataHolder.GetItem(itemId).itemType == ItemType.Room && GameManager.instance.GetBuyItems(ItemType.Room).Count == 1){
                    buyButton.interactable = false;
                }
            }
        }

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
           if(state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetItem(itemId).buyPrice)){
               buyButton.interactable = false;
           }
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            if(state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetItem(itemId).buyPrice)){
               buyButton.interactable = false;
           }
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }

    }


    public void Load(Pet d)
    {
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();
        itemName.text = DataHolder.GetPet(itemId).GetName(0);
        description.text = DataHolder.GetPet(itemId).GetDescription(0);

        if(!d.isAvailable){
            isCommingSoon = true;
        }


        if (GameManager.instance.IsEquipPet(d.iD))
        {
            state = ItemState.Equiped;
        }
        else if (GameManager.instance.IsHavePet(d.iD))
        {
            state = ItemState.Have;
        }
        else
        {
            state = ItemState.OnShop;
        }

        if(isCommingSoon){
            buyButton.interactable = false;
            buttonText.text = "Locked";
            price.text = d.buyPrice.ToString();
        }else{
            if (state == ItemState.OnShop)
            {
               buyButton.interactable = true;
               buttonText.text = "Buy";
               price.text = d.buyPrice.ToString();
            }else if (state == ItemState.Equiped)
            {
                buyButton.interactable = true;
                buttonText.text = "Sell";
                price.text = (d.buyPrice/2).ToString();
                if(GameManager.instance.GetBuyPets().Count == 1){
                    buyButton.interactable = false;
                }
            }
        }


        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
           if(state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetPet(itemId).buyPrice)){
               buyButton.interactable = false;
           }
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            if(state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetItem(itemId).buyPrice)){
               buyButton.interactable = false;
           }
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }
    }

     public void OnBuy()
    {
        if (isBusy)
            return;

        if(isCommingSoon){
            MageManager.instance.OnNotificationPopup("Item will be available soon!");
            return;
        }
        MageManager.instance.PlaySoundName("BubbleButton",false);
        BuyCoroutine();
    }

    void BuyCoroutine()
    {
        isBusy = true;
        
        if (isCharacter)
        {
            if (state == ItemState.Equiped)
                UIManager.instance.OnConfirmationShopPanel(itemId,true,false);
            else if (state == ItemState.Have)
            {
                //animator.Play("Use", 0);
                //yield return new WaitForSeconds(0.5f);
                //UIManager.instance.OnConfirmationShopPanel(itemId,true,false);
            }
            else
            {
                UIManager.instance.OnConfirmationShopPanel(itemId,true,true);
            }

        }
        else
        {
            if(DataHolder.GetItem(itemId).itemType == ItemType.Diamond){
                MageManager.instance.OnNotificationPopup("Tính năng mua kim cương chưa mở");
            }else if(DataHolder.GetItem(itemId).itemType == ItemType.Coin){
                UIManager.instance.OnConfirmationShopPanel(itemId,false,true);
            }else
            {
                if (state == ItemState.Equiped){
                    UIManager.instance.OnConfirmationShopPanel(itemId,false,false);
                }
                else if (state == ItemState.Have)
                {
                    
                }
                else
                {
                    UIManager.instance.OnConfirmationShopPanel(itemId,false,true);
                }
            }

            

        }

        isBusy = false;
        Close();
    }


    public void Close(){
        this.GetComponent<Popup>().Close();
    }

}
