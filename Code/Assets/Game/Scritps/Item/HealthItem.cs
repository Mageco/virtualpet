using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealthItem : BaseDragItem
{
    ItemObject item;
    public CharController pet;
    bool eated = false;

    public float amounnt = 1;
    public float timeDelay = 10;
    public SickType sickType = SickType.Sick;

    public GameObject effectPrefab;

    GameObject effect;


    public void OnDestroy()
    {
        
    }

    public void Open(){
       isDragable  = true;
    }

    public void Close(){
        isDragable = false;
    }

    protected override void Start(){
        base.Start();
        item = this.transform.parent.GetComponent<ItemObject>();
    }

    protected override void OnActive()
    {
        if(!eated){
            eated = true;
            anim.Play("Active");
            if (pet != null)
            {
                pet.OnHealth(sickType,amounnt);
                if(effect == null){
                    effect = GameObject.Instantiate(effectPrefab,pet.transform.position,Quaternion.identity);
                    effect.transform.parent = pet.transform;
                    effect.GetComponent<AutoDestroy>().liveTime = timeDelay;
                }
                    
            }
            GameManager.instance.ResetCameraTarget();
            StartCoroutine(Deactive());
        }

    }

    IEnumerator Deactive(){
        this.transform.position = new Vector3(1000,1000,0);
        yield return new WaitForSeconds(timeDelay);
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;
		height = originalHeight;
		this.scalePosition = this.transform.position + new Vector3(0,-height,0);
        fallSpeed = 0;
        anim.Play("Idle", 0);
        eated = false;
        state = ItemDragState.None;
    }



    protected override void OnHit()
    {
        StartCoroutine(OnHitCouroutine());
    }

    IEnumerator OnHitCouroutine()
    {
        yield return new WaitForSeconds(1);
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;
		height = originalHeight;
		this.scalePosition = this.transform.position + new Vector3(0,-height,0);
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

