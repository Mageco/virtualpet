﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestPanel : MonoBehaviour
{
    int questId = 0;
    public Image exp;
    public Image coin;
    public Image diamon;
    public Image item;
    Animator anim;
    
    public GameObject[] itemEffects;

    void Awake(){
        anim = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        MageManager.instance.PlaySoundName("Win",false);
        yield return new WaitForSeconds(1f);
       
        //MageManager.instance.PlaySoundName("Changing2",false);
    }

    public void Load(int id)
    {
        questId = id;
        
        if (DataHolder.Quest(questId).haveItem)
        {
            string url = DataHolder.GetItem(DataHolder.Quest(questId).itemId).iconUrl.Replace("Assets/Game/Resources/", "");
            url = url.Replace(".png", "");
            item.sprite = Resources.Load<Sprite>(url) as Sprite;
        }else
        {
            item.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.Quest(questId).expValue != 0)
        {
            exp.GetComponentInChildren<Text>().text = DataHolder.Quest(questId).expValue.ToString();
        }
        else
        {
            exp.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.Quest(questId).coinValue != 0)
        {
            coin.GetComponentInChildren<Text>().text = DataHolder.Quest(questId).coinValue.ToString();
        }
        else
        {
            coin.transform.parent.gameObject.SetActive(false);
        }

        if (DataHolder.Quest(questId).diamondValue != 0)
        {
            diamon.GetComponentInChildren<Text>().text = DataHolder.Quest(questId).diamondValue.ToString();
        }
        else
        {
            diamon.transform.parent.gameObject.SetActive(false);
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close(){
        StartCoroutine(CloseCoroutine());
    }

    IEnumerator CloseCoroutine()
    {
        Debug.Log("Close Quest");
        anim.Play("Close");    
        yield return new WaitForEndOfFrame();
        for(int i=0;i<itemEffects.Length;i++){
            if(itemEffects[i].transform.parent.gameObject.activeSelf){
                itemEffects[i].SetActive(true);
                MageManager.instance.PlaySoundName("Changing2",false);
                yield return new WaitForSeconds(0.5f);
            }
        }
        this.GetComponent<Popup>().Close();
        yield return new WaitForSeconds(0.1f);
        QuestManager.instance.EndCompleteQuest();
    }
}
