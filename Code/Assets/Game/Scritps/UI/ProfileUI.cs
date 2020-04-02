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

    public void Load(int id){
        data = GameManager.instance.GetPet(id);
        level.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + data.level.ToString();
        petName.text = DataHolder.GetPet(id).GetName(0);
        sick.fillAmount = (data.MaxHealth - data.Health)/data.MaxHealth;
        ịnjury.fillAmount = data.Damage/data.MaxDamage;
        sleep.fillAmount = (data.MaxSleep - data.Sleep)/data.MaxSleep;
        food.fillAmount = (data.MaxFood - data.Food)/data.MaxFood;
        drink.fillAmount = (data.MaxWater - data.Water)/data.MaxWater;
        dirty.fillAmount = data.Dirty/data.MaxDirty;
        toilet.fillAmount = (Mathf.Max(data.Shit/data.MaxShit,data.Pee/data.MaxPee));

        sickText.text = data.MaxHealth.ToString();
        injuryText.text = data.MaxDamage.ToString();
        sleepText.text = data.MaxSleep.ToString();
        foodText.text = data.MaxFood.ToString();
        drinkText.text = data.MaxWater.ToString();
        toiletText.text = data.MaxPee.ToString();
        dirtyText.text = data.MaxDirty.ToString();

        string url = DataHolder.GetPet(id).iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price = data.level * data.level * 10;
        priceText.text = price.ToString();
        strengthText.text = data.MaxHealth.ToString();
        heartText.text = "+" + (data.rateHappy + data.level / 5).ToString();
    }

    void Update(){
        level.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + data.level.ToString();
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
        price = data.level * data.level * 10;
        priceText.text = price.ToString();
        strengthText.text = data.MaxHealth.ToString();
        heartText.text = "+" + (data.rateHappy + data.level/5).ToString();
    }

    public void Upgrade()
    {
        if (price > GameManager.instance.GetHappy())
        {
            MageManager.instance.OnNotificationPopup(DataHolder.Dialog(8).GetDescription(MageManager.instance.GetLanguage()));
        }else
        {
            GameManager.instance.LevelUp(data.iD);
            if(UIManager.instance != null && UIManager.instance.shopPanel != null)
            {
                UIManager.instance.shopPanel.ReLoad();
            }
        }
    }
}
