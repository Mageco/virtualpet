using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    int itemId = 0;
    public Image icon;
    public Text price;
    public GameObject buyButton;
    public GameObject useButton;
    public GameObject usedButton;
    public GameObject coinIcon;
    public GameObject diamonIcon;
    public GameObject moneyIcon;

    public GameObject commingSoon;
    public GameObject locked;
    public Text levelRequired;
    Animator animator;
    bool isBusy = false;
    bool isLevelRequired = false;
    bool isCommingSoon = false;
    bool isCharacter = false;

    ItemState state = ItemState.OnShop;

    void Awake(){
        animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public void Load(Item d)
    {

        itemId = d.iD;
        //Debug.Log(d.iconUrl);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        if(!d.isAvailable){
            isCommingSoon = true;
        }

        if(d.levelRequire > GameManager.instance.GetPet(0).level){
            isLevelRequired = true;
        }

        if(isCommingSoon){
            commingSoon.SetActive(true);
            locked.SetActive(false);
        }else{
            commingSoon.SetActive(false);
            if(isLevelRequired){
                locked.SetActive(true);
                levelRequired.text = "Level " + d.levelRequire.ToString();
            }else
                locked.SetActive(false);
        }
            
        url = url.Replace(".png", "");
        //Debug.Log(url);
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();

        if (ApiManager.GetInstance().IsEquipItem(d.iD))
        {
            state = ItemState.Equiped;
        }
        else if (ApiManager.GetInstance().IsHaveItem(d.iD))
        {
            state = ItemState.Have;
        }
        else
        {
            state = ItemState.OnShop;
        }

        if (state == ItemState.OnShop)
        {
            buyButton.SetActive(true);
            useButton.SetActive(false);
            usedButton.SetActive(false);
        }
        else if (state == ItemState.Have)
        {
            buyButton.SetActive(false);
            useButton.SetActive(true);
            usedButton.SetActive(false);
            animator.Play("Bought", 0);
        }
        else if (state == ItemState.Equiped)
        {
            buyButton.SetActive(false);
            useButton.SetActive(false);
            usedButton.SetActive(true);
            animator.Play("Equiped", 0);
        }


        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }

    }


    public void Load(Pet d)
    {
        isCharacter = true;
        itemId = d.iD;
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();

        if(!d.isAvailable){
            isCommingSoon = true;
        }

        if(d.levelRequire > GameManager.instance.GetPet(0).level){
            isLevelRequired = true;
        }

        if(isCommingSoon){
            commingSoon.SetActive(true);
            locked.SetActive(false);
        }else{
            commingSoon.SetActive(false);
            if(isLevelRequired){
                locked.SetActive(true);
                levelRequired.text = "Level " + d.levelRequire.ToString();
            }else
                locked.SetActive(false);
        }

        if (ApiManager.GetInstance().IsEquipPet(d.iD))
        {
            state = ItemState.Equiped;
        }
        else if (ApiManager.GetInstance().IsHavePet(d.iD))
        {
            state = ItemState.Have;
        }
        else
        {
            state = ItemState.OnShop;
        }

        if (state == ItemState.OnShop)
        {
            buyButton.SetActive(true);
            useButton.SetActive(false);
            usedButton.SetActive(false);
        }
        else if (state == ItemState.Have)
        {
            buyButton.SetActive(false);
            useButton.SetActive(true);
            usedButton.SetActive(false);
            animator.Play("Bought", 0);
        }
        else if (state == ItemState.Equiped)
        {
            buyButton.SetActive(false);
            useButton.SetActive(false);
            usedButton.SetActive(true);
            animator.Play("Equiped", 0);
        }


        if (d.priceType == PriceType.Coin)
        {
            coinIcon.SetActive(true);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Diamond)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(true);
            moneyIcon.SetActive(false);
        }
        else if (d.priceType == PriceType.Money)
        {
            coinIcon.SetActive(false);
            diamonIcon.SetActive(false);
            moneyIcon.SetActive(true);
        }

    }

    // Update is called once per frame
    void UpdateState()
    {

    }

    public void OnBuy()
    {
        if (isBusy)
            return;

        if(isCommingSoon){
            MageManager.instance.OnNotificationPopup("Item will be available soon!");
            return;
        }

        if(isLevelRequired){
            MageManager.instance.OnNotificationPopup("Your pet level is not meet requirement!");
            return;
        }

        StartCoroutine(BuyCoroutine());
    }

    IEnumerator BuyCoroutine()
    {
        isBusy = true;

        if (isCharacter)
        {
            if (state == ItemState.Equiped)
                yield return null;
            else if (state == ItemState.Have)
            {
                animator.Play("Use", 0);
                yield return new WaitForSeconds(0.5f);
                UIManager.instance.UsePet(itemId);

            }
            else
            {
                animator.Play("Buy", 0);
                yield return new WaitForSeconds(1f);
                UIManager.instance.BuyPet(itemId);
            }

        }
        else
        {
            if(DataHolder.GetItem(itemId).itemType == ItemType.Diamond){
                MageManager.instance.OnNotificationPopup("Tính năng mua kim cương chưa mở");
            }else if(DataHolder.GetItem(itemId).itemType == ItemType.Coin){
                if(ApiManager.GetInstance().GetDiamond() > (DataHolder.GetItem(itemId).buyPrice)){
                    animator.Play("Use", 0);
                    yield return new WaitForSeconds(0.5f);
                    ApiManager.GetInstance().AddDiamond(-DataHolder.GetItem(itemId).buyPrice);
                    GameManager.instance.AddCoin(DataHolder.GetItem(itemId).sellPrice);
                    animator.Play("Idle", 0);
                    MageManager.instance.OnNotificationPopup("bạn đã mua thành công");
                }else{
                    MageManager.instance.OnNotificationPopup ("You have not enough Coin");
                }
            }else
            {
                if (state == ItemState.Equiped)
                    yield return null;
                else if (state == ItemState.Have)
                {
                    animator.Play("Use", 0);
                    yield return new WaitForSeconds(0.5f);
                    UIManager.instance.UseItem(itemId);
                }
                else
                {
                    animator.Play("Buy", 0);
                    yield return new WaitForSeconds(1f);
                    UIManager.instance.BuyItem(itemId);
                    MageManager.instance.OnNotificationPopup("bạn đã mua thành công");
                }
            }

            

        }

        isBusy = false;
    }

    public void OnUse()
    {

    }
}
