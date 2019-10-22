using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    ItemData data = new ItemData();
    public Image icon;
    public Text price;
    public GameObject buyButton;
    public GameObject useButton;
    public GameObject usedButton;
    public GameObject coinIcon;
    public GameObject diamonIcon;

    // Start is called before the first frame update
    public void Load(ItemData d)
    {
        
        data.Copy(d);
        icon.sprite = data.icon;
        price.text = data.price.ToString();
        if(data.itemState == ItemState.Buy){
            buyButton.SetActive(true);
            useButton.SetActive(false);
            usedButton.SetActive(false);
        }else if(data.itemState == ItemState.Use){
            buyButton.SetActive(false);
            useButton.SetActive(true);
            usedButton.SetActive(false);
        }else if(data.itemState == ItemState.Used){
            buyButton.SetActive(false);
            useButton.SetActive(false);
            usedButton.SetActive(true);
        }

        if(data.priceType == PriceType.Coin){
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
        }else 
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);            
        }

    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuy(){

    }

    public void OnUse(){

    }
}
