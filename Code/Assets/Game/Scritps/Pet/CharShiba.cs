using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShiba : CharController
{
    #region Main Action

    
    protected override IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        int ran1 = Random.Range(0,100);
        


        while (!isAbort && n < maxCount)
        {

            SetTarget(PointType.Patrol);
            if(ran1 < 20){
                yield return StartCoroutine(WalkToPoint());
            }else{
                yield return StartCoroutine(RunToPoint());
            }
                
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                
                anim.Play("Standby", 0);
            }
            else
                anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 3)));
            n++;
        }
        CheckAbort();
    }



    protected override IEnumerator Itchi()
    {

        int i = Random.Range(0, 3);

        if (i == 0)
        {
            anim.Play("Itching1", 0);
        }
        else if (i == 1)
        {
            anim.Play("Itching2" , 0);
        }
        else
        {
            anim.Play("Itching3" , 0);
        }

        Debug.Log("Itchi");
        while (data.itchi > 0.5 * data.MaxItchi && !isAbort)
        {
            data.itchi -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }



    #endregion

}
