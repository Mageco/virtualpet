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
            data.actionEnergyConsume = 0.7f;
        else if (actionType == ActionType.Discover)
        {
            data.actionEnergyConsume = 0.5f;
        }
        else if (actionType == ActionType.Patrol)
        {
            data.actionEnergyConsume = 0.3f;
        }else if (actionType == ActionType.Bath)
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
        data.Stamina -= data.staminaConsume;
        data.Stamina += data.actionEnergyConsume;

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

    protected override void CalculateDirection(){
        if (agent.transform.eulerAngles.z < 30f && agent.transform.eulerAngles.z > -30f || (agent.transform.eulerAngles.z > 330f && agent.transform.eulerAngles.z < 390f) || (agent.transform.eulerAngles.z < -330f && agent.transform.eulerAngles.z > -390f))
            direction = Direction.U;
        else if ((agent.transform.eulerAngles.z > 30f && agent.transform.eulerAngles.z < 80f) || (agent.transform.eulerAngles.z > -330f && agent.transform.eulerAngles.z < -280f))
            direction = Direction.LU;
        else if ((agent.transform.eulerAngles.z >= 80f && agent.transform.eulerAngles.z <= 150f) || (agent.transform.eulerAngles.z >= -280f && agent.transform.eulerAngles.z <= -210f))
            direction = Direction.LD;
        else if ((agent.transform.eulerAngles.z <= -30f && agent.transform.eulerAngles.z >= -80f) || (agent.transform.eulerAngles.z >= 280f && agent.transform.eulerAngles.z <= 330f))
            direction = Direction.RU;
        else if ((agent.transform.eulerAngles.z <= -80 && agent.transform.eulerAngles.z >= -150) || (agent.transform.eulerAngles.z >= 210f && agent.transform.eulerAngles.z <= 280f))
            direction = Direction.RD;
        else
            direction = Direction.D;
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

        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.Sick;
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
        if (id < 10)
        {
            actionType = ActionType.Rest;
        }
        else if (id < 60)
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
        //Debug.Log("DoAction " + actionType);
        isAbort = false;
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
            //if (!isAbort)
            //{
                InputController.instance.SetTarget(PointType.Patrol);
                yield return StartCoroutine(MoveToPoint());
            //}
            //if (!isAbort)
            //{
                int ran = Random.Range(0, 100);
                if (ran < 30)
                {
                    SetDirection(Direction.D);
                    anim.Play("BathStart_D", 0);
                }
                else
                    anim.Play("Idle_" + this.direction.ToString(), 0);
                yield return StartCoroutine(Wait(Random.Range(1, 3)));
            //}

            n++;
        }
        CheckAbort();
    }

    IEnumerator Bath()
    {

        anim.Play("BathStart_D", 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Table()
    {
        int rand = Random.Range(0,100);    
        if(rand < 50){
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
            Vector2 speed = new Vector2(-30,0);
            Vector3 dropPosition = new Vector3(this.transform.position.x - 10,this.transform.position.y-15,0);
            charInteract.interactType = InteractType.Drop;

            while (charInteract.interactType == InteractType.Drop && !isAbort)
            {
                speed += new Vector2(0,-100*Time.deltaTime);
                if (speed.y < -50)
                    speed = new Vector2(speed.x,-50);
                Vector3 pos1 = agent.transform.position;
                pos1.y += speed.y * Time.deltaTime;
                pos1.x += speed.x * Time.deltaTime;
                pos1.z = dropPosition.y;
                agent.transform.position = pos1;

                if (Mathf.Abs(agent.transform.position.y - dropPosition.y) < 2f)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;                
                }
                yield return new WaitForEndOfFrame();            
            }
            yield return DoAnim("Fall_L");
        } 
        CheckAbort();
    }

    IEnumerator Discover()
    {
        if (!isAbort && data.curious > data.maxCurious * 0.4f)
        {
            InputController.instance.SetTarget(PointType.MouseGate);
            yield return StartCoroutine(MoveToPoint());
             if (this.direction == Direction.RD || this.direction == Direction.RU)
                 yield return StartCoroutine(DoAnim("Smell_RD")) ;
            else if(this.direction == Direction.LD || this.direction == Direction.LU)
                yield return StartCoroutine(DoAnim("Smell_LD")) ;
            else
                 yield return StartCoroutine(DoAnim("Smell_" + direction.ToString()));

            yield return StartCoroutine(DoAnim("Smell_Bark_LD"));
            data.curious -= 10;
            
        }
        
       
        CheckAbort();
    }

    IEnumerator Hold()
    {
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        Vector3 dropPosition = Vector3.zero;
        SetDirection(Direction.D);
        if (data.Health < data.maxHealth * 0.1f)
        {
            anim.Play("Hold_Sick_D", 0);
        }
        else{
            anim.Play("Hold_D", 0);
        }
        while (charInteract.interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > dragOffset)
                pos.y = dragOffset;
            else if (pos.y < -20)
                pos.y = -20;

            if (pos.x > 52)
                pos.x = 52;
            else if (pos.x < -49)
                pos.x = -49;

            pos.z = -50;
            agent.transform.position = pos;

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.identity, Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();
        }

        //Start Drop
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0, -2, 0), -Vector2.up, 100);
        Vector3 pos2 = this.transform.position;
        pos2.y = pos2.y - dragOffset - 2;
        if (pos2.y < -20)
            pos2.y = -20;
        dropPosition = pos2;
        enviromentType = EnviromentType.Room;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.tag == "Table")
            {
                pos2.y = hit[i].collider.transform.position.y;
                dropPosition = pos2;
                enviromentType = EnviromentType.Table;
                break;
            }
            else if (hit[i].collider.tag == "Bath")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Bath;
                break;
            }
        }

        float fallSpeed = 0;
        float maxTime = 1;
        while (charInteract.interactType == InteractType.Drop)
        {
            fallSpeed += 100f * Time.deltaTime;
            if (fallSpeed > 50)
                fallSpeed = 50;
            Vector3 pos1 = agent.transform.position;
            pos1.y -= fallSpeed * Time.deltaTime;
            pos1.z = dropPosition.z;
            agent.transform.position = pos1;


            if (Vector2.Distance(agent.transform.position, dropPosition) < fallSpeed * Time.deltaTime * 2)
            {
                this.transform.rotation = Quaternion.identity;

                if (data.Health < data.maxHealth * 0.1f)
                {
                    SetDirection(Direction.D);
                    anim.Play("Drop_Sick_D", 0);
                    maxTime = 3;
                }
                else
                {
                    if (fallSpeed < 50)
                    {
                        SetDirection(Direction.D);
                        anim.Play("Drop_Light_D", 0);
                        maxTime = 1;
                    }
                    else
                    {
                        SetDirection(Direction.D);
                        anim.Play("Drop_Hard_D", 0);
                        maxTime = 3;
                    }
                }

                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return StartCoroutine(Wait(maxTime));
        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.None;
        }
        else
        {
            if (enviromentType == EnviromentType.Bath)
            {
                OnBath();
            }
            else if (enviromentType == EnviromentType.Table)
            {
                OnTable();
            }
        }

        CheckAbort();
    }

    IEnumerator Mouse()
    {
        while(GetMouse().state != MouseState.Idle && !isAbort){
            agent.SetDestination(GetMouse().transform.position);
            agent.speed = 45;
            //int ran = Random.Range(0,100);
            //if(ran > 50 && direction == Direction.LD){
            //    anim.Play("Jump_LookDown_D");
            //}else
            anim.Play("Run_" + this.direction.ToString(), 0);
            anim.speed = 1.5f;
            yield return StartCoroutine(Wait(0.2f));
        }
        CheckAbort();
    }

    IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Listen_"+direction.ToString()));

        if (!isAbort)
        {
            InputController.instance.SetTarget(PointType.Call);
            yield return StartCoroutine(MoveToPoint());
        }

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

    IEnumerator Pee()
    {
        SetDirection(Direction.D);
        anim.Play("Pee_D" , 0);
        Debug.Log("Pee");
        SpawnPee();
        OnLearnSkill(SkillType.Pee);
        while (data.Pee > 1 && !isAbort)
        {
            data.Pee -= 0.5f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Shit()
    {
         SetDirection(Direction.D);
        anim.Play("Poop_D" , 0);
        SpawnShit();
        OnLearnSkill(SkillType.Toilet);
        while (data.Shit > 1 && !isAbort)
        {
            data.Shit -= 0.5f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Eat()
    {
        if(GetFoodItem() != null){
            if (!isAbort)
            {
                InputController.instance.SetTarget(PointType.Eat);
                yield return StartCoroutine(MoveToPoint());
            }
            bool canEat = true;
            if (GetFoodItem().CanEat() && !isAbort)
            {
                direction = Direction.LD;
                anim.Play("Eat_LD" , 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Food < data.maxFood && !isAbort && canEat)
                {
                    data.Food += 0.3f;
                    GetFoodItem().Eat(0.3f);
                    if (!GetFoodItem().CanEat())
                    {
                        canEat = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetFoodItem().anchor.position) > 0.5f)
                        canEat = false;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        CheckAbort();
    }

    IEnumerator Drink()
    {
        if(GetDrinkItem() != null){
           //Debug.Log("Drink");
            if (!isAbort)
            {
                InputController.instance.SetTarget(PointType.Drink);
                yield return StartCoroutine(MoveToPoint());
            }

            bool canDrink = true;

            if (GetDrinkItem().CanEat())
            {
                direction = Direction.LD;
                anim.Play("Eat_LD" , 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Water < data.maxWater && !isAbort && canDrink)
                {
                    data.Water += 0.5f;
                    GetDrinkItem().Eat(0.5f);
                    if (!GetDrinkItem().CanEat())
                    {
                        canDrink = false;
                    }
                    if (Vector2.Distance(this.transform.position, GetDrinkItem().anchor.position) > 0.5f)
                        canDrink = false;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        CheckAbort();
    }

    IEnumerator Sleep()
    {
        //Debug.Log("Sleep");
        if(data.SkillLearned(SkillType.Sleep)){
            InputController.instance.SetTarget(PointType.Sleep);
        }else{
            OnLearnSkill(SkillType.Sleep);
            InputController.instance.SetTarget(PointType.Patrol);
        }
           
        yield return StartCoroutine(MoveToPoint());
        if (!isAbort){
             direction = Direction.LD;
            anim.Play("Sleep_LD" , 0);
        }

        while (data.Sleep < data.maxSleep && !isAbort)
        {
            data.Sleep += 0.01f;
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
            if (ran < 20)
            {
                SetDirection(Direction.D);
                anim.Play("Idle_Sit_D", 0);
            }
            else if (ran < 40)
            {
                if (this.direction == Direction.RD || this.direction == Direction.RU)
                    anim.Play("Lay_RD", 0);
                else
                    anim.Play("Lay_LD", 0);
            }else if(ran < 60){

            }
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
        yield return new WaitForEndOfFrame();
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

    IEnumerator Fear()
    {
        SetDirection(Direction.D);
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(DoAnim("Surprised_Hard_D"));
        
        int n = 0;
        int maxCount = Random.Range(2, 5);
        while (!isAbort && n < maxCount)
        {
            InputController.instance.SetTarget(PointType.Patrol);
            yield return StartCoroutine(MoveToPoint());
            n++;
        }

        InputController.instance.SetTarget(PointType.Safe);
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

    
    #endregion

}
