using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharBunny : CharController
{


    protected override IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if(ran < 40){
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(RunToPoint());
            }else if (ran < 60)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else{
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            
            n++;
        }
        CheckAbort();
    }
}
