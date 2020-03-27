using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShiba : CharController
{
    #region Main Action

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
