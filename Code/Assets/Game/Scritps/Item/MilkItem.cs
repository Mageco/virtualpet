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
        anim.Play("Active");
        if (pet != null && !eated)
        {
            pet.OnEat();
            eated = true;
        }
        GameManager.instance.ResetCameraTarget();
        ApiManager.GetInstance().RemoveItem(item.itemID);
        Destroy(this.transform.parent.gameObject);
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

