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
            if(ran1 < 30){
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


    protected override IEnumerator Table()
    {           
        anim.Play("Standby", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator Discover()
    {
        if (data.curious > data.MaxCurious * 0.4f)
        {
            int ran = Random.Range(0,100);
            if(ran < 30){
                SetTarget(PointType.MouseGate);
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(DoAnim("Smell_" + direction.ToString())) ;
                yield return StartCoroutine(DoAnim("Smell_Bark"));
                data.curious -= 10;
            }else
            {
                int ran1 = Random.Range(0,100);
                Vector3 t = GetRandomPoint(PointType.Patrol).position;
                if(ran1 < 5 && GetRandomPoint(PointType.Eat) != null){
                    t = GetRandomPoint(PointType.Eat).position;
                }else if(ran1 < 10 && GetRandomPoint(PointType.Drink) != null){
                    t = GetRandomPoint(PointType.Drink).position;                    
                }else if(ran1 < 15 && GetRandomPoint(PointType.Toilet) != null){
                    t = GetRandomPoint(PointType.Toilet).position;                    
                }else if(ran1 < 20 && GetRandomPoint(PointType.Sleep) != null){
                    t = GetRandomPoint(PointType.Sleep).position;                    
                }else if(ran1 < 25 && GetRandomPoint(PointType.Bath) != null){
                    t = GetRandomPoint(PointType.Bath).position;                    
                }else if(ran1 < 30 && GetRandomPoint(PointType.Table) != null){
                    t = GetRandomPoint(PointType.Table).position;                    
                }else if(ran1 < 35 && GetRandomPoint(PointType.Cleaner) != null){
                    t = GetRandomPoint(PointType.Cleaner).position;                    
                }else if(ran1 < 40 && GetRandomPoint(PointType.Caress) != null){
                    t = GetRandomPoint(PointType.Caress).position;                    
                }else if(ran1 < 45 && GetRandomPoint(PointType.Window) != null){
                    t = GetRandomPoint(PointType.Window).position;                    
                }

                target = t;
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(DoAnim("Smell_" + direction.ToString()));
                yield return StartCoroutine(DoAnim("Smell_Bark"));
            }

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
