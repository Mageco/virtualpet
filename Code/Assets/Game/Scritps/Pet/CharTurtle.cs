using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTurtle : CharController
{
    
    public override void OnMouse()
    {

    }

     protected override IEnumerator Supprised(){
        anim.Play("Teased",0);
        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator Fall()
    {
        anim.Play("Fall_" + direction.ToString(),0);
        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator Patrol()
    {
        int ran = Random.Range(0, 100);
        if (ran < 10)
        {
            anim.Play("Standby", 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }
        else if (ran < 20)
        {
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }
        else if (ran < 70 && GameManager.instance.myPlayer.questId > 19)
        {
            SetTarget(PointType.Banana);
            yield return StartCoroutine(RunToPoint());
            anim.Play("Sleep", 0);
            while (data.Sleep < data.MaxSleep && !isAbort)
            {
                data.Sleep += data.rateSleep * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameManager.instance.AddExp(5, data.iD);
        }
        else 
        {
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(RunToPoint());
        }
       

            CheckAbort();
    }

}
