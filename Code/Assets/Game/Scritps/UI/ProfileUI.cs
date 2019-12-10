using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    int id = 0;
    public Text level;
    public Image icon;
    public Image energyProgress;
 
    Pet data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProfile();
    }

    public void Load(int itemId){
        id = itemId;
        string url = DataHolder.GetPet(itemId).iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
    }

    public void UpdateProfile(){
        data = GameManager.instance.GetPet(id);
        level.text = data.level.ToString();
        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        energyProgress.fillAmount = data.Exp/e;
    }
}
