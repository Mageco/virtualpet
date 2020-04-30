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
    int price = 10;
    Pet data;
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

    public void Load(PlayerPet p){
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
       petName.text = playerPet.petName;
       string url = pet.iconUrl.Replace("Assets/Game/Resources/", "");
       url = url.Replace(".png", "");
       icon.sprite = Resources.Load<Sprite>(url) as Sprite;
       price = playerPet.level * playerPet.level * 2;
       priceText.text = price.ToString();
       strengthText.text = (pet.maxHealth + playerPet.level * pet.levelRate).ToString();
       heartText.text = "+" + (pet.RateHappy + playerPet.level / 5).ToString();


   }

   void Update(){
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
        strengthText.text = (pet.maxHealth + playerPet.level * pet.levelRate).ToString();
        heartText.text = "+" + (pet.RateHappy + playerPet.level / 5).ToString();

        int n = playerPet.level / 5;
        for(int i = 0; i < icons.Length; i++)
        {
            if(i < n)
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
        }else
        {
            GameManager.instance.LevelUp(playerPet.iD);
            if(UIManager.instance != null && UIManager.instance.shopPanel != null)
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
}
