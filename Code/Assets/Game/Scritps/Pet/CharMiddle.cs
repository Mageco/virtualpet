using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMiddle : CharController
{
    protected override void CalculateData()
    {
        data.actionEnergyConsume = 0;
        if (actionType == ActionType.Call)
            data.actionEnergyConsume = 0.2f;
        else if (actionType == ActionType.Mouse)
            data.actionEnergyConsume = 0.7f;
        else if (actionType == ActionType.Discover)
        {
            data.actionEnergyConsume = 0.5f;
        }
        else if (actionType == ActionType.Patrol)
        {
            data.actionEnergyConsume = 0.3f;
        }
        else if (actionType == ActionType.Bath)
            data.actionEnergyConsume = 0.05f;
        else if (actionType == ActionType.Fear)
            data.actionEnergyConsume = 0.3f;

        data.Energy -= data.basicEnergyConsume + data.actionEnergyConsume;

        data.Happy -= data.happyConsume;
        if (actionType == ActionType.Call)
            data.Happy += 0.01f;

        if (data.Food > 0 && data.Water > 0)
        {
            float delta = 0.1f + data.Health * 0.001f + data.Happy * 0.001f;
            data.Food -= delta;
            data.Water -= delta;
            data.Energy += delta;
            data.Shit += delta;
            data.Pee += delta * 2;
        }

        data.Dirty += data.dirtyFactor;
        data.Itchi += data.Dirty * 0.001f;

        data.Sleep -= data.sleepConsume;

        float deltaHealth = data.healthConsume;

        deltaHealth += (data.Happy - data.maxHappy * 0.3f) * 0.001f;

        if (data.Dirty > data.maxDirty * 0.8f)
            deltaHealth -= (data.Dirty - data.maxDirty * 0.8f) * 0.003f;

        if (data.Pee > data.maxPee * 0.9f)
            deltaHealth -= (data.Pee - data.maxPee * 0.9f) * 0.001f;

        if (data.Shit > data.maxShit * 0.9f)
            deltaHealth -= (data.Shit - data.maxShit * 0.9f) * 0.002f;

        if (data.Food < data.maxFood * 0.1f)
            deltaHealth -= (data.maxFood * 0.1f - data.Food) * 0.001f;

        if (data.Water < data.maxWater * 0.1f)
            deltaHealth -= (data.maxWater * 0.1f - data.Water) * 0.001f;

        if (data.Sleep < data.maxSleep * 0.05f)
            deltaHealth -= (data.maxSleep * 0.05f - data.Sleep) * 0.004f;

        data.Health += deltaHealth;

        data.curious += 0.1f;
    }


    #region Thinking
    protected override void Think()
    {

        if (data.Shit > data.maxShit * 0.9f)
        {
            actionType = ActionType.Shit;
            return;
        }

        if (data.Pee > data.maxPee * 0.9f)
        {
            actionType = ActionType.Pee;
            return;
        }


        if (data.Food < data.maxFood * 0.2f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Eat;
                return;
            }
        }

        if (data.Water < data.maxWater * 0.2f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Drink;
                return;
            }
        }

        if (data.Sleep < data.maxSleep * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        else if (data.Energy < data.maxEnergy * 0.3f)
        {
            actionType = ActionType.Rest;
            return;
        }


        //Other Action
        int id = Random.Range(0, 100);
        if (id < 30)
        {
            actionType = ActionType.Rest;
        }
        else
        {
            actionType = ActionType.Patrol;
        }


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
        else if (actionType == ActionType.Pee)
        {
            StartCoroutine(Pee());
        }
        else if (actionType == ActionType.Shit)
        {
            StartCoroutine(Shit());
        }
        else if (actionType == ActionType.Eat)
        {
            StartCoroutine(Eat());
        }
        else if (actionType == ActionType.Drink)
        {
            StartCoroutine(Drink());
        }
        else if (actionType == ActionType.Sleep)
        {
            StartCoroutine(Sleep());
        }
        else if (actionType == ActionType.Hold)
        {
            StartCoroutine(Hold());
        }
        else if (actionType == ActionType.Bath)
        {
            StartCoroutine(Bath());
        }
        else if (actionType == ActionType.OnTable)
        {
            StartCoroutine(Table());
        }
        else if (actionType == ActionType.Call)
        {
            StartCoroutine(Call());
        }
        else if (actionType == ActionType.Listening)
        {
            StartCoroutine(Listening());
        }
        else if (actionType == ActionType.LevelUp)
        {
            StartCoroutine(LevelUp());
        } else if (actionType == ActionType.OnBed)
        {
            StartCoroutine(Bed());
        }else if (actionType == ActionType.OnToilet)
        {
            StartCoroutine(Toilet());
        }
    }
    #endregion




    #region Main Action

    

    IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(MoveToPoint());
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                yield return StartCoroutine(DoAnim("Smell_" + this.direction.ToString()));
            }
            else
            {
                anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 3)));
            }
            n++;
        }
        CheckAbort();
    }

    IEnumerator Table()
    {

        SetDirection(Direction.D);
        anim.Play("Idle_D", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        
        CheckAbort();
    }
    
    IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Listen_D"));

        if (!isAbort)
        {
            yield return StartCoroutine(MoveToPoint());
        }

        float t = 0;
        float maxTime = 6f;
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Call);

        while (!isAbort && t < maxTime)
        {
            anim.Play("Idle_" +direction.ToString());
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Listening()
    {
        yield return StartCoroutine(DoAnim("Listen_D" ));
        CheckAbort();
    }

    IEnumerator Rest()
    {
        float maxTime = Random.Range(2, 5);
        if (!isAbort)
        {
            SetDirection(Direction.D);
            anim.Play("Idle_"+direction.ToString(), 0);
        }

        Debug.Log("Rest");
        yield return StartCoroutine(Wait(maxTime));
        CheckAbort();
    }

    
    IEnumerator Sick()
    {
        if (this.direction == Direction.RD || this.direction == Direction.RU)
            anim.Play("Lay_Sick_RD", 0);
        else
            anim.Play("Lay_Sick_LD", 0);
        Debug.Log("Sick");
        while (data.health < 0.1f * data.maxHealth && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        CheckAbort();
    }

    IEnumerator Fall()
    {
        if (this.direction == Direction.RD || this.direction == Direction.RU)
            yield return StartCoroutine(DoAnim("Fall_Water_R"));
        else
            yield return StartCoroutine(DoAnim("Fall_Water_L"));

        CheckAbort();
    }

    protected override IEnumerator LevelUp()
    {
        Debug.Log("Level Up" + data.level);
        yield return StartCoroutine(DoAnim("LevelUp_LD"));

        if (data.level >= 6){
            GrowUp();
            data.Load();
        }
        CheckAbort();
    }




    #endregion
}
