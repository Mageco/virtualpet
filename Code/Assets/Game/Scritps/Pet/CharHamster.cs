using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharHamster : CharController
{
    
    protected override IEnumerator Mouse()
    {
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            LookToTarget(GetMouse().transform.position);
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if(ran < 60){
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(RunToPoint());
            }else if (ran < 80)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 5)));
            }
            else{
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 5)));
            }
            
            n++;
        }
        CheckAbort();
    }
}
