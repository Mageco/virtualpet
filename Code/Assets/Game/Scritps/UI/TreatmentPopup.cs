using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TreatmentPopup : MonoBehaviour
{

    public Text timeWait;
    float coin;
    public Text price;
    SickType sickType;
    public Sprite[] sickTypes;
    public Image sickTypeIcon;
    public Text description;
    Pet pet;

    public void Load(Pet p,SickType type)
    {
        pet = p;
        sickType = type;
        
        if(sickType == SickType.Sick)
        {
            sickTypeIcon.sprite = sickTypes[0];
            description.text = DataHolder.Dialog(0).GetDescription(MageManager.instance.GetLanguage());
        }else if(sickType == SickType.Injured)
        {
            sickTypeIcon.sprite = sickTypes[1];
            description.text = DataHolder.Dialog(1).GetDescription(MageManager.instance.GetLanguage());
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = pet.MaxTimeSick - (float)(System.DateTime.Now - pet.timeSick).TotalSeconds;
        float m = (int)(t / 60);
        float s = (int)(t - m * 60);
        timeWait.text = m.ToString("00") + ":" + s.ToString("00");
        coin = Mathf.Max((int)(t / 60),100);
        price.text = coin.ToString();
    }

    public void OnConfirm()
    {
        if (GameManager.instance.GetCoin() >= coin)
        {
            UIManager.instance.OnTreatmentConfirmPopup(pet, sickType);
            this.Close();
        }
        else
            MageManager.instance.OnNotificationPopup(DataHolder.GetDialog(6).GetDescription(MageManager.instance.GetLanguage()));
        //UIManager.instance.On
    }

    public void Close() {
        this.GetComponent<Popup>().Close();
    }
}
