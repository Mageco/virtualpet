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
    public Text sick;
    public Text ịnjury;
    public Text sleep;
    public Text food;
    public Text drink;
    public Text toilet;
    public Text dirty;
    Pet data;

    public void Load(int id){
        data = GameManager.instance.GetPet(id);
        petName.text = data.petName;
        level.text = "Level " + data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        expProgress.fillAmount = data.Exp/e;
        sick.text = (data.maxHealth - data.Health).ToString("F0");
        ịnjury.text = data.Damage.ToString("F0");
        sleep.text = data.Sleep.ToString("F0");
        food.text = data.Food.ToString("F0");
        drink.text = data.Water.ToString("F0");
        dirty.text = data.Dirty.ToString("F0");
        toilet.text = ((data.Shit + data.Pee)/2).ToString("F0");
        
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
        sick.text = (data.maxHealth - data.Health).ToString("F0");
        ịnjury.text = data.Damage.ToString("F0");
        sleep.text = data.Sleep.ToString("F0");
        food.text = data.Food.ToString("F0");
        drink.text = data.Water.ToString("F0");
        dirty.text = data.Dirty.ToString("F0");
        toilet.text = ((data.Shit + data.Pee)/2).ToString("F0");  
    }
}
