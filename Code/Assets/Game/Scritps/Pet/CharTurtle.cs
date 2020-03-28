using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTurtle : CharController
{
     protected override IEnumerator Supprised(){
        anim.Play("Teased",0);
        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }
        ItemManager.instance.SpawnHeart(data.rateHappy, this.transform.position);
        CheckAbort();
    }

    protected override IEnumerator Fall()
    {
        anim.Play("Fall_" + direction.ToString(),0);
        while(!isAbort){
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }
}
