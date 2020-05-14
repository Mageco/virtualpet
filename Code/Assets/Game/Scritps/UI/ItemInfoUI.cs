using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    int itemId = 0;
    public Image icon;
    public Text itemName;
    public Text description;

    void Awake()
    {

    }

    // Start is called before the first frame update
    public void LoadItem(int itemId)
    {
        this.itemId = itemId;
        Item d = DataHolder.GetItem(itemId);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        itemName.text = DataHolder.GetItem(itemId).GetName(MageManager.instance.GetLanguage());
        description.text = DataHolder.GetItem(itemId).GetDescription(MageManager.instance.GetLanguage());
    }



    public void LoadPet(int petId)
    {
        itemId = petId;
        Pet d = DataHolder.GetPet(petId);
        itemName.text = d.GetName(MageManager.instance.GetLanguage());
        description.text = d.GetDescription(MageManager.instance.GetLanguage());
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;

    }

    public void Close()
    {
        this.GetComponent<Popup>().Close();
    }

}
