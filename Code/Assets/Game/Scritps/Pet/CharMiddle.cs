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

        if (data.Energy < data.maxEnergy * 0.1f)
        {
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
        else if (actionType == ActionType.Sick)
        {
            StartCoroutine(Sick());
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
        else if (actionType == ActionType.Fall)
        {
            StartCoroutine(Fall());
        }
        else if (actionType == ActionType.Listening)
        {
            StartCoroutine(Listening());
        }
        else if (actionType == ActionType.SkillUp)
        {
            StartCoroutine(SkillUp());
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

    IEnumerator Bath()
    {
        int ran = Random.Range(0,100);
        if(ran < data.GetSkillProgress(SkillType.Bath) * 10){
            anim.Play("Idle_" + direction.ToString(),0);
            while(!isAbort){   
                yield return new WaitForEndOfFrame();
            }
        }
        else{
            if(!isAbort){
                anim.Play("Jump_D", 0);
                float speed = 15;
                Vector3 dropPosition = new Vector3(this.transform.position.x, this.transform.position.y - 10, 0);
                charInteract.interactType = InteractType.Drop;
                
                while (charInteract.interactType == InteractType.Drop && !isAbort)
                {
                    speed -= 40 * Time.deltaTime;
                    if (speed < -50)
                        speed = -50;
                    Vector3 pos1 = agent.transform.position;
                    pos1.y += speed * Time.deltaTime;
                    pos1.x = agent.transform.position.x;
                    if(speed > 0)
                        pos1.z = pos1.y;
                    else
                        pos1.z = dropPosition.y;
                    agent.transform.position = pos1;

                    if (Mathf.Abs(agent.transform.position.y - dropPosition.y) < 2f)
                    {
                        this.transform.rotation = Quaternion.identity;
                        charInteract.interactType = InteractType.None;
                    }
                    yield return new WaitForEndOfFrame();
                }
                enviromentType = EnviromentType.Room;
                OnLearnSkill(SkillType.Bath);
            }
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
        else
        {
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
            else if (hit[i].collider.tag == "Bed")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Bed;
                break;
            }
            else if (hit[i].collider.tag == "Toilet")
            {
                pos2.y = hit[i].collider.transform.position.y;
                pos2.z = hit[i].collider.transform.position.z;
                dropPosition = pos2;
                enviromentType = EnviromentType.Toilet;
                break;
            }
        }

        float fallSpeed = 0;
        float maxTime = 1;
        while (charInteract.interactType == InteractType.Drop && !isAbort)
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
                Vector3 pos3 = agent.transform.position;
                pos3.y = dropPosition.y;
                agent.transform.position = pos3;


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
                        maxTime = 2;
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
            if(!isAbort){
                if (enviromentType == EnviromentType.Bath)
                {
                    OnBath();
                }
                else if (enviromentType == EnviromentType.Table)
                {
                    OnTable();
                }
                else if (enviromentType == EnviromentType.Bed)
                {
                    OnBed();
                }
                else if (enviromentType == EnviromentType.Toilet)
                {
                    OnToilet();
                }
            }
        }

        CheckAbort();
    }

    IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Listen_" + direction.ToString()));

        if (!isAbort)
        {
            //SetTarget(PointType.Call);
            yield return StartCoroutine(MoveToPoint());
        }

        float t = 0;
        float maxTime = 6f;

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
        yield return StartCoroutine(DoAnim("Listen_" + direction.ToString()));
        CheckAbort();
    }

    IEnumerator Pee()
    {
        agent.Stop();
        SetDirection(Direction.D);
        anim.Play("Pee_D", 0);
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
        anim.Play("Poop_D", 0);
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
        if (GetFoodItem() != null)
        {
            if (!isAbort)
            {
                SetTarget(PointType.Eat);
                yield return StartCoroutine(MoveToPoint());
            }
            bool canEat = true;
            if (GetFoodItem().CanEat() && !isAbort)
            {
                direction = Direction.LD;
                anim.Play("Eat_LD", 0);
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
        if (GetDrinkItem() != null)
        {
            //Debug.Log("Drink");
            if (!isAbort)
            {
                SetTarget(PointType.Drink);
                yield return StartCoroutine(MoveToPoint());
            }

            bool canDrink = true;

            if (GetDrinkItem().CanEat())
            {
                direction = Direction.LD;
                anim.Play("Drink_LD", 0);
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
        if (data.SkillLearned(SkillType.Sleep))
        {
            SetTarget(PointType.Sleep);
        }
        else
        {
            OnLearnSkill(SkillType.Sleep);
            SetTarget(PointType.Patrol);
        }

        yield return StartCoroutine(MoveToPoint());
        if (!isAbort)
        {
            direction = Direction.LD;
            anim.Play("Sleep_LD", 0);
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

    IEnumerator Bed()
    {
        if(data.sleep < 0.3f*data.maxSleep){
            actionType = ActionType.Sleep;
            isAbort = true;
        }
        else{
            if(!isAbort){
                anim.Play("Jump_D", 0);
                float speed = 5;
                Vector3 dropPosition = new Vector3(this.transform.position.x, this.transform.position.y - 10, 0);
                charInteract.interactType = InteractType.Drop;
                while (charInteract.interactType == InteractType.Drop && !isAbort)
                {
                    speed -= 30 * Time.deltaTime;
                    if (speed < -50)
                        speed = -50;
                    Vector3 pos1 = agent.transform.position;
                    pos1.y += speed * Time.deltaTime;
                    pos1.x = agent.transform.position.x;
                    pos1.z = dropPosition.y;
                    agent.transform.position = pos1;

                    if (Mathf.Abs(agent.transform.position.y - dropPosition.y) < 2f)
                    {
                        this.transform.rotation = Quaternion.identity;
                        charInteract.interactType = InteractType.None;
                    }
                    yield return new WaitForEndOfFrame();
                }
                enviromentType = EnviromentType.Room;
            }
        }

        CheckAbort();
    }

    IEnumerator Toilet()
    {
        if(data.pee > 0.7f*data.maxPee){
            actionType = ActionType.Pee;
            isAbort = true;
        }
        else{
            anim.Play("Jump_D", 0);
            float speed = 5;
            Vector3 dropPosition = new Vector3(this.transform.position.x, this.transform.position.y - 5, 0);
            charInteract.interactType = InteractType.Drop;
            while (charInteract.interactType == InteractType.Drop && !isAbort)
            {
                speed -= 30 * Time.deltaTime;
                if (speed < -50)
                    speed = -50;
                Vector3 pos1 = agent.transform.position;
                pos1.y += speed * Time.deltaTime;
                pos1.x = agent.transform.position.x;
                pos1.z = dropPosition.y;
                agent.transform.position = pos1;

                if (Mathf.Abs(agent.transform.position.y - dropPosition.y) < 2f)
                {
                    this.transform.rotation = Quaternion.identity;
                    charInteract.interactType = InteractType.None;
                }
                yield return new WaitForEndOfFrame();
            }
            enviromentType = EnviromentType.Room;
        }

        CheckAbort();
    }


    protected override IEnumerator LevelUp()
    {
        Debug.Log("Level Up" + data.level);
        yield return StartCoroutine(DoAnim("LevelUp_LD"));

        if (data.level >= 5)
            data.Load();

        CheckAbort();

    }


    #endregion
}
