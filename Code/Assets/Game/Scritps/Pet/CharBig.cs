using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharBig : CharController
{
    protected override void CalculateData()
    {
        data.actionEnergyConsume = 0;
        if (actionType == ActionType.Call)
            data.actionEnergyConsume = 0.2f;
        else if (actionType == ActionType.Mouse)
            data.actionEnergyConsume = 1.5f;
        else if (actionType == ActionType.Discover)
        {
            data.actionEnergyConsume = 1f;
        }
        else if (actionType == ActionType.Patrol)
        {
            data.actionEnergyConsume = 0.6f;
        }else if (actionType == ActionType.Bath)
            data.actionEnergyConsume = 0.1f;
        else if (actionType == ActionType.Fear)
            data.actionEnergyConsume = 0.3f;
        else if(actionType == ActionType.Rest || actionType == ActionType.OnBed 
        || actionType == ActionType.Sleep || actionType == ActionType.OnTable){
            if(data.Food > 2 && data.Water > 2){
                data.Energy += 2;
                data.Food -= 2;
                data.Water -=2;
            }
        }

        

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
        if(charInteract.interactType != InteractType.None)
            return;

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

        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.Sick;
            return;
        }

        if (data.Food < data.maxFood * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 20)
            {
                actionType = ActionType.Eat;
                return;
            }
        }

        if (data.Water < data.maxWater * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 20)
            {
                actionType = ActionType.Drink;
                return;
            }
        }

        if (data.Sleep < data.maxSleep * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 20)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        if (data.Itchi > data.maxItchi * 0.7f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 50)
            {
                actionType = ActionType.Itchi;
                return;
            }
        }

        if(data.Energy < data.maxEnergy * 0.1f){
            actionType = ActionType.Tired;
            return;
        }
        else if (data.Energy < data.maxEnergy * 0.3f)
        {
            actionType = ActionType.Rest;
            return;
        }

        if (data.happy < data.maxHappy * 0.1f)
        {
            actionType = ActionType.Sad;
            return;
        }

        if (data.curious > data.maxCurious * 0.9f)
        {
            actionType = ActionType.Discover;
            return;
        }


        //Other Action
        int id = Random.Range(0, 100);
        if (id < 80)
        {
            actionType = ActionType.Patrol;
        }
        else
        {
            actionType = ActionType.Discover;
        }

    }


    

    protected override void DoAction()
    {
        if(isAction){
            Debug.Log("Action is doing " + actionType);
            StopAllCoroutines();
            isAction = false;
        }
        //Debug.Log("DoAction " + actionType);
        isAbort = false;
        agent.Stop();
        isAction = true;

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
        else if (actionType == ActionType.Itchi)
        {
            StartCoroutine(Itchi());
        }
        else if (actionType == ActionType.Sick)
        {
            StartCoroutine(Sick());
        }
        else if (actionType == ActionType.Discover)
        {
            StartCoroutine(Discover());
        }
        else if (actionType == ActionType.Hold)
        {
            StartCoroutine(Hold());
        }
        else if (actionType == ActionType.Mouse)
        {
            StartCoroutine(Mouse());
        }
        else if (actionType == ActionType.Bath)
        {
            StartCoroutine(Bath());
        }
        else if (actionType == ActionType.OnTable)
        {
            StartCoroutine(Table());
        }
        else if (actionType == ActionType.Fear)
        {
            StartCoroutine(Fear());
        }
        else if (actionType == ActionType.Tired)
        {
            StartCoroutine(Tired());
        }
        else if (actionType == ActionType.Sad)
        {
            StartCoroutine(Sad());
        }
        else if (actionType == ActionType.Happy)
        {
           StartCoroutine(Happy());
        }
        else if (actionType == ActionType.Call)
        {
            StartCoroutine(Call());
        } else if (actionType == ActionType.Fall)
        {
            StartCoroutine(Fall());
        }else if(actionType == ActionType.Listening){
            StartCoroutine(Listening());
        }else if(actionType == ActionType.SkillUp){
            StartCoroutine(SkillUp());
        }else if(actionType == ActionType.LevelUp){
            StartCoroutine(LevelUp());
        }else if(actionType == ActionType.OnBed){
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
                SetDirection(Direction.D);
                anim.Play("BathStart_D", 0);
            }
            else
                anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 3)));
            

            n++;
        }
        CheckAbort();
    }


    IEnumerator Table()
    {   
        int rand = Random.Range(0,100);    
        if(rand < data.GetSkillProgress(SkillType.Table)){
            SetDirection(Direction.D);
            anim.Play("BathStart_D", 0);
            while (!isAbort)
            {
                yield return new WaitForEndOfFrame();
            }
        }   
        else{
            anim.Play("Jump_L",0);
            //bool isDone = false;
            Vector3 speed = new Vector3(-30,5,-3);
            //Vector3 dropPosition = new Vector3(this.transform.position.x - 10,this.transform.position.y-15,0);
            charInteract.interactType = InteractType.Jump;

            while (charInteract.interactType == InteractType.Jump && !isAbort)
            {
                speed += new Vector3(0,-100*Time.deltaTime,0);
                if (speed.y < -50)
                    speed = new Vector2(speed.x,-50);
                Vector3 pos1 = agent.transform.position;
                pos1.y += speed.y * Time.deltaTime;
                pos1.x += speed.x * Time.deltaTime;
                charScale.height += speed.y * Time.deltaTime;
                charScale.scalePosition.x += speed.x * Time.deltaTime;
                charScale.scalePosition.y += speed.z * Time.deltaTime;
                //pos1.z = dropPosition.y;
                agent.transform.position = pos1;

                if (agent.transform.position.y < charScale.scalePosition.y)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;   
                    enviromentType = EnviromentType.Room;             
                }        
            }
            yield return DoAnim("Fall_L");
            OnLearnSkill(SkillType.Table);
        } 
        CheckAbort();
    }

    IEnumerator Discover()
    {
        if (data.curious > data.maxCurious * 0.4f)
        {
            int ran = Random.Range(0,100);
            if(ran < 30){
                SetTarget(PointType.MouseGate);
                yield return StartCoroutine(MoveToPoint());
                if (this.direction == Direction.RD || this.direction == Direction.RU)
                    yield return StartCoroutine(DoAnim("Smell_RD")) ;
                else if(this.direction == Direction.LD || this.direction == Direction.LU)
                    yield return StartCoroutine(DoAnim("Smell_LD")) ;
                else
                    yield return StartCoroutine(DoAnim("Smell_" + direction.ToString()));

                yield return StartCoroutine(DoAnim("Smell_Bark_LD"));
                data.curious -= 10;
            }else if(ran < 50)
            {
                SetTarget(PointType.Door);
                yield return StartCoroutine(MoveToPoint());
                if (this.direction == Direction.RD || this.direction == Direction.RU)
                    yield return StartCoroutine(DoAnim("Dig_RD")) ;
                else
                    yield return StartCoroutine(DoAnim("Dig_LD")) ;
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
                yield return StartCoroutine(MoveToPoint());
                if (this.direction == Direction.RD || this.direction == Direction.RU)
                    yield return StartCoroutine(DoAnim("Smell_RD")) ;
                else if(this.direction == Direction.LD || this.direction == Direction.LU)
                    yield return StartCoroutine(DoAnim("Smell_LD")) ;
                else
                    yield return StartCoroutine(DoAnim("Smell_" + direction.ToString()));

                yield return StartCoroutine(DoAnim("Smell_Bark_LD"));
              
            }

        }
        CheckAbort();
    }


    IEnumerator Mouse()
    {
        agent.maxSpeed = data.speed * 1.5f;
        anim.speed = 1.5f;
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            agent.SetDestination(GetMouse().transform.position);
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(0.1f));
        }
        agent.maxSpeed = data.speed;
        CheckAbort();
    }

    IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Listen_"+direction.ToString()));
        yield return StartCoroutine(MoveToPoint());
        


        touchObject.SetActive(true);
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Call);
        float t = 0;
        float maxTime = 6f;
        bool isWait = true;
        int n=0;
        n = Random.Range(0, 9);

        while (!isAbort && isWait)
        {
            if (!isTouch)
            {
                if (t == 0)
                {
                    n = Random.Range(0, 9);
                }
                if (n == 0)
                {
                    
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Wait_Lay_PetBack_RD", 0);
                    else
                        anim.Play("Wait_Lay_PetBack_LD", 0);
                }
                else if (n == 1)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Wait_Lay_PetHead_RD", 0);
                    else
                        anim.Play("Wait_Lay_PetHead_LD", 0);
                }
                else if (n == 2)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Wait_Lay_PetStomatch_RD", 0);
                    else
                        anim.Play("Wait_Lay_PetStomatch_LD", 0);
                }
                else if (n == 3)
                {
                    SetDirection(Direction.D);
                    anim.Play("Wait_Sit_Pet_Up_D" );
                }
                else if (n == 4)
                {
                     SetDirection(Direction.D);
                    anim.Play("Wait_Sit_Pet_Down_D" );
                }
                else if (n == 5)
                {
                     SetDirection(Direction.D);
                    anim.Play("Wait_Sit_Pet_Left_D" );
                }
                else if (n == 6)
                {
                     SetDirection(Direction.D);
                    anim.Play("Wait_Sit_Pet_Right_D" );
                }
                else if (n == 7)
                {
                     SetDirection(Direction.D);
                    anim.Play("WaitHandshake_D" );
                }
                else if (n == 8)
                {
                     SetDirection(Direction.D);
                    anim.Play("WaitJumpRound_D" );
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
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_PetBack_RD", 0);
                    else
                        anim.Play("Lay_PetBack_LD", 0);

                }
                else if (n == 1)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_PetHead_RD", 0);
                    else
                        anim.Play("Lay_PetHead_LD", 0);
                }
                else if (n == 2)
                {
                    if (this.direction == Direction.RD || this.direction == Direction.RU)
                        anim.Play("Lay_PetStomatch_RD", 0);
                    else
                        anim.Play("Lay_PetStomatch_LD", 0);
                }
                else if (n == 3)
                {
                     SetDirection(Direction.D);
                    anim.Play("Sit_Pet_Up_D" , 0);
                }
                else if (n == 4)
                {
                     SetDirection(Direction.D);
                    anim.Play("Sit_Pet_Down_D" , 0);
                }
                else if (n == 5)
                {
                     SetDirection(Direction.D);
                    anim.Play("Sit_Pet_Right_D" , 0);
                }
                else if (n == 6)
                {
                     SetDirection(Direction.D);
                    anim.Play("Sit_Pet_Left_D" , 0);
                }
                else if (n == 7)
                {
                     SetDirection(Direction.D);
                    anim.Play("Handshake_D" , 0);
                }
                else if (n == 8)
                {
                     SetDirection(Direction.D);
                    anim.Play("JumpRound_D" , 0);
                }
                t = 0;


            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Listening(){
        yield return StartCoroutine(DoAnim("Listen_" + direction.ToString()));
        CheckAbort();
    }

    
    IEnumerator Rest()
    {
        float maxTime = Random.Range(2, 5);

        int ran = Random.Range(0, 100);
        if (ran < 50)
        {
            SetDirection(Direction.D);
            anim.Play("Idle_Sit_D", 0);
        }
        else
        {
            if (this.direction == Direction.RD || this.direction == Direction.RU)
                anim.Play("Lay_RD", 0);
            else
                anim.Play("Lay_LD", 0);
        }
        

        Debug.Log("Rest");
        yield return StartCoroutine(Wait(maxTime));
        CheckAbort();
    }

    IEnumerator Itchi()
    {

        int i = Random.Range(0, 3);

        if (i == 0)
        {
            if (this.direction == Direction.RD || this.direction == Direction.RU)
                anim.Play("Itchy_RD", 0);
            else
                anim.Play("Itchy_LD", 0);
        }
        else if (i == 1)
        {
            SetDirection(Direction.D);
            anim.Play("Idle_ShedHair_D" , 0);
        }
        else
        {
            SetDirection(Direction.D);
            anim.Play("Shake_Hair_D" , 0);
        }



        Debug.Log("Itchi");
        while (data.itchi > 0.5 * data.maxItchi && !isAbort)
        {
            data.itchi -= 0.1f;
            yield return new WaitForEndOfFrame();
        }
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
        CheckEnviroment();
        CheckAbort();
    }

    IEnumerator Fear()
    {
        SetDirection(Direction.D);
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(DoAnim("Surprised_Hard_D"));
        
        int n = 0;
        int maxCount = Random.Range(1, 3);
        while (!isAbort && n < maxCount)
        {
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(MoveToPoint());
            n++;
        }

        SetTarget(PointType.Safe);
        yield return StartCoroutine(MoveToPoint());

        while(data.Fear > 10){
            data.Fear -= 1;
            yield return StartCoroutine(DoAnim("Scared_LD"));
        }
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


    IEnumerator Tired()
    {
        yield return StartCoroutine(DoAnim("Idle_Tired_D"));
        if (this.direction == Direction.RD || this.direction == Direction.RU)
             anim.Play("Tired_RD", 0);
        else
             anim.Play("Tired_LD", 0);
        while (data.energy > data.maxEnergy * 0.4f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Sad()
    {
        if (this.direction == Direction.RD || this.direction == Direction.RU)
             anim.Play("Lay_Sad_RD", 0);
        else
             anim.Play("Lay_Sad_LD", 0);
        while (data.happy < data.maxHappy * 0.4f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Happy()
    {
        anim.Play("Itchy_RD", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    protected override IEnumerator LevelUp()
    {
        Debug.Log("Level Up" + data.level);
        yield return StartCoroutine(DoAnim("LevelUp_LD"));
        CheckAbort();
    }

    

    
    #endregion

}
