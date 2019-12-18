using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HealthItem : ItemDrag
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


    protected override void OnActive(CharController pet)
    {
        if(!isBusy && !eated){
            eated = true;
            anim.Play("Active");
            pet.OnHealth(sickType,amounnt);
            if(effect == null){
                effect = GameObject.Instantiate(effectPrefab,pet.transform.position,Quaternion.identity);
                effect.transform.parent = pet.transform;
                effect.GetComponent<AutoDestroy>().liveTime = timeDelay;
            }
            GameManager.instance.ResetCameraTarget();
            //Reset();
            StartCoroutine(Deactive());
        }

    }

    IEnumerator Deactive(){
        this.transform.position = new Vector3(1000,1000,0);
        yield return new WaitForSeconds(timeDelay);
        Reset();
    }

    void Reset(){
        StopAllCoroutines();
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;
		
        anim.Play("Idle", 0);
        eated = false;
        isDrag = false;
        isBusy = false;
    }
}

