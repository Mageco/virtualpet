using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSmall : CharController
{
    protected override void CalculateData()
    {
        data.Sleep -= data.sleepConsume;
        data.Food -= 0.1f;
        data.energy += 0.1f;
    }

    #region Thinking
    protected override void Think()
    {
        if (data.Sleep < data.maxSleep * 0.1f)
        {
            actionType = ActionType.Sleep;
            return;
        }else
        //Other Action
            actionType = ActionType.Rest;
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
        else if (actionType == ActionType.Sleep)
        {
            StartCoroutine(Sleep());
        }
        else if (actionType == ActionType.Hold)
        {
            StartCoroutine(Hold());
        }
        else if (actionType == ActionType.OnTable)
        {
            StartCoroutine(Table());
        }
        else if(actionType == ActionType.LevelUp){
            StartCoroutine(LevelUp());
        }else if(actionType == ActionType.OnBed){
            StartCoroutine(Bed());
        }else if(actionType == ActionType.Eat){
            StartCoroutine(Eat());
        }
    }
    #endregion


    public override void OnEat(){
        actionType = ActionType.Eat;
        isAbort = true;
    }

    public override void OnMouse(){
        
    }

    public override void OnListening(float sound)
    {

    }

    public override void OnCall(Vector3 pos)
    {

    }


    #region Main Action

    

    IEnumerator Table()
    {
        anim.Play("Lay_LD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }

        CheckAbort();
    }



    protected override IEnumerator Eat()
    {
        anim.Play("Eat_LD", 0);
        while (!isAbort && data.Food < 0.95f * data.maxFood)
        {
            data.Food += 0.2f;
            data.Sleep -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.AddExp(5);
        CheckAbort();
    }

   
   protected override IEnumerator Sleep()
    {
        anim.Play("Sleep_LD" , 0);
        
        while (!isAbort && data.Sleep < 0.9f * data.maxSleep)
        {
            data.Sleep += 0.1f;
            data.Food -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Rest()
    {
        float maxTime = Random.Range(2, 5);
        if (!isAbort)
        {
            int ran = Random.Range(0, 100);
            if (ran < 50)
            {
                anim.Play("Idle_LD", 0);
            }
            else 
            {
                anim.Play("Lay_LD", 0);
            }
        }

        //Debug.Log("Rest");
        yield return StartCoroutine(Wait(maxTime));
        CheckAbort();
    }

    protected override IEnumerator Bed()
    {
        if(data.Food > data.maxFood * 0.9f || data.Sleep < data.maxSleep * 0.5f)
        {
            anim.Play("Sleep_LD", 0);
            while(!isAbort && data.sleep < 0.9f*data.maxSleep){
                data.Sleep += 0.2f;
                data.Food -= 0.2f;
                yield return new WaitForEndOfFrame();
            }
            GameManager.instance.AddExp(5);
        }else
        {
            anim.Play("Lay_LD", 0);
            while(!isAbort){
                yield return new WaitForEndOfFrame();
            }
        }
        CheckAbort();
    }

    protected override IEnumerator LevelUp()
    {
        Debug.Log("Level Up" + data.level);
        yield return StartCoroutine(DoAnim("LevelUp_LD"));

        if (data.level >= 3){
            GrowUp();
            data.Load();
        }
           

        CheckAbort();

    }

    #endregion
}
