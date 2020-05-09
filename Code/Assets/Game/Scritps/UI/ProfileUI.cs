using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    public Image icon;
    public Text level;
    public Text petName;
    public Image sick;
    public Image ịnjury;
    public Image sleep;
    public Image food;
    public Image drink;
    public Image toilet;
    public Image dirty;
    public Text sickText;
    public Text injuryText;
    public Text sleepText;
    public Text foodText;
    public Text drinkText;
    public Text toiletText;
    public Text dirtyText;
    public Text priceText;
    public Text strengthText;
    public Text heartText;
    public Text sellText;
    int price = 10;
    int sellPrice = 0;
    PlayerPet playerPet;
    public Button editButton;
    public InputField input;

    public Image[] icons;

    void Awake()
    {
        petName.gameObject.SetActive(true);
        editButton.gameObject.SetActive(true);
        input.gameObject.SetActive(false);
    }

    public void Load(PlayerPet p) {
        playerPet = p;
        level.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + playerPet.level.ToString();
        /*
       if (GameManager.instance.GetPetObject(playerPet.iD) != null)
       {
           data = GameManager.instance.GetPetObject(playerPet.iD).data;
           sick.fillAmount = (data.MaxHealth - data.Health) / data.MaxHealth;
           ịnjury.fillAmount = data.Damage / data.MaxDamage;
           sleep.fillAmount = (data.MaxSleep - data.Sleep) / data.MaxSleep;
           food.fillAmount = (data.MaxFood - data.Food) / data.MaxFood;
           drink.fillAmount = (data.MaxWater - data.Water) / data.MaxWater;
           dirty.fillAmount = data.Dirty / data.MaxDirty;
           toilet.fillAmount = (Mathf.Max(data.Shit / data.MaxShit, data.Pee / data.MaxPee));

           sickText.text = data.MaxHealth.ToString();
           injuryText.text = data.MaxDamage.ToString();
           sleepText.text = data.MaxSleep.ToString();
           foodText.text = data.MaxFood.ToString();
           drinkText.text = data.MaxWater.ToString();
           toiletText.text = data.MaxPee.ToString();
           dirtyText.text = data.MaxDirty.ToString();
       }*/
        Pet pet = DataHolder.GetPet(playerPet.iD);
        Accessory a = DataHolder.GetAccessory(playerPet.iD,playerPet.accessoryId);
        petName.text = playerPet.petName;
        string url = a.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price = playerPet.level * playerPet.level * 2;
        priceText.text = price.ToString();
        //strengthText.text = (pet.maxHealth + playerPet.level * pet.levelRate).ToString();
        heartText.text = "+" + (pet.RateHappy + playerPet.level/10).ToString();

        sellPrice = pet.buyPrice / 2;
        sellText.text = sellPrice.ToString();
    }

    void Update() {
        petName.text = playerPet.petName;
        level.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + playerPet.level.ToString();
        /*
        if (GameManager.instance.GetPetObject(playerPet.iD) != null)
        {
            data = GameManager.instance.GetPetObject(playerPet.iD).data;
            sick.fillAmount = (data.MaxHealth - data.Health) / data.MaxHealth;
            ịnjury.fillAmount = data.Damage / data.MaxDamage;
            sleep.fillAmount = (data.MaxSleep - data.Sleep) / data.MaxSleep;
            food.fillAmount = (data.MaxFood - data.Food) / data.MaxFood;
            drink.fillAmount = (data.MaxWater - data.Water) / data.MaxWater;
            dirty.fillAmount = data.Dirty / data.MaxDirty;
            toilet.fillAmount = (Mathf.Max(data.Shit / data.MaxShit, data.Pee / data.MaxPee));

            sickText.text = data.MaxHealth.ToString();
            injuryText.text = data.MaxDamage.ToString();
            sleepText.text = data.MaxSleep.ToString();
            foodText.text = data.MaxFood.ToString();
            drinkText.text = data.MaxWater.ToString();
            toiletText.text = data.MaxPee.ToString();
            dirtyText.text = data.MaxDirty.ToString();
        }*/
        Pet pet = DataHolder.GetPet(playerPet.iD);
        price = playerPet.level * playerPet.level * 2;
        priceText.text = price.ToString();
        //strengthText.text = (pet.maxHealth + playerPet.level * pet.levelRate).ToString();
        heartText.text = "+" + (pet.RateHappy + playerPet.level / 10).ToString();

        int n = playerPet.level / 5;
        for (int i = 0; i < icons.Length; i++)
        {
            if (i < n)
            {
                icons[i].material = null;
                icons[i].transform.parent.GetComponent<Image>().enabled = false;
            }
        }
    }

    public void Upgrade()
    {
        if (price > GameManager.instance.GetHappy())
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
        } else
        {
            GameManager.instance.LevelUp(playerPet.realId);
            if (UIManager.instance != null && UIManager.instance.shopPanel != null)
            {
                UIManager.instance.shopPanel.ReLoad();
            }
        }
    }

    public void OnEdit()
    {
        editButton.gameObject.SetActive(false);
        input.gameObject.SetActive(true);
        input.text = playerPet.petName;
        petName.gameObject.SetActive(false);
        input.Select();
    }

    public void EndEdit()
    {
        playerPet.petName = input.text;
        petName.gameObject.SetActive(true);
        editButton.gameObject.SetActive(true);
        input.gameObject.SetActive(false);
        if (GameManager.instance.GetPetObject(playerPet.realId) != null)
            GameManager.instance.GetPetObject(playerPet.realId).SetName();
    }

    public void Sell(){
        Debug.Log(playerPet.realId);
        UIManager.instance.OnConfirmationShopPanel(playerPet.realId, true, false);
    }

    public void Accessory()
    {
        UIManager.instance.OnAccessoryPanel(playerPet.realId);        
    }

    public void OnSkill(int skillId)
    {
        if(skillId == 0)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(149).GetName(MageManager.instance.GetLanguage()));
        }else if(skillId == 1)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(153).GetName(MageManager.instance.GetLanguage()));
        }
        else if (skillId == 2)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(150).GetName(MageManager.instance.GetLanguage()));
        }
        else if (skillId == 3)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(151).GetName(MageManager.instance.GetLanguage()));
        }
        else if (skillId == 4)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(152).GetName(MageManager.instance.GetLanguage()));
        }
        else if (skillId == 5)
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(154).GetName(MageManager.instance.GetLanguage()));
        }
    }
}
