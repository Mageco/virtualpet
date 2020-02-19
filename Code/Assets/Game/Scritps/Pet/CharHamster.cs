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
            if(ran < 30){
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(RunToPoint());
            }else if (ran < 50)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 5)));
            }
            else if(ran < 70)
            {
                SetTarget(PointType.ChickenDefence);
                yield return StartCoroutine(RunToPoint());
                ChickenController chicken = FindObjectOfType<ChickenController>();
                bool isSpeak = false;
                while(chicken != null && data.Energy > data.MaxEnergy * 0.1f && !isAbort)
                {
                    agent.SetDestination(chicken.transform.position);
                    anim.Play("Run_Angry_" + this.direction.ToString(), 0);
                    data.Energy -= 1.5f * Time.deltaTime;
                    if (Vector2.Distance(this.transform.position, chicken.transform.position) < 2 && !isSpeak)
                    {
                        chicken.OnFlee();
                        int r = Random.Range(0, 100);
                        if(r < 30)
                        {
                            yield return StartCoroutine(DoAnim("Love"));
                            MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
                            ItemManager.instance.SpawnHeart((1+data.level/5), this.transform.position);
                        }
                            
                        isSpeak = true;
                        
                    }
                    if(Vector2.Distance(this.transform.position, chicken.transform.position) > 3 && isSpeak)
                    {
                        isSpeak = false;
                    }
                    yield return new WaitForEndOfFrame();
                }
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
