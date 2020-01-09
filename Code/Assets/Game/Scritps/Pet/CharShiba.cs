using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShiba : CharController
{
    #region Main Action

    protected override void Think()
    {
        if (charInteract.interactType != InteractType.None)
            return;

        if (data.Damage > data.MaxDamage * 0.8f)
        {
            actionType = ActionType.Injured;
            return;
        }

        if (data.Health < data.MaxHealth * 0.1f)
        {
            actionType = ActionType.Sick;
            return;
        }

        if (data.Shit > data.MaxShit * 0.9f)
        {
            actionType = ActionType.Shit;
            return;
        }

        if (data.Pee > data.MaxPee * 0.9f)
        {
            actionType = ActionType.Pee;
            return;
        }



        if (data.Food < data.MaxFood * 0.3f && GetFoodItem() != null && Vector2.Distance(this.transform.position, GetFoodItem().transform.position) < 3)
        {
            actionType = ActionType.Eat;
            return;
        }

        if (data.Water < data.MaxWater * 0.3f && GetDrinkItem() != null && Vector2.Distance(this.transform.position, GetDrinkItem().transform.position) < 3)
        {
            actionType = ActionType.Drink;
            return;
        }

        if (data.Food < data.MaxFood * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 0)
            {
                actionType = ActionType.Eat;
                return;
            }
        }


        if (data.Water < data.MaxWater * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 0)
            {
                actionType = ActionType.Drink;
                return;
            }
        }



        if (data.Sleep < data.MaxSleep * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        if (data.Itchi > data.MaxItchi * 0.7f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 50)
            {
                actionType = ActionType.Itchi;
                return;
            }
        }

        if (data.Energy < data.MaxEnergy * 0.1f)
        {
            actionType = ActionType.Tired;
            return;
        }


        if (data.curious > data.MaxCurious * 0.9f)
        {
            actionType = ActionType.Discover;
            return;
        }

        if (GameManager.instance.myPlayer.gameType == GameType.Garden)
        {
            actionType = ActionType.OnGarden;
        }
        else if (GameManager.instance.myPlayer.gameType == GameType.House)
        {
            int r = Random.Range(0, 100);
            if (r < 20)
                actionType = ActionType.Patrol;
            else
                actionType = ActionType.OnCall;
        }
    }

    protected override void Start()
    {
        if(actionType != ActionType.Sick && actionType != ActionType.Injured && enviromentType == EnviromentType.Room)
            actionType = ActionType.OnCall;
        base.Start();
    }

    protected override IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        int ran1 = Random.Range(0,100);
        


        while (!isAbort && n < maxCount)
        {

            SetTarget(PointType.Patrol);
            if(ran1 < 20){
                yield return StartCoroutine(WalkToPoint());
            }else{
                yield return StartCoroutine(RunToPoint());
            }
                
            int ran = Random.Range(0, 100);
            if (ran < 30)
            {
                
                anim.Play("Standby", 0);
            }
            else
                anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 3)));
            n++;
        }
        CheckAbort();
    }


    protected override IEnumerator Table()
    {           
        anim.Play("Standby", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator Discover()
    {
        if (data.curious > data.MaxCurious * 0.4f)
        {
            int ran = Random.Range(0,100);
            if(ran < 30){
                SetTarget(PointType.MouseGate);
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(DoAnim("Smell_" + direction.ToString())) ;
                yield return StartCoroutine(DoAnim("Smell_Bark"));
                data.curious -= 10;
            }else
            {
                if(GameManager.instance.myPlayer.questId > 19)
                {
                    data.curious -= 50;
                    OnGarden();
                }
            }
        }
        CheckAbort();
    }



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

    protected override IEnumerator Call()
    {
        int ran = Random.Range(3,5);
        SetTarget(PointType.Call);
        yield return StartCoroutine(RunToPoint());
        int n = 0;
        while(n < ran && !isAbort)
        {
            int r = Random.Range(0, 100);
            if(r < 30)
                yield return StartCoroutine(DoAnim("Standby"));
            else if (r < 50)
                yield return StartCoroutine(DoAnim("Sit"));
            else if (r < 70)
            {
                MageManager.instance.PlaySoundName(charType.ToString() + "_Speak", false);
                yield return DoAnim("Speak_" + direction.ToString());
            }else
                yield return StartCoroutine(DoAnim("Love"));

            n++;
        }
        
        CheckAbort();
    }


    #endregion

}
