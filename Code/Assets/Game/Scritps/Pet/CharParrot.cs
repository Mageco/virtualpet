using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharParrot : CharController
{
    Vector3[] paths;


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
            charInteract.interactType = InteractType.Fly;
            if (target.x > this.transform.position.x)
            {
                SetDirection(Direction.R);
            }
            else
                SetDirection(Direction.L);
            paths = new Vector3[3];
            paths[0] = this.transform.position;
            paths[1] = (this.transform.position + target) / 2 + new Vector3(0, Random.Range(10,20), 0);
            paths[2] = target;
            Vector3 startPoint = this.transform.position;
            //iTween.StopByName("Parrot" + this.gameObject.name);
            iTween.MoveTo(this.gameObject, iTween.Hash("name", "Parrot" + this.gameObject.name, "path", paths, "speed", data.Speed * 5, "orienttopath", false, "easetype", "linear", "oncomplete", "CompleteFly"));
            anim.Play("Run_" + this.direction.ToString(),0);
            while (charInteract.interactType == InteractType.Fly)
            {
                float x0 = startPoint.x;
                float y0 = startPoint.y;
                float x2 = target.x;
                float y2 = target.y;
                float x1 = this.transform.position.x;
                float y1 = (x1 - x0)*(y2 - y0) / (x2 - x0) + y0;
                charScale.scalePosition = new Vector3(x1, y1, this.transform.position.z);
                charScale.height = this.transform.position.y - y1;
                agent.transform.position = this.transform.position;
                yield return new WaitForEndOfFrame();
            }

        }
    }


    void CompleteFly()
    {
        charInteract.interactType = InteractType.None;
        isMoving = false;
    }

    protected override IEnumerator Patrol()
    {
        //int n = 0;
        //int maxCount = Random.Range(2, 5);

            int ran = Random.Range(0, 100);
            if(ran < 20){
                SetTarget(AreaType.All);
                yield return StartCoroutine(RunToPoint());
                MageManager.instance.PlaySound3D(charType.ToString() + "_Speak",false,this.transform.position);
                yield return DoAnim("Speak_" + direction.ToString());
            }if(ran < 40){
                SetTarget(AreaType.All);
                yield return StartCoroutine(WalkToPoint());
            }else if (ran < 60)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else {
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            
        CheckAbort();
    }
}

    /*
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
                    
                    isArrived = true;
                }
                t+=Time.deltaTime;
                yield return new WaitForEndOfFrame();
                                                     */