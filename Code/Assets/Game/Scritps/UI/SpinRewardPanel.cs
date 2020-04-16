using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinRewardPanel : MonoBehaviour
{

    public Image icon;
    public Text text;

    public void Load(Sprite s,string n)
    {
        text.text = n;
        icon.sprite = s;
    }


    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
