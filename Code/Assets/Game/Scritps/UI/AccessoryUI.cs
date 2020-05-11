﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryUI : MonoBehaviour
{
    public int realId = 0;
    public int itemId = 0;
    public Image icon;
    public Text price;
    public Button buyButton;
    public Button equipButton;
    public Text levelText;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;
    public GameObject happyIcon;
    public GameObject[] tags;
    bool isBusy = false;
    bool isLevelRequire = false;

    ItemState state = ItemState.OnShop;

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    public void Load(Accessory d,PlayerPet p)
    {
        if (d.accessoryId == p.accessoryId)
            state = ItemState.Equiped;
        else if (p.accessories.Contains(d.accessoryId))
            state = ItemState.Have;
        else
            state = ItemState.OnShop;

        realId = p.realId;
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;

        if (d.levelRequire > p.level && state == ItemState.OnShop)
            isLevelRequire = true;

        OffAllIcon();

        if (d.itemTag == ItemTag.Hot)
        {
            tags[0].transform.parent.gameObject.SetActive(true);
            tags[0].SetActive(true);
        }
        else if (d.itemTag == ItemTag.Sale)
        {
            tags[0].transform.parent.gameObject.SetActive(true);
            tags[1].SetActive(true);
        }
        else if (d.itemTag == ItemTag.New)
        {
            tags[0].transform.parent.gameObject.SetActive(true);
            tags[2].SetActive(true);
        }

        if (isLevelRequire)
        {
            price.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
            levelText.text = DataHolder.Dialog(145).GetName(MageManager.instance.GetLanguage()) + " " + DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + d.levelRequire.ToString();
        }
        else
        {

            if (state == ItemState.OnShop)
            {
                price.gameObject.SetActive(true);
                price.text = d.buyPrice.ToString();
                buyButton.gameObject.SetActive(true);
                buyButton.interactable = true;

                if (d.priceType == PriceType.Coin)
                {
                    coinIcon.SetActive(true);
                    if (GameManager.instance.GetCoin() < (DataHolder.GetAccessory(itemId).buyPrice))
                    {
                        buyButton.interactable = false;
                    }
                }
                else if (d.priceType == PriceType.Diamond)
                {
                    diamonIcon.SetActive(true);
                    if (GameManager.instance.GetDiamond() < (DataHolder.GetAccessory(itemId).buyPrice))
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
                    if (state == ItemState.OnShop && GameManager.instance.GetHappy() < (DataHolder.GetAccessory(itemId).buyPrice))
                    {
                        buyButton.interactable = false;
                    }
                }
            }
            else if (state == ItemState.Have)
            {
                equipButton.gameObject.SetActive(true);
                equipButton.interactable = true;
            }else if(state == ItemState.Equiped)
            {
                equipButton.gameObject.SetActive(true);
                equipButton.interactable = false;
            }
            
        }
    }

    void OffAllIcon()
    {
        levelText.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        price.gameObject.SetActive(false);
        coinIcon.SetActive(false);
        diamonIcon.SetActive(false);
        moneyIcon.SetActive(false);
        happyIcon.SetActive(false);
        tags[0].transform.parent.gameObject.SetActive(false);
        for(int i = 0; i < tags.Length; i++)
        {
            tags[i].SetActive(false);
        }
    }

    public void OnBuy()
    {
        if (isBusy)
            return;

        MageManager.instance.PlaySound("BubbleButton", false);

        if(state == ItemState.OnShop)
        {
            GameManager.instance.BuyAccessory(itemId, realId);
            
            if (UIManager.instance.profilePanel != null)
            {
                UIManager.instance.profilePanel.Load();
            }
            if (UIManager.instance.accessoryPanel != null)
            {
                UIManager.instance.accessoryPanel.Close();
            }
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(76).GetName(MageManager.instance.GetLanguage()));
        }else if(state == ItemState.Have)
        {
            GameManager.instance.EquipAccessory(itemId, realId);
            if (UIManager.instance.profilePanel != null)
            {
                UIManager.instance.profilePanel.Load();
            }
            if (UIManager.instance.accessoryPanel != null)
            {
                UIManager.instance.accessoryPanel.Close();
            }
        }

    }


}
