using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMinigame1 : CharController
{
    protected override void CalculateData()
    {
        data.Sleep -= data.sleepConsume;
    }

    #region Thinking
    protected override void Think()
    {

 
    }

    protected override void DoAction()
    {
        //Debug.Log("DoAction " + actionType);
        isAbort = false;
        if (agent == null)
            return;
        agent.Stop();
        if (actionType == ActionType.Rest)
        {
            StartCoroutine(Rest());
        }
        else if (actionType == ActionType.Patrol)
        {
            StartCoroutine(Patrol());
        }

    }
    #endregion


    IEnumerator Patrol()
    {
        anim.Play("Lay_LD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }

        CheckAbort();
    }

    IEnumerator Rest()
    {
        anim.Play("Lay_LD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }

        CheckAbort();
    }    
}
