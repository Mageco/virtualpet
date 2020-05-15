using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidItem : BaseFloorItem
{
    bool isOpen = false;
    public HealthItem[] healthItems;
    BoxCollider2D collide;

    protected override void Awake()
    {
        base.Awake();
        collide = this.GetComponent<BoxCollider2D>();
    }

    protected override void OnClick()
    {
        state = EquipmentState.Idle;
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
        MageManager.instance.PlaySound("Drag",false);
        Debug.Log("Open");
        isOpen = true;
        for(int i=0;i<healthItems.Length;i++){
            healthItems[i].Open();
        }
        animator.Play("Open",0);
        collide.size = new Vector2(9, 5);
        collide.offset = new Vector2(-2, 0);
    }

    void Close(){
        MageManager.instance.PlaySound("Drag",false);
        isOpen = false;
        for(int i=0;i<healthItems.Length;i++){
            healthItems[i].Close();
        }

        animator.Play("Idle",0);
        collide.size = new Vector2(4.5f, 5);
        collide.offset = new Vector2(0, 0);
    }
}
