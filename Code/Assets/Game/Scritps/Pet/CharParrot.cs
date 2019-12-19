using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharParrot : CharController
{
    float targetHeight = 0;
    protected override IEnumerator Mouse()
    {
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            LookToTarget(GetMouse().transform.position);
            anim.Play("Idle_" + this.direction.ToString(), 0);
            if(Vector2.Distance(GetMouse().transform.position,this.transform.position) < 5){
                OnSupprised();
            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator RunToPoint()
    {
        isMoving = true;
        isArrived = false;

        int ran = Random.Range(0,100);
        if(ran < 50){
            
            agent.SetDestination(target);
            while (!isArrived && !isAbort)
            {
                anim.Play("Walk_" + this.direction.ToString(), 0);
                data.Energy -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

        }else{
            float length = Vector2.Distance(this.transform.position,target);
            float t = 0;
            charInteract.interactType = InteractType.Fly;
            if(target.x > this.transform.position.x){
                SetDirection(Direction.R);
            }else
                SetDirection(Direction.L);
        
            while (!isArrived && !isAbort)
            {
                if(t < 0.01f * (length + targetHeight)){
                    charScale.height = Mathf.Lerp(charScale.height,targetHeight + 20,Time.deltaTime*5);
                }else{
                    charScale.height = Mathf.Lerp(charScale.height,targetHeight,Time.deltaTime*4);
                    
                }
                if(charScale.height > 0.5f)
                    anim.Play("Run_" + this.direction.ToString(), 0);
                else
                    anim.Play("Idle_" + this.direction.ToString(), 0);

                data.Energy -= 0.5f*Time.deltaTime;
                
                charScale.scalePosition = Vector3.Lerp(charScale.scalePosition,target,Time.deltaTime*2);

                if(charScale.height < 0)
                    charScale.height = 0;
                Vector3 p = charScale.scalePosition + new Vector3(0,charScale.height,0);
                this.transform.position = p;
                agent.transform.position = p;



                if(Vector2.Distance(charScale.scalePosition,target) < 2 && Mathf.Abs(charScale.height - targetHeight) < 0.5f){
                    charScale.height = targetHeight;
                    charScale.scalePosition = target;
                    
                    this.transform.position = target + new Vector3(0,targetHeight,0);
                    agent.transform.position = target + new Vector3(0,targetHeight,0);
                    charInteract.interactType = InteractType.None;
                    isArrived = true;
                }
                t+=Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        //
       
        
       

        isMoving = false;
    }

    protected override IEnumerator Patrol()
    {
        //int n = 0;
        //int maxCount = Random.Range(2, 5);

            int ran = Random.Range(0, 100);
            if(ran < 20){
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(RunToPoint());
            }if(ran < 40){
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(WalkToPoint());
            }else if (ran < 60)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else {
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }/* else{
                SetTarget(PointType.Patrol);
                targetHeight = 100;
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(Wait(Random.Range(10,20)));
                SetTarget(PointType.Patrol);
                targetHeight = 0;
                yield return StartCoroutine(RunToPoint());
            } */
            
        CheckAbort();
    }
}
