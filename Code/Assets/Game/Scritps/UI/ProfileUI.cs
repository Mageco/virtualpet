using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    public Text petName;
    public Image icon;
    public Text level;
    public Text exp;
    public Image expProgress;
    public Image sick;
    public Image ịnjury;
    public Image sleep;
    public Image food;
    public Image drink;
    public Image toilet;
    public Image dirty;
    Pet data;

    public void Load(int id){
        data = GameManager.instance.GetPet(id);
        petName.text = data.petName;
        level.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        expProgress.fillAmount = data.Exp/e;
        sick.fillAmount = (data.MaxHealth - data.Health)/data.MaxHealth;
        ịnjury.fillAmount = data.Damage/data.MaxDamage;
        sleep.fillAmount = (data.MaxSleep - data.Sleep)/data.MaxSleep;
        food.fillAmount = (data.MaxFood - data.Food)/data.MaxFood;
        drink.fillAmount = (data.MaxWater - data.Water)/data.MaxWater;
        dirty.fillAmount = data.Dirty/data.MaxDirty;
        toilet.fillAmount = (Mathf.Max(data.Shit/data.MaxShit,data.Pee/data.MaxPee));
        
        string url = DataHolder.GetPet(id).iconProfileUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
    }

    void Update(){
        petName.text = data.petName;
        level.text = DataHolder.Dialog(27).GetName(MageManager.instance.GetLanguage()) + " " + data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        expProgress.fillAmount = data.Exp/e;
        sick.fillAmount = (data.MaxHealth - data.Health)/data.MaxHealth;
        ịnjury.fillAmount = data.Damage / data.MaxDamage;
        sleep.fillAmount = (data.MaxSleep - data.Sleep)/data.MaxSleep;
        food.fillAmount = (data.MaxFood - data.Food)/data.MaxFood;
        drink.fillAmount = (data.MaxWater - data.Water)/data.MaxWater;
        dirty.fillAmount = data.Dirty/data.MaxDirty;
        toilet.fillAmount = (Mathf.Max(data.Shit/data.MaxShit,data.Pee/data.MaxPee));
    }
}
