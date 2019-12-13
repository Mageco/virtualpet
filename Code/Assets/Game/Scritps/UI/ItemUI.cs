using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    int itemId = 0;
    public Image icon;
    public Image iconType;
    public Text price;
    public Button buyButton;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;

    public Text buttonText;
    Animator animator;
    bool isBusy = false;
    bool isCommingSoon = false;
    bool isCharacter = false;

    ItemState state = ItemState.OnShop;

    void Awake(){
        animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public void Load(Item d)
    {

        itemId = d.iD;
        //Debug.Log(d.iconUrl);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");

        if(!d.isAvailable){
            isCommingSoon = true;
        }

        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        iconType.sprite = Resources.Load<Sprite>("Icons/ItemType/"+d.itemType.ToString());

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
                price.text = (d.buyPrice).ToString();
            }
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


    public void Load(Pet d)
    {
        isCharacter = true;
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();

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
                price.text = d.buyPrice.ToString();
            }
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

    // Update is called once per frame
    void UpdateState()
    {

    }

    public void OnBuy()
    {
        if (isBusy)
            return;

        if(isCommingSoon){
            MageManager.instance.OnNotificationPopup("Item will be available soon!");
            return;
        }

        StartCoroutine(BuyCoroutine());
    }

    IEnumerator BuyCoroutine()
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
                if(GameManager.instance.GetDiamond() > (DataHolder.GetItem(itemId).buyPrice)){
                    animator.Play("Use", 0);
                    yield return new WaitForSeconds(0.5f);
                    GameManager.instance.AddDiamond(-DataHolder.GetItem(itemId).buyPrice);
                    GameManager.instance.AddCoin(DataHolder.GetItem(itemId).sellPrice);
                    animator.Play("Idle", 0);
                    MageManager.instance.OnNotificationPopup("bạn đã mua thành công");
                }else{
                    MageManager.instance.OnNotificationPopup ("You have not enough Coin");
                }
            }else
            {
                if (state == ItemState.Equiped)
                    if(DataHolder.GetItem(itemId).itemType == ItemType.Room && GameManager.instance.GetBuyItems(ItemType.Room).Count == 1){
                        MageManager.instance.OnNotificationPopup ("You can not sell this room");
                    }else
                        UIManager.instance.OnConfirmationShopPanel(itemId,false,false);
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
    }

    public void OnItemInfo(){
        UIManager.instance.OnItemInfoPanel(itemId,isCharacter);
    }

}
