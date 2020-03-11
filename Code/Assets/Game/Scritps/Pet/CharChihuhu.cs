using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharChihuhu : CharController
{
    protected override IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                SetTarget(PointType.Patrol);
                yield return StartCoroutine(RunToPoint());
            }
            else if (ran < 50)
            {
                anim.Play("Standby", 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else if (ran < 60)
            {
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 10)));
            }
            else if (ran < 80)
            {
                charInteract.interactType = InteractType.Touch;
                yield return DoAnim("Special");
                charInteract.interactType = InteractType.None;
            }
            else
            {
                MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
                yield return DoAnim("Speak_" + direction.ToString());
            }

            n++;
        }
        CheckAbort();
    }
}
