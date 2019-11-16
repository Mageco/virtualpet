using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxController : AnimalController
{
    public float maxTimeWait = 5;
    public ChickenController target;
    public ChickenController[] targets;

    protected override void Load(){
        speed = maxSpeed/2f;
    }

    protected override void Think()
    {
        if(Minigame.instance.IsInBound(this.transform.position))
            state = AnimalState.Seek;
        else
            state = AnimalState.Idle;
    }

    protected override void DoAction()
    {
        anim.speed = 1;
        isAbort = false;
        isArrived = false;
        if (state == AnimalState.Idle)
        {
            StartCoroutine(Idle());
        }else if (state == AnimalState.Hit)
        {
            StartCoroutine(Hit());
        }else if (state == AnimalState.Seek)
        {
            StartCoroutine(Seek());
        }else if (state == AnimalState.Run)
        {
            StartCoroutine(Run());
        }else if (state == AnimalState.Flee)
        {
            StartCoroutine(Flee());
        }
    }


    void OnMouseUp(){
        if(state == AnimalState.Seek){
            Debug.Log("Hit");
            isAbort = true;
            state = AnimalState.Hit;
        }else if(state == AnimalState.Run){
            target.gameObject.SetActive(true);
            target.transform.position = this.transform.position;
            Minigame.instance.AddLive(1);
            state = AnimalState.Hit;
            isAbort = true;
        }
    }

    public override void OnFlee(){
        if(state != AnimalState.Flee)
        {
            Debug.Log("Flee");
            isAbort = true;
            if(state == AnimalState.Run)
            {
                Debug.Log("Run");
                target.gameObject.SetActive(true);
                target.transform.position = this.transform.position;
                Minigame.instance.AddLive(1);
            }
            state = AnimalState.Flee;
        }
    }


    IEnumerator Idle()
    {
        anim.Play("Idle_" + direction.ToString());
        yield return StartCoroutine(Wait(maxTimeWait));
        GetTarget();
        if(target != null){
            isAbort = true;
            state = AnimalState.Seek;
        }
        CheckAbort();
    }

    IEnumerator Seek()
    {
        GetTarget();
        speed = Random.Range(maxSpeed/1.5f,maxSpeed);
        while(!isAbort){
            if(!target.gameObject.activeSelf){
                GetTarget();
            }
            if(target.transform.position.x > this.transform.position.x){
                SetDirection(Direction.R);
            }else
                SetDirection(Direction.L);
            anim.Play("Seek_" + direction.ToString(),0);   
            Vector3 d = Vector3.Normalize(target.transform.position - this.transform.position);
            this.transform.position += d * speed * Time.deltaTime;
            yield return new WaitForEndOfFrame(); 
            if(!isAbort && target.gameObject.activeSelf && Vector2.Distance(this.transform.position,target.transform.position) < 1f){
                Minigame.instance.AddLive(-1);
                state = AnimalState.Run;
                target.gameObject.SetActive(false);
                isAbort = true;
            }

        }
        CheckAbort();
    }

    IEnumerator Hit()
    {
        Debug.Log("Hit");
        anim.Play("Hit_" + direction.ToString(),0);
        yield return StartCoroutine(Wait(Random.Range(2,3))); 
        state = AnimalState.Seek;
        isAbort = true;
        CheckAbort(); 
    }

    IEnumerator Flee()
    {    
        speed = Random.Range(maxSpeed,maxSpeed*1.3f);
        if(originalPosition.x > this.transform.position.x){
            SetDirection(Direction.R);
        }else
            SetDirection(Direction.L);
        anim.Play("Flee_" + direction.ToString(),0);

        yield return StartCoroutine(MoveToPoint(originalPosition));
        if(!isAbort)
            state = AnimalState.Idle;
       CheckAbort();
    }

    IEnumerator Run()
    {
        speed = Random.Range(maxSpeed/1.5f,maxSpeed);
        if(originalPosition.x > this.transform.position.x){
            SetDirection(Direction.R);
        }else
            SetDirection(Direction.L);
        anim.Play("Run_" + direction.ToString(),0);

        yield return StartCoroutine(MoveToPoint(originalPosition));
        if(!isAbort)
            state = AnimalState.Idle;
        CheckAbort();
    }

    void GetTarget(){
         targets = GameObject.FindObjectsOfType<ChickenController>();
        for(int i=0;i<targets.Length;i++){
            if(targets[i].gameObject.activeSelf){
                target = targets[i];
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Chicken")
        {
            if(other.transform.parent.GetComponent<ChickenController>() != null && state == AnimalState.Seek){
                //other.transform.parent.GetComponent<ChickenController>().gameObject.SetActive(false);
                //OnRun();
            }            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }

 
}
