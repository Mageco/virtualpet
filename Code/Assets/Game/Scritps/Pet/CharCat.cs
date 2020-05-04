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
            if(Vector2.Distance(GetMouse().transform.position,this.transform.position) < 4){
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
            SetTarget(AreaType.All);
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
            SetTarget(AreaType.Room);
            yield return StartCoroutine(RunToPoint());
            anim.Play("Sleep", 0);
            while (data.Sleep < data.MaxSleep && !isAbort)
            {
                data.Sleep += 1f * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else{
            BaseFloorItem item = ItemManager.instance.GetRandomItem(ItemType.Table);
            if (item != null)
            {
                target = item.startPoint.position;
                yield return StartCoroutine(RunToPoint());
                if (Vector2.Distance(this.transform.position, target) < 1)
                {
                    equipment = item;
                    yield return StartCoroutine(JumpIn());
                    CheckEnviroment();
                }
            }
        }
        CheckAbort();
    }


    

}
