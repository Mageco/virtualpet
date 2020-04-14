using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemotePlayerPanel : MonoBehaviour
{
    public Image avatar;


    public void Load(Sprite image)
    {
        avatar.sprite = image;
    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }
}
