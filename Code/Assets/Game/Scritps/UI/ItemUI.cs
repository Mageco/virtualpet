using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public int itemId = 0;
    public int orderId = 0;
    public Image icon;
    public Image iconType;
    public Text price;
    public Button buyButton;
    public Button sellButton;
    public Text usedText;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public GameObject happyIcon;

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
        usedText.gameObject.SetActive(false);

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
            buyButton.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(false);
        }
        else{
            if (state == ItemState.OnShop)
            {
               buyButton.interactable = true;
               price.text = d.buyPrice.ToString();
               buyButton.gameObject.SetActive(true);
               sellButton.gameObject.SetActive(false);
            }else if (state == ItemState.Equiped)
            {
                sellButton.gameObject.SetActive(false);
                sellButton.interactable = false;
                buyButton.gameObject.SetActive(false);
                usedText.gameObject.SetActive(true);
                price.gameObject.SetActive(false);
                this.GetComponent<Image>().color = new Color(251f / 256, 134f / 256, 58f / 256);
                
            }else if(state == ItemState.Have)
            {
                sellButton.interactable = true;
                price.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(true);
            }
        }
        

        if(state == ItemState.OnShop)
        {
            if (d.priceType == PriceType.Coin)
            {
                coinIcon.SetActive(true);
                diamonIcon.SetActive(false);
                moneyIcon.SetActive(false);
                happyIcon.SetActive(false);
                if (state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetItem(itemId).buyPrice))
                {
                    buyButton.interactable = false;
                    sellButton.interactable = false;
                }
            }
            else if (d.priceType == PriceType.Diamond)
            {
                coinIcon.SetActive(false);
                diamonIcon.SetActive(true);
                moneyIcon.SetActive(false);
                happyIcon.SetActive(false);
                if (state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetItem(itemId).buyPrice))
                {
                    buyButton.interactable = false;
                    sellButton.interactable = false;
                }
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
                if (state == ItemState.OnShop && GameManager.instance.GetHappy() < (DataHolder.GetItem(itemId).buyPrice))
                {
                    buyButton.interactable = false;
                    sellButton.interactable = false;
                }
            }
        }
        else
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        

    }


    public void Load(Pet d, int colorId)
    {
        isCharacter = true;
        itemId = d.iD;
        orderId = colorId;
        string url = d.petColors[colorId].iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.petColors[colorId].buyPrice.ToString();
        iconType.sprite = Resources.Load<Sprite>("Icons/ItemType/Pet");
        state = GameManager.instance.GetPet(d.iD).petColors[colorId].itemState;

        usedText.gameObject.SetActive(false);
        if (state == ItemState.OnShop)
        {
            buyButton.interactable = true;
            price.text = d.buyPrice.ToString();
            buyButton.gameObject.SetActive(true);
            sellButton.gameObject.SetActive(false);
        }
        else if (state == ItemState.Equiped)
        {
            sellButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            usedText.gameObject.SetActive(true);
            price.gameObject.SetActive(false);
            this.GetComponent<Image>().color = new Color(251f / 256, 134f / 256, 58f / 256);

        }
        else if (state == ItemState.Have)
        {
            sellButton.interactable = true;
            price.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(true);
        }
        


        if (state == ItemState.OnShop)
        {
            if (d.petColors[colorId].priceType == PriceType.Coin)
            {
                coinIcon.SetActive(true);
                diamonIcon.SetActive(false);
                moneyIcon.SetActive(false);
                happyIcon.SetActive(false);
                if (state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetPet(itemId).petColors[colorId].buyPrice))
                {
                    buyButton.interactable = false;
                    sellButton.interactable = false;
                }
            }
            else if (d.petColors[colorId].priceType == PriceType.Diamond)
            {
                coinIcon.SetActive(false);
                diamonIcon.SetActive(true);
                moneyIcon.SetActive(false);
                happyIcon.SetActive(false);
                if (state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetPet(itemId).petColors[colorId].buyPrice))
                {
                    buyButton.interactable = false;
                    sellButton.interactable = false;
                }
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
                if (state == ItemState.OnShop && GameManager.instance.GetHappy() < (DataHolder.GetPet(itemId).petColors[colorId].buyPrice))
                {
                    buyButton.interactable = false;
                    sellButton.interactable = false;
                }
            }
        }
        else
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            happyIcon.SetActive(false);
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

        MageManager.instance.PlaySoundName("BubbleButton",false);
        BuyCoroutine();
    }

    void BuyCoroutine()
    {
        isBusy = true;
        
        if (isCharacter)
        {
            if (state == ItemState.Equiped)
                UIManager.instance.OnConfirmationShopPanel(itemId,true,false,orderId);
            else if (state == ItemState.Have)
            {
                UIManager.instance.EquipPetColor(itemId, orderId);
            }
            else
            {
                UIManager.instance.OnConfirmationShopPanel(itemId,true,true,orderId);
            }

        }
        else
        {
            if(DataHolder.GetItem(itemId).itemType == ItemType.Diamond){
                MageManager.instance.OnNotificationPopup("You can not buy diamond now.");
            }else if(DataHolder.GetItem(itemId).itemType == ItemType.Coin){
                UIManager.instance.OnConfirmationShopPanel(itemId,false,true);
            }else
            {
                if (state == ItemState.Equiped){
                    UIManager.instance.OnConfirmationShopPanel(itemId,false,false);
                }
                else if (state == ItemState.Have)
                {
                    UIManager.instance.EquipItem(itemId);
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
        MageManager.instance.PlaySoundName("BubbleButton",false);
        UIManager.instance.OnItemInfoPanel(itemId,isCharacter,orderId);
    }

}
