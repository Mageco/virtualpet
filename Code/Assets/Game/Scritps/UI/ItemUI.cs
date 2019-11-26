﻿using System.Collections;
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
    bool isInteract = true;

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
        if(!d.isAvailable || d.levelRequire > GameManager.instance.GetPet(0).level){
            //url = d.iconLockUrl.Replace("Assets/Game/Resources/", "");
            isInteract = false;
        }

        if(!d.isAvailable){
            commingSoon.SetActive(true);
        }else
            commingSoon.SetActive(false);

        if(d.levelRequire > GameManager.instance.GetPet(0).level){
            locked.SetActive(true);
            levelRequired.text = "Level " + d.levelRequire.ToString();
        }else
            locked.SetActive(false);
       
        url = url.Replace(".png", "");
        //Debug.Log(url);
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();

        if (ApiManager.instance.IsEquipItem(d.iD))
        {
            state = ItemState.Equiped;
        }
        else if (ApiManager.instance.IsHaveItem(d.iD))
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
        //Debug.Log(d.iconUrl);
        string url = d.iconUrl.Replace("Assets/Game/Resources/", "");
        url = url.Replace(".png", "");
        //Debug.Log(url);
        icon.sprite = Resources.Load<Sprite>(url) as Sprite;
        price.text = d.buyPrice.ToString();

        if (ApiManager.instance.IsEquipPet(d.iD))
        {
            state = ItemState.Equiped;
        }
        else if (ApiManager.instance.IsHavePet(d.iD))
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
        if (isBusy || !isInteract)
            return;


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
                
            }else if(DataHolder.GetItem(itemId).itemType == ItemType.Coin){
                if(ApiManager.instance.GetDiamond() > (DataHolder.GetItem(itemId).buyPrice)){
                    ApiManager.instance.AddDiamond(-DataHolder.GetItem(itemId).buyPrice);
                    GameManager.instance.AddCoin(DataHolder.GetItem(itemId).sellPrice);
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
                }
            }

            

        }

        isBusy = false;
    }

    public void OnUse()
    {

    }
}
