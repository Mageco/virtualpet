using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharParrot : CharController
{
    public Vector3[] paths;


    protected override IEnumerator RunToPoint()
    {

        isMoving = true;
        isArrived = false;
        
        int ran = Random.Range(0, 100);
        if (ran < 30)
        {

            agent.SetDestination(target);
            while (!isArrived && !isAbort)
            {
                anim.Play("Walk_L" , 0);
                //data.Energy -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            if (shadow != null)
                shadow.SetActive(true);

            charInteract.interactType = InteractType.Fly;
            if (target.x > this.transform.position.x)
            {
                SetDirection(Direction.R);
            }
            else
                SetDirection(Direction.L);

            float h = Mathf.Clamp(Vector2.Distance(target, this.transform.position)/3, 2, 15);
            paths = new Vector3[3];
            paths[0] = this.transform.position;
            paths[0].z = 0;
            paths[1] = (this.transform.position + target) / 2 + new Vector3(0, h, 0);
            paths[1].z = 0;
            paths[2] = target;
            paths[2].z = 0;
            Vector3 startPoint = this.transform.position;
            //iTween.StopByName("RunToPoint" + this.gameObject.name);
            iTween.MoveTo(this.gameObject, iTween.Hash("name", "RunToPoint" + this.gameObject.name, "path", paths, "speed", data.Speed * 5, "orienttopath", false, "easetype", "linear", "oncomplete", "CompleteFly"));
            anim.Play("Run_L", 0);
            //Debug.Log("Start Fly " + data.Speed);
            while (charInteract.interactType == InteractType.Fly)
                    {
                //Debug.Log(this.transform.position);
                agent.transform.position = this.transform.position;
                float x0 = startPoint.x;
                float y0 = startPoint.y;
                float x2 = target.x;
                float y2 = target.y;
                float x1 = this.transform.position.x;
                float y1 = y0;
                if (Mathf.Abs(x2-x0) > 0.1f)
                    y1 = (x1 - x0) * (y2 - y0) / (x2 - x0) + y0;
                charScale.scalePosition = new Vector3(x1, y1, this.transform.position.z);
                charScale.height = this.transform.position.y - y1;
                //agent.transform.position = this.transform.position;
                yield return new WaitForEndOfFrame();
            }
        }

    }


    void CompleteFly()
    {
        //Debug.Log("Complete Fly " + data.Speed);
        isArrived = true;
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
                yield return DoAnim("Speak_L");
            }if(ran < 40){
                SetTarget(AreaType.All);
                yield return StartCoroutine(WalkToPoint());
            }else if (ran < 60)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else {
                anim.Play("Idle_L", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            
        CheckAbort();
    }
}

   