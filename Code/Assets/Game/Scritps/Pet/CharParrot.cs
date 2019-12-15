using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharParrot : CharController
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
}
