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
        level.text = "Level " + data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        expProgress.fillAmount = data.Exp/e;
        sick.fillAmount = (data.maxHealth - data.Health)/data.maxHealth;
        ịnjury.fillAmount = data.Damage;
        sleep.fillAmount = (data.maxSleep - data.Sleep)/data.maxSleep;
        food.fillAmount = (data.maxFood - data.Food)/data.maxFood;
        drink.fillAmount = (data.maxWater - data.Water)/data.maxWater;
        dirty.fillAmount = data.Dirty/data.maxDirty;
        toilet.fillAmount = (Mathf.Max(data.Shit/data.maxShit,data.Pee/data.maxPee));
        
        string url = DataHolder.GetPet(id).iconProfileUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
    }

    void Update(){
        petName.text = data.petName;
        level.text = "Level " + data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        expProgress.fillAmount = data.Exp/e;
        sick.fillAmount = (data.maxHealth - data.Health)/data.maxHealth;
        ịnjury.fillAmount = data.Damage;
        sleep.fillAmount = (data.maxSleep - data.Sleep)/data.maxSleep;
        food.fillAmount = (data.maxFood - data.Food)/data.maxFood;
        drink.fillAmount = (data.maxWater - data.Water)/data.maxWater;
        dirty.fillAmount = data.Dirty/data.maxDirty;
        toilet.fillAmount = (Mathf.Max(data.Shit/data.maxShit,data.Pee/data.maxPee));
    }
}
