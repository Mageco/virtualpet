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
        Reset();
        isDragable = false;

    }

    protected override void Start(){
        base.Start();
        item = this.transform.parent.GetComponent<ItemObject>();
    }

    protected override void Update()
    {
        base.Update();
        if (isDrag)
        {
            anim.Play("Drag", 0);
        }
        else
            anim.Play("Idle", 0);
    }

    protected override void OnMouseUp()
    {
        if (pet != null)
            OnActive(pet);
        else
            base.OnMouseUp();
    }

    void OnActive(CharController pet)
    {
        if(!isBusy && !eated){
            eated = true;
            anim.Play("Active");
            pet.OnHealth(sickType,amounnt);
            MageManager.instance.PlaySoundName("Heal",false);
            if(effect == null){
                effect = GameObject.Instantiate(effectPrefab,pet.transform.position,Quaternion.identity);
                effect.transform.parent = pet.transform;
                effect.GetComponent<AutoDestroy>().liveTime = timeDelay;
            }
            ItemManager.instance.ResetCameraTarget();
            //Reset();
            StartCoroutine(Deactive());
        }
    }

    IEnumerator Deactive(){
        isDrag = false;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
        if(other.tag == "Player")
        {
            pet = other.GetComponent<CharController>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<CharController>() == pet)
        {
            pet = null;
        }
    }
}

