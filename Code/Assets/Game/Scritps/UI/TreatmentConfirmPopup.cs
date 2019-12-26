using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreatmentConfirmPopup : MonoBehaviour
{
    int coin;
    public Text price;
    SickType sickType;
    public Sprite[] sickTypes;
    public Image sickTypeIcon;
    Pet pet;
    // Start is called before the first frame update
    public void Load(Pet p, SickType type)
    {
        pet = p;
        sickType = type;
        float t = p.MaxTimeSick - (System.DateTime.Now - p.timeSick).Seconds;
        if (sickType == SickType.Sick)
        {
            sickTypeIcon.sprite = sickTypes[0];
        }
        else if (sickType == SickType.Injured)
        {
            sickTypeIcon.sprite = sickTypes[1];
        }

        coin = (int)((pet.MaxTimeSick - (System.DateTime.Now - pet.timeSick).Seconds) / 60 * 10);
        price.text = coin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        coin = (int)((pet.MaxTimeSick - (System.DateTime.Now - pet.timeSick).Seconds) / 60 * 10);
        price.text = coin.ToString();
    }

    public void OnConfirm()
    {
        GameManager.instance.OnTreatment(pet, sickType,coin);
        this.Close();
        //UIManager.instance.On
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
