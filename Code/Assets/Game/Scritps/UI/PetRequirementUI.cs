using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetRequirementUI : MonoBehaviour
{
    public Image icon;
    public Text number;
    public GameObject done;

    public void Load(string url,int num,int maxNum)
    {
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        number.text = num.ToString() + "/" + maxNum.ToString();
        if (num >= maxNum)
        {
            done.SetActive(true);
        }
        else
            done.SetActive(false);
    }

    public void OnClick()
    {
        MageManager.instance.OnNotificationPopup("Find the items by collecting or completing challenges");
    }
}
