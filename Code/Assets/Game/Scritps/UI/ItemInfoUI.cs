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
    public GameObject heartIcon;
    public Text itemName;
    public Text description;

    public Button buyButton;
    public Button sellButton;
    public Text buttonText;

    bool isBusy = false;
    bool isCommingSoon = false;
    bool isCharacter = false;
    ItemState state = ItemState.OnShop;

    public Image happyIcon;
    public Image sickIcon;
    public Image injuredIcon;
    public Image foodIcon;
    public Image drinkIcon;
    public Image cleanIcon;
    public Image bathIcon;

    public Text happyText;
    public Text sickText;
    public Text injuredText;
    public Text foodText;
    public Text drinkText;
    public Text cleanText;
    public Text bathText;

    void Awake()
    {

    }

    // Start is called before the first frame update
    public void Load(Item d)
    {
        OffAllIcon();
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();
        itemName.text = DataHolder.GetItem(itemId).GetName(MageManager.instance.GetLanguage());
        description.text = DataHolder.GetItem(itemId).GetDescription(MageManager.instance.GetLanguage());

        if (!d.isAvailable)
        {
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

        if (isCommingSoon)
        {
            buyButton.interactable = false;
            sellButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(false);
            buttonText.text = "Locked";
            price.text = d.buyPrice.ToString();
        }
        else
        {
            if (state == ItemState.OnShop)
            {
                buyButton.gameObject.SetActive(true);
                sellButton.gameObject.SetActive(false);
                price.text = d.buyPrice.ToString();
            }
            else if (state == ItemState.Equiped)
            {
                buyButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(false);
                price.text = (d.buyPrice / 2).ToString();

                if (DataHolder.GetItem(itemId).itemType == ItemType.Room && GameManager.instance.GetItemNumber(ItemType.Room) == 1)
                {
                    sellButton.interactable = false;
                }
            }
        }

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            heartIcon.SetActive(false);
            if (state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetItem(itemId).buyPrice))
            {
                buyButton.interactable = false;
            }
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
            heartIcon.SetActive(false);
            if (state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetItem(itemId).buyPrice))
            {
                buyButton.interactable = false;
            }
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
            heartIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
            heartIcon.SetActive(true);
        }

        if (d.itemType == ItemType.Food)
        {
            if (d.value > 0)
            {
                foodIcon.gameObject.SetActive(true);
                foodText.text = d.value.ToString("F0");
            }
        }
        else if (d.itemType == ItemType.Drink)
        {
            if (d.value > 0)
            {
                drinkIcon.gameObject.SetActive(true);
                drinkText.text = d.value.ToString("F0");
            }
        }
        else if (d.itemType == ItemType.Bath)
        {
            if (d.value > 0)
            {
                bathIcon.gameObject.SetActive(true);
                bathText.text = d.value.ToString("F0");
            }
        }
        else if (d.itemType == ItemType.Clean)
        {
            if (d.value > 0)
            {
                cleanIcon.gameObject.SetActive(true);
                cleanText.text = d.value.ToString("F0");
            }
        }
        else if (d.itemType == ItemType.MedicineBox)
        {
            if (d.value > 0)
            {
                sickIcon.gameObject.SetActive(true);
                sickText.text = d.value.ToString("F0");
            }
        }
    }

    void OffAllIcon()
    {
        happyIcon.gameObject.SetActive(false);
        sickIcon.gameObject.SetActive(false);
        injuredIcon.gameObject.SetActive(false);
        foodIcon.gameObject.SetActive(false);
        drinkIcon.gameObject.SetActive(false);
        bathIcon.gameObject.SetActive(false);
        cleanIcon.gameObject.SetActive(false);
    }


    public void Load(Pet d)
    {
        OffAllIcon();
        itemId = d.iD;
        itemName.text = DataHolder.GetPet(itemId).GetName(MageManager.instance.GetLanguage());
        description.text = DataHolder.GetPet(itemId).GetDescription(MageManager.instance.GetLanguage());
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();
        state = GameManager.instance.GetPet(d.iD).itemState;


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
            price.gameObject.SetActive(false);
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
            if (d.priceType == PriceType.Coin)
            {
                coinIcon.SetActive(true);
                diamonIcon.SetActive(false);
                moneyIcon.SetActive(false);
                heartIcon.SetActive(false);
                if (state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetPet(itemId).buyPrice))
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
                heartIcon.SetActive(false);
                if (state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetPet(itemId).buyPrice))
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
                heartIcon.SetActive(false);
            }
            else if (d.priceType == PriceType.Happy)
            {
                coinIcon.SetActive(false);
                diamonIcon.SetActive(false);
                moneyIcon.SetActive(false);
                heartIcon.SetActive(true);
                if (state == ItemState.OnShop && GameManager.instance.GetHappy() < (DataHolder.GetPet(itemId).buyPrice))
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
            heartIcon.SetActive(false);
        }
    }

    public void OnBuy()
    {
        if (isBusy)
            return;

        if (isCommingSoon)
        {
            MageManager.instance.OnNotificationPopup("Item will be available soon!");
            return;
        }
        MageManager.instance.PlaySound("BubbleButton", false);
        BuyCoroutine();
    }

    void BuyCoroutine()
    {
        isBusy = true;

        if (isCharacter)
        {
            if (state == ItemState.Equiped)
                UIManager.instance.OnConfirmationShopPanel(itemId, true, false);
            else if (state == ItemState.Have)
            {
                //animator.Play("Use", 0);
                //yield return new WaitForSeconds(0.5f);
                //UIManager.instance.OnConfirmationShopPanel(itemId,true,false);
            }
            else
            {
                UIManager.instance.OnConfirmationShopPanel(itemId, true, true);
            }

        }
        else
        {
            if (DataHolder.GetItem(itemId).itemType == ItemType.Diamond)
            {
                MageManager.instance.OnNotificationPopup("You can not buy diamon now.");
            }
            else if (DataHolder.GetItem(itemId).itemType == ItemType.Coin)
            {
                UIManager.instance.OnConfirmationShopPanel(itemId, false, true);
            }
            else
            {
                if (state == ItemState.Equiped)
                {
                    UIManager.instance.OnConfirmationShopPanel(itemId, false, false);
                }
                else if (state == ItemState.Have)
                {
                    UIManager.instance.EquipItem(itemId);
                }
                else
                {
                    UIManager.instance.OnConfirmationShopPanel(itemId, false, true);
                }
            }



        }

        isBusy = false;
        Close();
    }


    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }

}
