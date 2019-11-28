using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{

    public Image energy;
    public Text level; 
    public Image icon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProfile();
    }

    public void UpdateProfile(){
        if (GameManager.instance.GetPet(0) == null)  {
            return;
        }
        energy.fillAmount = GameManager.instance.GetPet(0).energy/GameManager.instance.GetPet(0).maxEnergy;
        level.text = GameManager.instance.GetPet(0).level.ToString();
        if(energy.fillAmount < 0.25f){
            energy.color = Color.red;
        }else if(energy.fillAmount < 0.6f)
            energy.color = Color.yellow;
        else
            energy.color = Color.green;
    }
}
