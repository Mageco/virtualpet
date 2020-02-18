using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCat : CharController
{
  

    protected override IEnumerator Mouse()
    {
        bool isLove = false;
        isMoving = true;
        charScale.speedFactor = 1.2f;
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            agent.SetDestination(GetMouse().transform.position);
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            data.Energy -= 1.5f*Time.deltaTime;
            if(Vector2.Distance(GetMouse().transform.position,this.transform.position) < 2){
                if(!isLove){
                    GetMouse().OnActive();
                    isLove = true;
                }
                    
            }
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
        charScale.speedFactor = 1;
        CheckAbort();
    }

    protected override IEnumerator Patrol()
    {
       int ran = Random.Range(0, 100);
        if(ran < 20){
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(RunToPoint());
        }else if (ran < 30)
        {
            anim.Play("Standby", 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }
        else if (ran < 40){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }else if(ran < 70)
        {
            SetTarget(PointType.Sunny);
            yield return StartCoroutine(RunToPoint());
            anim.Play("Sleep", 0);
            while (data.Sleep < data.MaxSleep && !isAbort)
            {
                data.Sleep += data.rateSleep * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else{
            if(GetRandomPoint(PointType.Table) != null)
            {
                SetTarget(PointType.Table);
                yield return StartCoroutine(RunToPoint());
                if (Vector2.Distance(this.transform.position, target) < 1)
                {
                    ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Table);
                    enviromentType = EnviromentType.Table;
                    yield return StartCoroutine(JumpUp(35, 0, col.transform.position + new Vector3(0, col.height - 0.5f, 0), col.height));
                    enviromentType = EnviromentType.Table;
                    CheckEnviroment();
                }
            }
        }
        CheckAbort();
    }

    protected override IEnumerator Table()
    {   
        int ran = Random.Range(0,100);
        if(ran < 30){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(5,15)));
            MageManager.instance.PlaySoundName(charType.ToString() + "_Speak",false);
            yield return StartCoroutine(DoAnim("Speak_" + direction.ToString()));
        }else if(ran < 60){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(5,15)));
        }else{
            anim.Play("Sleep", 0);
            while (data.Sleep < data.MaxSleep && !isAbort)
            {
                data.Sleep += data.rateSleep * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        if(!isAbort)
            yield return StartCoroutine(JumpDown(-2,10,30)); 
        CheckAbort();
    }

    

}
