using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : AnimalController
{

    public bool isCatched;

    protected override void Load(){
        speed = maxSpeed/2f;
        //this.originalScale = this.originalScale * Random.Range(0.9f,1.1f);
    }

    protected override void Think()
    {
        int ran = Random.Range(0, 100);
        if (ran > 45)
        {
            state = AnimalState.Run;
        }
        else if(ran > 35)
        {
            state = AnimalState.Eat;
        }
        else
        {
            state = AnimalState.Idle;
        }
    }

    protected override void DoAction()
    {
        isAbort = false;
        isArrived = false;
        if (state == AnimalState.Idle)
        {
            StartCoroutine(Idle());
        }
        else if (state == AnimalState.Eat)
        {
            StartCoroutine(Eat());
        }
        else if (state == AnimalState.Run)
        {
            StartCoroutine(Run());
        }
    }

    IEnumerator Idle()
    {
        int n = Random.Range(1,4);
        for(int i=0;i<n;i++){
            int ran = Random.Range(0,100);
            if(ran > 50)
                direction = Direction.L;
            else
                direction = Direction.R;
            anim.Play("Idle_"+direction.ToString(),0);
            yield return StartCoroutine(Wait(Random.Range(1,2)));
        }
        CheckAbort();
    }

    IEnumerator Eat()
    {
        int n = Random.Range(1,4);
        for(int i=0;i<n;i++){
            int ran = Random.Range(0,100);
            if(ran > 50)
                direction = Direction.L;
            else
                direction = Direction.R;
            yield return StartCoroutine(DoAnim("Eat_" + direction.ToString()));
        }
        CheckAbort();
    }

    IEnumerator Run()
    {
        Vector3 target = Minigame.instance.GetPointInBound();
        speed = Random.Range(maxSpeed/2,maxSpeed/1.5f);
        if(target.x > this.transform.position.x){
            SetDirection(Direction.R);
        }else
            SetDirection(Direction.L);
        anim.Play("Run_" + direction.ToString(),0);

        yield return StartCoroutine(MoveToPoint(target));
        CheckAbort();
    }
}
