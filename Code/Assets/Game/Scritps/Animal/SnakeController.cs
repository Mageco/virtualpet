using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : AnimalController
{
    public float maxTimeWait = 5;
    public ChickenController target;
    BoxCollider2D col;

    protected override void Load(){
        speed = maxSpeed/2f;
        col = this.GetComponent<BoxCollider2D>();
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
        }
    }


    void OnMouseUp(){
        if(state == AnimalState.Seek){
            Debug.Log("Hit");
            isAbort = true;
            state = AnimalState.Hit;
        }
    }

    IEnumerator Idle()
    {
        anim.Play("Idle_" + direction.ToString());
        yield return StartCoroutine(Wait(maxTimeWait));
        GetTarget();
        if(target != null)
            state = AnimalState.Seek;
        DoAction();
    }

    IEnumerator Seek()
    {
        GetTarget();
        speed = Random.Range(maxSpeed/1.5f,maxSpeed);
        while(!isAbort && target != null && target.gameObject.activeSelf && Vector2.Distance(this.transform.position,target.transform.position) > 1f){
            if(target.transform.position.x > this.transform.position.x){
                SetDirection(Direction.R);
            }else
                SetDirection(Direction.L);
            anim.Play("Seek_" + direction.ToString(),0);   
            Vector3 d = Vector3.Normalize(target.transform.position - this.transform.position);
            this.transform.position += d * speed * Time.deltaTime;
            yield return new WaitForEndOfFrame(); 
        }
        
        if(!isAbort)
        {
            if(target != null && target.gameObject.activeSelf){
                state = AnimalState.Run;
                target.gameObject.SetActive(false);
            }
            else{
                GetTarget();
                if(target != null)
                    state = AnimalState.Seek;
                else
                    state = AnimalState.Flee;
            }
        }
        yield return new WaitForEndOfFrame(); 
        DoAction();
    }

    IEnumerator Hit()
    {
        Debug.Log("Hit");
        anim.Play("Hit_" + direction.ToString(),0);
        yield return StartCoroutine(Wait(Random.Range(2,3))); 
        state = AnimalState.Flee;
        DoAction();     
    }

    IEnumerator Flee()
    {
        Vector3 target = originalPosition;
        speed = Random.Range(maxSpeed/1.5f,maxSpeed);
        if(target.x > this.transform.position.x){
            SetDirection(Direction.R);
        }else
            SetDirection(Direction.L);
        anim.Play("Flee_" + direction.ToString(),0);

        yield return StartCoroutine(MoveToPoint(target));
        state = AnimalState.Idle;
        DoAction();   
    }

    IEnumerator Run()
    {
        //yield return StartCoroutine(DoAnim("Bite_" + direction.ToString()));
        speed = Random.Range(maxSpeed/1.5f,maxSpeed);
        if(originalPosition.x > this.transform.position.x){
            SetDirection(Direction.R);
        }else
            SetDirection(Direction.L);
        anim.Play("Run_" + direction.ToString(),0);

        yield return StartCoroutine(MoveToPoint(originalPosition));
        state = AnimalState.Idle;
        DoAction(); 
    }

    void GetTarget(){
        ChickenController[] t = GameObject.FindObjectsOfType<ChickenController>();
        if(t != null && t.Length > 0)
        {
            int id = Random.Range(0,t.Length);
            target = t[id];
        }
    }



 
}
