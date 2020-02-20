﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HappyItem : MonoBehaviour
{
    public ItemSaveDataType itemSaveDataType = ItemSaveDataType.Happy;
    Animator animator;
    public int value = 1;
    bool isSound = true;
    public GameObject body;
    public void Load(int e,bool isSound){
        value = e;
        this.isSound = isSound;
    }

    void Awake(){
        animator = this.GetComponent<Animator>();
        
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(isSound)
            MageManager.instance.PlaySoundName("Button",false);
        Vector3 pos = this.transform.position;
        pos.z = (this.transform.position.y - 2) * 10;
        this.transform.position = pos;
        yield return new WaitForSeconds(0.5f);
        OnPick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPick()
    {
        GameManager.instance.LogAchivement(AchivementType.CollectHeart);
        if(CheckPositionOutBound())
            StartCoroutine(Pick());
        else
        {
            GameManager.instance.AddHappy(value);
            Destroy(this.gameObject);
        }
    }

    IEnumerator Pick(){
        animator.enabled = false;
        this.body.transform.parent = Camera.main.transform;
        Vector3 target = Camera.main.ScreenToWorldPoint(UIManager.instance.heartIcon.transform.position) - Camera.main.transform.position;
        target.z = -100;
        while (Vector2.Distance(this.body.transform.localPosition,target) > 0.5)
        {
            this.body.transform.localPosition = Vector3.Lerp(this.body.transform.localPosition, target, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.AddHappy(value);
        Destroy(this.body.gameObject);
        Destroy(this.gameObject);
    }

    bool CheckPositionOutBound()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(this.transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }
}
