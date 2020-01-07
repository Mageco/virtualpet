using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MilkItem : BaseDragItem
{
    ItemObject item;
    public CharController pet;
    bool eated = false;

    public void OnDestroy()
    {
        
    }

    protected override void Start(){
        base.Start();
        item = this.transform.parent.GetComponent<ItemObject>();
    }

    protected override void OnActive()
    {
        if(pet!= null && !eated){
            if(pet.data.Food > 0.8f * pet.data.maxFood){
                OnReturn();
                UIManager.instance.OnQuestNotificationPopup("Thú cưng cửa bạn không đói");
            }else{
                anim.Play("Active");
                pet.OnEat();
                eated = true;
                ItemManager.instance.ResetCameraTarget();
                this.transform.position = new Vector3(1000,1000,0);
                Invoke("OnReturn",5);
                
            }
        }

    }

    void OnReturn(){
        state = ItemDragState.None;
        anim.Play("Idle");
        eated = false;
        this.transform.position = originalPosition;
    }

    protected override void OnHit()
    {
        StartCoroutine(OnHitCouroutine());

    }

    IEnumerator OnHitCouroutine()
    {
        state = ItemDragState.Hited;
        yield return StartCoroutine(DoAnim("Drop_Light"));
        this.transform.rotation = originalRotation;
        fallSpeed = 0;
        anim.Play("Idle", 0);
        state = ItemDragState.None;

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            pet = other.GetComponent<CharController>();
            isHighlight = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isHighlight = false;
        }

    }
}

