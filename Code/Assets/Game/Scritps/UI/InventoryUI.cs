using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public int realId = 0;
    public int itemId = 0;
    public Image icon;
    public Image iconType;
    public Text price;
    public Button sellButton;
    public Button equipButton;
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
    public Image plusCoinIcon;
    public Image toyIcon;

    public Text happyText;
    public Text sickText;
    public Text injuredText;
    public Text foodText;
    public Text drinkText;
    public Text cleanText;
    public Text bathText;
    public Text bedText;
    public Text strengthText;
    public Text plusCoinText;
    public Text toyText;
    public Text number;
    bool isBusy = false;

    void Awake()
    {
    }

    // Start is called before the first frame update
    public void Load(PlayerItem d)
    {
        itemId = d.itemId;
        realId = d.realId;
        Item item = DataHolder.GetItem(itemId);
        //Debug.Log(item.iconUrl);
        string url = item.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        iconType.sprite = Resources.Load<Sprite>("Icons/ItemType/" + item.itemType.ToString());

        OffAllIcon();

        if(d.state == ItemState.Equiped)
        {
            price.gameObject.SetActive(true);
            price.text = (item.buyPrice / 2).ToString();
            sellButton.gameObject.SetActive(true);
            sellButton.interactable = true;
        }
        else
        {
            if(d.itemType == ItemType.QuestItem)
            {
                price.gameObject.SetActive(true);
                price.text = (item.buyPrice / 2).ToString();
                sellButton.gameObject.SetActive(true);
                sellButton.interactable = true;
                number.gameObject.SetActive(true);
                number.text = "x " + d.number.ToString();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
                equipButton.interactable = true;
            }

        }

        if ((item.itemType == ItemType.Room && GameManager.instance.GetItemNumber(ItemType.Room) == 1) || (item.itemType == ItemType.Gate && GameManager.instance.GetItemNumber(ItemType.Gate) == 1) || (item.itemType == ItemType.Board && GameManager.instance.GetItemNumber(ItemType.Board) == 1))
            sellButton.interactable = false;

        if (item.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
        }
        else if (item.priceType == PriceType.Diamond)
        {
            diamonIcon.SetActive(true);
        }
        else if (item.priceType == PriceType.Happy)
        {
            happyIcon.SetActive(true);
        }


        statPanel.SetActive(true);

        if (item.itemType == ItemType.Food)
        {
            if (item.value > 0)
            {
                foodIcon.gameObject.SetActive(true);
                foodText.text = item.value.ToString("F0");
            }
        }
        else if (item.itemType == ItemType.Drink)
        {
            if (item.value > 0)
            {
                drinkIcon.gameObject.SetActive(true);
                drinkText.text = item.value.ToString("F0");
            }
        }
        else if (item.itemType == ItemType.Bath)
        {
            if (item.value > 0)
            {

                bathIcon.gameObject.SetActive(true);
                bathText.text = item.value.ToString("F0");
            }
        }
        else if (item.itemType == ItemType.Clean)
        {
            if (item.value > 0)
            {
                cleanIcon.gameObject.SetActive(true);
                cleanText.text = item.value.ToString("F0");
            }
        }

        else if (item.itemType == ItemType.Bed)
        {
            if (item.value > 0)
            {
                bedIcon.gameObject.SetActive(true);
                bedText.text = item.value.ToString("F0");
            }
        }
        else if (item.itemType == ItemType.MedicineBox)
        {
            if (item.value > 0)
            {
                sickIcon.gameObject.SetActive(true);
                sickText.text = item.value.ToString("F0");
            }
        }

        else if (item.itemType == ItemType.Toy)
        {
            if (item.value > 0)
            {
                toyIcon.gameObject.SetActive(true);
                toyText.text = "+" + item.value.ToString("F0");
            }
        }
        else if (item.itemType == ItemType.Toilet)
        {
            if (item.value > 0)
            {
                cleanIcon.gameObject.SetActive(true);
                cleanText.text = item.value.ToString("F0");
            }
        }
    }

    void OffAllIcon()
    {
        sellButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        price.gameObject.SetActive(false);
        coinIcon.SetActive(false);
        diamonIcon.SetActive(false);
        moneyIcon.SetActive(false);
        happyIcon.SetActive(false);
        statPanel.SetActive(false);
        happyIcon.gameObject.SetActive(false);
        sickIcon.gameObject.SetActive(false);
        injuredIcon.gameObject.SetActive(false);
        foodIcon.gameObject.SetActive(false);
        drinkIcon.gameObject.SetActive(false);
        bathIcon.gameObject.SetActive(false);
        cleanIcon.gameObject.SetActive(false);
        strengthIcon.gameObject.SetActive(false);
        plusCoinIcon.gameObject.SetActive(false);
        toyIcon.gameObject.SetActive(false);
        number.gameObject.SetActive(false);
    }

    public void OnBuy()
    {
        if (isBusy)
            return;

        MageManager.instance.PlaySound("BubbleButton", false);
        UIManager.instance.OnConfirmationShopPanel(realId, false, false);
    }

    public void OnEquip()
    {
        if (isBusy)
            return;

        MageManager.instance.PlaySound("BubbleButton", false);
        GameManager.instance.EquipItem(realId);
        if (UIManager.instance.inventoryPanel != null)
            UIManager.instance.inventoryPanel.Load();
    }
}
