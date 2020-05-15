using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinRewardPanel : MonoBehaviour
{

    public Image icon;
    public Text text;
    public Text itemName;

    public void Load(Sprite s,string number,string n)
    {
        MageManager.instance.PlaySound("Collect_Achivement", false);
        text.text = number;
        icon.sprite = s;
        itemName.text = n;
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
