using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCat : CharController
{
  

    protected override IEnumerator Patrol()
    {
       int ran = Random.Range(0, 100);
        if(ran < 30){
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(RunToPoint());
        }else if (ran < 50)
        {
            anim.Play("Standby", 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }
        else if (ran < 70){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }else{ 
            SetTarget(PointType.Table);
            yield return StartCoroutine(RunToPoint());
            if(Vector2.Distance(this.transform.position,target) < 1){
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Table);
                enviromentType = EnviromentType.Table;
                yield return StartCoroutine(JumpUp(35,0,col.transform.position + new Vector3(0,col.height-0.5f,0),col.height));
                enviromentType = EnviromentType.Table;
                CheckEnviroment();
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
        }else if(ran < 60){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(5,15)));
        }else{
            anim.Play("Sleep", 0);
            while (data.Sleep < data.maxSleep && !isAbort)
            {
                data.Sleep += data.rateSleep * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameManager.instance.AddExp(5,data.iD);
        }
        yield return StartCoroutine(JumpDown(-2,10,30)); 
        CheckAbort();
    }
}
