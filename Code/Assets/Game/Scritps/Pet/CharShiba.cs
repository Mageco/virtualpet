using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharShiba : CharController
{
    #region Main Action

    protected override IEnumerator Eat()
    {
        if (GetFoodItem() != null)
        {
            SetTarget(PointType.Eat);
            yield return StartCoroutine(RunToPoint());
            bool canEat = true;
            if (GetFoodItem().CanEat())
            {
                anim.Play("Eat", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Food < data.maxFood && !isAbort && canEat)
                {
                    data.Food += 0.3f;
                    GetFoodItem().Eat(0.3f);
                    if (!GetFoodItem().CanEat())
                    {
                        canEat = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetFoodItem().anchor.position) > 1f)
                        canEat = false;
                    yield return new WaitForEndOfFrame();
                }
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Eat);
                GameManager.instance.AddExp(5,data.iD);
                if(GetFoodItem() != null && GetFoodItem().GetComponent<ItemObject>() != null)
 			        GameManager.instance.LogAchivement(AchivementType.Eat,ActionType.None,GetFoodItem().GetComponent<ItemObject>().itemID);

            }else{
                int ran = Random.Range(0,100);
                if(ran < 20)
                    yield return DoAnim("Bark");
                else if(ran < 40)
                {
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(RunToPoint());
                    yield return DoAnim("Bark");
                    SetTarget(PointType.Eat);
                    yield return StartCoroutine(RunToPoint());
                }else if(ran < 50)
                    yield return DoAnim("Bark_Sit");
                else if(ran < 70) 
                    yield return DoAnim("Standby");
                else if(ran < 80){
                    yield return DoAnim("Eat");
                }else if(ran < 90){
                    yield return DoAnim("Smell_" + direction.ToString());
                }else{
                    yield return DoAnim("Smell_Bark");
                }
            }
        }

        yield return new WaitForEndOfFrame();
        CheckAbort();
    }

    protected override IEnumerator Drink()
    {
        if (GetDrinkItem() != null)
        {
            //Debug.Log("Drink");

            SetTarget(PointType.Drink);
            yield return StartCoroutine(RunToPoint());
            

            bool canDrink = true;

            if (GetDrinkItem().CanEat())
            {
                
                anim.Play("Drink", 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Water < data.maxWater && !isAbort && canDrink)
                {
                    data.Water += 0.5f;
                    GetDrinkItem().Eat(0.5f);
                    if (!GetDrinkItem().CanEat())
                    {
                        canDrink = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetDrinkItem().anchor.position) > 1f)
                        canDrink = false;
                    yield return new WaitForEndOfFrame();
                }
                GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Drink);
                GameManager.instance.AddExp(5,data.iD);
                if(GetDrinkItem() != null && GetDrinkItem().GetComponent<ItemObject>() != null)
 			        GameManager.instance.LogAchivement(AchivementType.Drink,ActionType.None,GetDrinkItem().GetComponent<ItemObject>().itemID);
            }else{
                int ran = Random.Range(0,100);
                if(ran < 20)
                    yield return DoAnim("Bark");
                else if(ran < 40)
                {
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(RunToPoint());
                    yield return DoAnim("Bark");
                    SetTarget(PointType.Eat);
                    yield return StartCoroutine(RunToPoint());
                }else if(ran < 50)
                    yield return DoAnim("Bark_Sit");
                else if(ran < 70) 
                    yield return DoAnim("Standby");
                else if(ran < 80){
                    yield return DoAnim("Eat");
                }else if(ran < 90){
                    yield return DoAnim("Smell_" + direction.ToString());
                }else{
                    yield return DoAnim("Smell_Bark");
                }

            }
        }
        CheckAbort();
    }

    protected override IEnumerator Patrol()
    {
        int n = 0;
        int maxCount = Random.Range(2, 5);
        int ran1 = Random.Range(0,100);
        


        while (!isAbort && n < maxCount)
        {

            SetTarget(PointType.Patrol);
            if(ran1 < 30){
                yield return StartCoroutine(WalkToPoint());
                charScale.speedFactor = 0.4f;
            }else{
                yield return StartCoroutine(RunToPoint());
                charScale.speedFactor = 1f;
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
        if (data.curious > data.maxCurious * 0.4f)
        {
            int ran = Random.Range(0,100);
            if(ran < 30){
                SetTarget(PointType.MouseGate);
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(DoAnim("Smell_" + direction.ToString())) ;
                yield return StartCoroutine(DoAnim("Smell_Bark"));
                data.curious -= 10;
            }else if(ran < 50)
            {
                SetTarget(PointType.Door);
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(DoAnim("Dig_" + direction.ToString())) ;
                data.curious -= 10;                
            }else
            {
                int ran1 = Random.Range(0,100);
                Vector3 t = GetRandomPoint(PointType.Patrol).position;
                if(ran1 < 5 && GetRandomPoint(PointType.Eat) != null){
                    t = GetRandomPoint(PointType.Eat).position;
                }else if(ran1 < 10 && GetRandomPoint(PointType.Drink) != null){
                    t = GetRandomPoint(PointType.Drink).position;                    
                }else if(ran1 < 15 && GetRandomPoint(PointType.Toilet) != null){
                    t = GetRandomPoint(PointType.Toilet).position;                    
                }else if(ran1 < 20 && GetRandomPoint(PointType.Sleep) != null){
                    t = GetRandomPoint(PointType.Sleep).position;                    
                }else if(ran1 < 25 && GetRandomPoint(PointType.Bath) != null){
                    t = GetRandomPoint(PointType.Bath).position;                    
                }else if(ran1 < 30 && GetRandomPoint(PointType.Table) != null){
                    t = GetRandomPoint(PointType.Table).position;                    
                }else if(ran1 < 35 && GetRandomPoint(PointType.Cleaner) != null){
                    t = GetRandomPoint(PointType.Cleaner).position;                    
                }else if(ran1 < 40 && GetRandomPoint(PointType.Caress) != null){
                    t = GetRandomPoint(PointType.Caress).position;                    
                }else if(ran1 < 45 && GetRandomPoint(PointType.Window) != null){
                    t = GetRandomPoint(PointType.Window).position;                    
                }

                target = t;
                yield return StartCoroutine(RunToPoint());
                yield return StartCoroutine(DoAnim("Smell_" + direction.ToString()));
                yield return StartCoroutine(DoAnim("Smell_Bark"));
            }

        }
        CheckAbort();
    }


    

    protected override IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Listen_"+direction.ToString()));
        yield return StartCoroutine(RunToPoint());
        touchObject.SetActive(true);
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Call);
        float t = 0;
        float maxTime = 6f;
        bool isWait = true;
        int n = Random.Range(0,(int)data.Intelligent/10);

        while (!isAbort && isWait)
        {
            if (!isTouch)
            {
                if (t == 0)
                {
                    n = Random.Range(0,(int)data.Intelligent/10);
                }
                if (n == 0)
                {
                     anim.Play("Lay_WaitPetBack_" + direction.ToString(), 0);
                }
                else if (n == 1)
                {
                    anim.Play("Lay_WaitPetHead_"+ direction.ToString(), 0);
                }
                else if (n == 2)
                {
                    anim.Play("Lay_WaitPetStomatch_"+ direction.ToString(), 0);
                }
                else if (n == 3)
                {  
                    anim.Play("Sit_WaitPetUp" );
                }
                else if (n == 4)
                {
                    anim.Play("Sit_WaitPetDown" );
                }
                else if (n == 5)
                {
                     
                    anim.Play("Sit_WaitPetLeft" );
                }
                else if (n == 6)
                {
                    anim.Play("Sit_WaitPetRight" );
                }
                else if (n == 7)
                {
                    anim.Play("WaitHandshake" );
                }
                else if (n == 8)
                {
                    anim.Play("WaitJumpRound" );
                }
                t += Time.deltaTime;
                if (t > maxTime)
                {
                    isWait = false;
                    n = Random.Range(0, 9);
                    maxTime = Random.Range(2f, 5f);
                }
            }
            else
            {
                if (n == 0)
                {
                    anim.Play("Lay_PetBack_"+ direction.ToString(), 0);
                }
                else if (n == 1)
                {
                    anim.Play("Lay_PetHead_"+ direction.ToString(), 0);
                }
                else if (n == 2)
                {
                    anim.Play("Lay_PetStomatch_"+ direction.ToString(), 0);
                }
                else if (n == 3)
                {
                    anim.Play("Sit_Pet_Up" , 0);
                }
                else if (n == 4)
                {
                    anim.Play("Sit_Pet_Down" , 0);
                }
                else if (n == 5)
                {
                    anim.Play("Sit_Pet_Right" , 0);
                }
                else if (n == 6)
                {
                    anim.Play("Sit_Pet_Left" , 0);
                }
                else if (n == 7)
                {
                    anim.Play("Handshake" , 0);
                }
                else if (n == 8)
                {
                    anim.Play("JumpRound" , 0);
                }
                t = 0;


            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator Listening(){
        yield return StartCoroutine(DoAnim("Listen_" + direction.ToString()));
        CheckAbort();
    }

    
    protected override IEnumerator Rest()
    {
        float maxTime = Random.Range(2, 5);

        int ran = Random.Range(0, 100);
        if (ran < 50)
        {
            
            anim.Play("Sit", 0);
        }
        else
        {
            anim.Play("Lay_"+ direction.ToString(), 0);
        }
        

        Debug.Log("Rest");
        
        while(data.Food > 0 && data.Water > 0 && data.Sleep > 0 &&data.Energy < 0.5f * data.maxEnergy && !isAbort){
            data.Energy += 0.05f;
            data.Food -= 0.03f;
            data.Water -= 0.03f;
            data.Sleep -= 0.01f;
            yield return new WaitForEndOfFrame();
        }  
        yield return StartCoroutine(Wait(maxTime));
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
        while (data.itchi > 0.5 * data.maxItchi && !isAbort)
        {
            data.itchi -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    
    #endregion

}
