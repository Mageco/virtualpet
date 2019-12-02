using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxController : AnimalController
{
    public float maxTimeWait = 5;
    public ChickenController target;
    public ChickenController[] targets;

    public Vector3 fleePoint;

    protected override void Load(){
        speed = maxSpeed/2f;
        targets = GameObject.FindObjectsOfType<ChickenController>();
        fleePoint = agent.transform.position;
    }

    protected override void Think()
    {
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
            agent.Stop();
            Debug.Log("Hit");
            isAbort = true;
            state = AnimalState.Hit;
        }else if(state == AnimalState.Run){
             agent.Stop();
            state = AnimalState.Hit_Grab;
            isAbort = true;
        }
    }

    public override void OnFlee(){
        if(state != AnimalState.Flee)
        {
            Debug.Log("Flee");
            isAbort = true;
            agent.Stop();
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
        agent.transform.position = GetFleePoint();
        this.transform.position = agent.transform.position;
        fleePoint = this.transform.position;
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
        agent.maxSpeed = this.speed;
        if(target == null){
            isAbort = true;
            state = AnimalState.Flee;
        }else
        {
            speed = Random.Range(maxSpeed/1.5f,maxSpeed);
            while(!isAbort){
                if(target.state == AnimalState.Cached || target.state == AnimalState.Hold){
                    GetTarget();
                }

                anim.Play("Seek_" + direction.ToString(),0);   
                agent.SetDestination(target.transform.position);
                yield return new WaitForEndOfFrame(); 
                if(!isAbort  && Vector2.Distance(this.transform.position,target.transform.position) < 3f && target.state != AnimalState.Cached && target.state != AnimalState.Hold){
                    state = AnimalState.Run;
                    agent.Stop();
                    target.OnCached();
                    Minigame.instance.UpdateLive();
                    isAbort = true;
                }
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
        anim.Play("Hit_Grab_" + direction.ToString(),0);
        yield return StartCoroutine(Wait(Random.Range(2,3))); 
        state = AnimalState.Run;
        isAbort = true;
        CheckAbort(); 
    }

    IEnumerator Flee()
    {    
        speed = Random.Range(maxSpeed,maxSpeed*1.3f);
        agent.maxSpeed = this.speed * 2;
        anim.Play("Flee_" + direction.ToString(),0);
        yield return StartCoroutine(DoAnim("Start_Flee_" + direction.ToString()));
        yield return StartCoroutine(MoveToPoint(fleePoint));
        if(!isAbort)
            state = AnimalState.Idle;
        CheckAbort();
    }

    IEnumerator Run()
    {
        speed = Random.Range(maxSpeed/1.5f,maxSpeed);
        anim.Play("Run_" + direction.ToString(),0);
        yield return StartCoroutine(DoAnim("Start_Run_" + direction.ToString()));
        yield return StartCoroutine(MoveToPoint(fleePoint));
        if(!isAbort)
            state = AnimalState.Idle;
        CheckAbort();
    }

    void GetTarget(){
        float l = 1000;
        for(int i=0;i<targets.Length;i++){
            if(targets[i].state != AnimalState.Cached){
                if(Vector2.Distance(this.transform.position,targets[i].transform.position) < l){
                    target = targets[i];
                    l = Vector2.Distance(this.transform.position,targets[i].transform.position);
                }
            }
        }
    }

    Vector3 GetFleePoint(){
        return fleePoints[Random.Range(0,fleePoints.Count)].transform.position;
    }
}
