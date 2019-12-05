using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    int id = 0;
    public Text level;
    //public Text exp;
    //public Text energy;
    //public Image expProgress;
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

    public void UpdateProfile(){
        if (GameManager.instance.GetPet(0) == null)  {
            return;
        }
        data = GameManager.instance.GetPet(id);
        level.text = data.level.ToString();
        //float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        //exp.text = data.Exp.ToString("F0") + "/" + e.ToString("F0");
        //energy.text = data.energy.ToString("F0") + "/" + data.maxEnergy.ToString("F0");
        //expProgress.fillAmount = data.Exp/e;

        float e = 10 * (data.level) + 10 * (data.level) * (data.level);
        energyProgress.fillAmount = data.Exp/e;

        //energyProgress.fillAmount = data.energy/data.maxEnergy;
    }
}
