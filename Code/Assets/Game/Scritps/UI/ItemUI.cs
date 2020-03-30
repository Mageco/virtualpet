﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public int itemId = 0;
    public Image icon;
    public Image iconType;
    public Text price;
    public Button buyButton;
    public Button unEquipButton;
    public Button equipButton;
    public Text levelText;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public GameObject happyIcon;
    public GameObject statPanel;
    public Image heartIcon;
    public Image sickIcon;
    public Image injuredIcon;
    public Image foodIcon;
    public Image drinkIcon;
    public Image cleanIcon;
    public Image bathIcon;
    public Image bedIcon;
    public Image strengthIcon;

    public Text happyText;
    public Text sickText;
    public Text injuredText;
    public Text foodText;
    public Text drinkText;
    public Text cleanText;
    public Text bathText;
    public Text bedText;
    public Text strengthText;

    public Text petLevelText;
    public Text buttonText;

    Animator animator;
    bool isBusy = false;
    bool isCharacter = false;
    bool isLevelRequire = false;
    public Material greyMaterial;

    

    ItemState state = ItemState.OnShop;

    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public void Load(Item d)
    {
        itemId = d.iD;
        //Debug.Log(d.iconUrl);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        iconType.sprite = Resources.Load<Sprite>("Icons/ItemType/" + d.itemType.ToString());

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

        if (d.levelRequire > GameManager.instance.myPlayer.level && state == ItemState.OnShop)
            isLevelRequire = true;




        OffAllIcon();

        

        if (isLevelRequire)
        {
            icon.material = greyMaterial;
            price.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
            levelText.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + d.levelRequire.ToString(); 
        }
        else
        {
            if (state == ItemState.OnShop)
            {
                price.gameObject.SetActive(true);
                price.text = d.buyPrice.ToString();
                buyButton.gameObject.SetActive(true);
            }
            else if (state == ItemState.Equiped)
            {
                unEquipButton.gameObject.SetActive(true);
                if(d.itemType == ItemType.Room || d.itemType == ItemType.Food || d.itemType == ItemType.Drink)
                    unEquipButton.interactable = false;
                else
                    unEquipButton.interactable = true;
                this.GetComponent<Image>().color = new Color(251f / 256, 134f / 256, 58f / 256);
            }
            else if (state == ItemState.Have && d.itemType != ItemType.Chest)
            {
                equipButton.gameObject.SetActive(true);
            }

            if (state == ItemState.OnShop)
            {
                if (d.priceType == PriceType.Coin)
                {
                    coinIcon.SetActive(true);
                    if (state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetItem(itemId).buyPrice))
                    {
                        buyButton.interactable = false;
                    }
                }
                else if (d.priceType == PriceType.Diamond)
                {
                    diamonIcon.SetActive(true);
                    if (state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetItem(itemId).buyPrice))
                    {
                        buyButton.interactable = false;
                    }
                }
                else if (d.priceType == PriceType.Money)
                {
                    moneyIcon.SetActive(true);
                    moneyIcon.GetComponent<Text>().text = DataHolder.Dialog(64).GetName(MageManager.instance.GetLanguage());
                    price.text = (d.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00");
                }
                else if (d.priceType == PriceType.Happy)
                {
                    happyIcon.SetActive(true);
                    if (state == ItemState.OnShop && GameManager.instance.GetHappy() < (DataHolder.GetItem(itemId).buyPrice))
                    {
                        buyButton.interactable = false;
                    }
                }
            }
        }

        


        
        

        statPanel.SetActive(true);

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

        else if (d.itemType == ItemType.Bed)
        {
            if (d.value > 0)
            {
                bedIcon.gameObject.SetActive(true);
                bedText.text = d.value.ToString("F0");
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
        /*
        else if (d.itemType == ItemType.Toy)
        {
            if (d.value > 0)
            {
                heartIcon.gameObject.SetActive(true);
                happyText.text = d.value.ToString("F0");
            }
        }*/

        //SuperSale
        if (!isLevelRequire)
        {
            if (d.iD == 128)
            {
                if (GameManager.instance.GetItemNumber(128) > 2)
                {
                    icon.material = greyMaterial;
                    price.gameObject.SetActive(false);
                    moneyIcon.SetActive(false);
                    buyButton.interactable = false;
                    isLevelRequire = true;
                    levelText.gameObject.SetActive(true);
                    levelText.text = DataHolder.Dialog(115).GetName(MageManager.instance.GetLanguage()) + " " + GameManager.instance.GetItemNumber(128).ToString() + "/3"; ;

                }
                else
                {
                    price.gameObject.SetActive(true);
                    //buyButton.gameObject.SetActive(true);
                    //buyButton.interactable = true;
                    moneyIcon.SetActive(true);
                    moneyIcon.GetComponent<Text>().text = DataHolder.Dialog(64).GetName(MageManager.instance.GetLanguage());
                    price.text = (d.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00") ;
                    levelText.gameObject.SetActive(true);
                    levelText.text = DataHolder.Dialog(115).GetName(MageManager.instance.GetLanguage()) + " " + GameManager.instance.GetItemNumber(128).ToString() + "/3"; ;
                }
            }
            else if (d.iD == 129)
            {
                if (GameManager.instance.GetItemNumber(129) > 0)
                {
                    icon.material = greyMaterial;
                    price.gameObject.SetActive(false);
                    moneyIcon.SetActive(false);
                    buyButton.interactable = false;
                    isLevelRequire = true;
                    levelText.gameObject.SetActive(true);
                    levelText.text = DataHolder.Dialog(115).GetName(MageManager.instance.GetLanguage()) + " " + GameManager.instance.GetItemNumber(129).ToString() + "/1";
                }
                else
                {
                    price.gameObject.SetActive(true);
                    //buyButton.gameObject.SetActive(true);
                    //buyButton.interactable = true;
                    moneyIcon.SetActive(true);
                    moneyIcon.GetComponent<Text>().text = DataHolder.Dialog(64).GetName(MageManager.instance.GetLanguage());
                    price.text = (d.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00");
                    levelText.gameObject.SetActive(true);
                    levelText.text = DataHolder.Dialog(115).GetName(MageManager.instance.GetLanguage()) + " " + GameManager.instance.GetItemNumber(129).ToString() + "/1";
                }
            }
            if (d.iD == 130)
            {
                if (GameManager.instance.GetItemNumber(130) > 0)
                {
                    icon.material = greyMaterial;

                    price.gameObject.SetActive(false);
                    moneyIcon.SetActive(false);
                    buyButton.interactable = false;
                    isLevelRequire = true;
                    levelText.gameObject.SetActive(true);
                    levelText.text = DataHolder.Dialog(115).GetName(MageManager.instance.GetLanguage()) + " " + GameManager.instance.GetItemNumber(130).ToString() + "/1";
                }
                else
                {
                    price.gameObject.SetActive(true);
                    //buyButton.gameObject.SetActive(true);
                    //buyButton.interactable = true;
                    moneyIcon.SetActive(true);
                    moneyIcon.GetComponent<Text>().text = DataHolder.Dialog(64).GetName(MageManager.instance.GetLanguage());
                    price.text = (d.buyPrice * (float.Parse(DataHolder.Dialog(64).GetDescription(MageManager.instance.GetLanguage())))).ToString(".00");
                    levelText.gameObject.SetActive(true);
                    levelText.text = DataHolder.Dialog(115).GetName(MageManager.instance.GetLanguage()) + " " + GameManager.instance.GetItemNumber(130).ToString() + "/1";

                }
            }
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
        iconType.sprite = Resources.Load<Sprite>("Icons/ItemType/Pet_" + d.rareType.ToString());
        Debug.Log("Icons/ItemType/Pet_" + d.rareType.ToString());
        state = ItemState.OnShop;
        foreach(Pet p in GameManager.instance.myPlayer.pets)
        {
            if(p.iD == d.iD)
            {
                state = GameManager.instance.GetPet(d.iD).itemState;
            }
        }

        OffAllIcon();

        statPanel.SetActive(true);
        if (d.rareType == RareType.Rare)
        {
            heartIcon.gameObject.SetActive(true);
            happyText.text = "+2";
        }
        else if (d.rareType == RareType.Epic)
        {
            heartIcon.gameObject.SetActive(true);
            happyText.text = "+4";
        }
        else if (d.rareType == RareType.Legend)
        {
            heartIcon.gameObject.SetActive(true);
            happyText.text = "+9";
        }

        if (state == ItemState.OnShop)
        {
            price.gameObject.SetActive(true);
            price.text = d.buyPrice.ToString();
            buyButton.gameObject.SetActive(true);
        }
        else if (state == ItemState.Equiped)
        {
            unEquipButton.gameObject.SetActive(true);
            this.GetComponent<Image>().color = new Color(251f / 256, 134f / 256, 58f / 256);
        }
        else if (state == ItemState.Have)
        {
            equipButton.gameObject.SetActive(true);
        }



        if (state == ItemState.OnShop)
        {
            if (d.priceType == PriceType.Coin)
            {
                coinIcon.SetActive(true);
                if (state == ItemState.OnShop && GameManager.instance.GetCoin() < (DataHolder.GetPet(itemId).buyPrice))
                {
                    buyButton.interactable = false;
                }
            }
            else if (d.priceType == PriceType.Diamond)
            {
                diamonIcon.SetActive(true);
                if (state == ItemState.OnShop && GameManager.instance.GetDiamond() < (DataHolder.GetPet(itemId).buyPrice))
                {
                    buyButton.interactable = false;
                }
            }
            else if (d.priceType == PriceType.Money)
            {
                moneyIcon.SetActive(true);
            }
            else if (d.priceType == PriceType.Happy)
            {
                happyIcon.SetActive(true);
                if (state == ItemState.OnShop && GameManager.instance.GetHappy() < (DataHolder.GetPet(itemId).buyPrice))
                {
                    buyButton.interactable = false;
                }
            }
        }
    }


    void OffAllIcon()
    {
        levelText.gameObject.SetActive(false);
        unEquipButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        price.gameObject.SetActive(false);
        coinIcon.SetActive(false);
        diamonIcon.SetActive(false);
        moneyIcon.SetActive(false);
        happyIcon.SetActive(false);
        petLevelText.gameObject.SetActive(false);
        

        statPanel.SetActive(false);
        happyIcon.gameObject.SetActive(false);
        sickIcon.gameObject.SetActive(false);
        injuredIcon.gameObject.SetActive(false);
        foodIcon.gameObject.SetActive(false);
        drinkIcon.gameObject.SetActive(false);
        bathIcon.gameObject.SetActive(false);
        cleanIcon.gameObject.SetActive(false);
        strengthIcon.gameObject.SetActive(false);
    }

    public void OnBuy()
    {
        if (isBusy)
            return;

        MageManager.instance.PlaySound("BubbleButton", false);
        BuyCoroutine();
    }

    void BuyCoroutine()
    {
        isBusy = true;

        if (isCharacter)
        {
            if (state == ItemState.Equiped)
                UIManager.instance.UnEquipPet(itemId);
            else if (state == ItemState.Have)
            {
                UIManager.instance.UsePet(itemId);
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
                
                if (itemId == 3)
                    PurchaseManager.instance.BuyConsumable(0);
                else if(itemId == 19)
                    PurchaseManager.instance.BuyConsumable(1);
                else if (itemId == 20)
                    PurchaseManager.instance.BuyConsumable(2);
                else if (itemId == 21)
                    PurchaseManager.instance.BuyConsumable(3);
                    
            }
            else if (DataHolder.GetItem(itemId).itemType == ItemType.Coin)
            {
                UIManager.instance.OnConfirmationShopPanel(itemId, false, true);
            }
            else if (DataHolder.GetItem(itemId).itemType == ItemType.Chest)
            {
                if (itemId == 128)
                {
                    PurchaseManager.instance.BuyConsumable(4);
                }
                else if (itemId == 129)
                {
                    PurchaseManager.instance.BuyConsumable(5);
                }
                else if (itemId == 130)
                {
                    PurchaseManager.instance.BuyConsumable(6);
                }
            }
            else
            {
                if (state == ItemState.Equiped)
                {
                    UIManager.instance.UnEquipItem(itemId);
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
    }

    public void OnItemInfo()
    {
        if(isCharacter && (state == ItemState.Have || state == ItemState.Equiped))
        {
            //UIManager.instance.OnProfilePanel(itemId);
        }

        if(!isCharacter && !isLevelRequire)
        {
            if(itemId == 128)
            {
                UIManager.instance.OnChestSalePanel(RareType.Common);
            }else if(itemId == 129)
            {
                UIManager.instance.OnChestSalePanel(RareType.Rare);
            }
            else if (itemId == 130)
            {
                UIManager.instance.OnChestSalePanel(RareType.Epic);
            }
        }
        //MageManager.instance.PlaySoundName("BubbleButton", false);
        //UIManager.instance.OnItemInfoPanel(itemId, isCharacter);
    }

}
