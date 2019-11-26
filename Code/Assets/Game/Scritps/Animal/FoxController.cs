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
         targets = GameObject.FindObjectsOfType<ChickenController>();
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
        }else if (state == AnimalState.Hit_Grab)
        {
            StartCoroutine(Hit_Grab());
        }
    }


    void OnMouseUp(){
        if(state == AnimalState.Seek){
            Debug.Log("Hit");
            isAbort = true;
            state = AnimalState.Hit;
        }else if(state == AnimalState.Run){
            state = AnimalState.Hit_Grab;
            isAbort = true;
        }
    }

    public override void OnFlee(){
        if(state != AnimalState.Flee)
        {
            Debug.Log("Flee");
            isAbort = true;
            if(state == AnimalState.Run || state == AnimalState.Hit_Grab)
            {
                Debug.Log("Run");
                target.OffCached();
                target.transform.position = this.transform.position;
                Minigame.instance.UpdateLive();
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
            if(target.state == AnimalState.Cached || target.state == AnimalState.Hold){
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
            if(!isAbort  && Vector2.Distance(this.transform.position,target.transform.position) < 1f && target.state != AnimalState.Cached && target.state != AnimalState.Hold){
                
                state = AnimalState.Run;
                target.OnCached();
                Minigame.instance.UpdateLive();
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

    IEnumerator Hit_Grab()
    {
        Debug.Log("Hit_Grab");
        anim.Play("Hit_Grab_" + direction.ToString(),0);
        yield return StartCoroutine(Wait(Random.Range(2,3))); 
        state = AnimalState.Run;
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
        for(int i=0;i<targets.Length;i++){
            if(targets[i].state != AnimalState.Cached){
                target = targets[i];
                return;
            }
        }
    }
}
