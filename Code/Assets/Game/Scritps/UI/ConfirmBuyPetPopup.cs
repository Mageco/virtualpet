using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBuyPetPopup : MonoBehaviour
{
    int itemId = 0;
   
    public Image icon;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject happyIcon;
    public Text petName;
    public Text priceText;
    public Text question;
    public GameObject okButton;


    public void Load(Pet d){
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        question.text = DataHolder.Dialog(5).GetDescription(MageManager.instance.GetLanguage()) + " ";
        priceText.text = (d.buyPrice).ToString();
        petName.text = d.GetName(MageManager.instance.GetLanguage());

        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            happyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Happy)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            happyIcon.SetActive(true);
        }
    }

    public void Confirm(){
        if (UIManager.instance.petRequirementPanel)
        {
            UIManager.instance.petRequirementPanel.Close();
        }
        UIManager.instance.BuyPet(itemId);
        ItemManager.instance.SetLocation(GameType.House);
        this.Close();
    }

    public void Close(){
        this.GetComponent<Popup>().Close();
    }
}
