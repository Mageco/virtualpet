using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
int itemId = 0;
    public Image icon;
    public Text number;

    // Start is called before the first frame update
    public void Load(Item d)
    {
        
        itemId = d.iD;
        //Debug.Log(d.iconUrl);
        string url = d.iconUrl.Replace("Assets/Game/Resources/","");
        url = url.Replace(".png","");
        //Debug.Log(url);
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;

        if(ApiManager.instance.UsedItem(d.iD)){
        }
    }
}

