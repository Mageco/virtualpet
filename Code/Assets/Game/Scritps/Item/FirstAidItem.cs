using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidItem : ItemCollider
{
    bool isOpen = false;
    public HealthItem[] healthItems;
    BoxCollider2D collider;

    protected override void Awake()
    {
        base.Awake();
        collider = this.GetComponent<BoxCollider2D>();
    }

    protected override void OnClick()
    {
        if(isOpen){
            Close();
        }else
        {
            Open();
        }
    }

    protected override void OnMouseDown()
    {
        bool isActive = false;
        for(int i = 0; i < healthItems.Length; i++)
        {
            if (healthItems[i].isActive)
                isActive = true;
        }
        if (isActive)
            return;
        base.OnMouseDown();
    }

    void Open(){
        MageManager.instance.PlaySoundName("Drag",false);
        Debug.Log("Open");
        isOpen = true;
        for(int i=0;i<healthItems.Length;i++){
            healthItems[i].Open();
        }
        animator.Play("Open",0);
        collider.size = new Vector2(9, 5);
        collider.offset = new Vector2(-2, 2.7f);
    }

    void Close(){
        MageManager.instance.PlaySoundName("Drag",false);
        isOpen = false;
        for(int i=0;i<healthItems.Length;i++){
            healthItems[i].Close();
        }

        animator.Play("Idle",0);
        collider.size = new Vector2(4.5f, 5);
        collider.offset = new Vector2(0, 2.7f);
    }
}
