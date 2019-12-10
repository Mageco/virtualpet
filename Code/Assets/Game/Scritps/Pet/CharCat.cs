using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCat : CharController
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

        if(data.Food < data.maxFood * 0.3f && GetFoodItem() != null && Vector2.Distance(this.transform.position,GetFoodItem().transform.position) < 3){
            actionType = ActionType.Eat;
            return;
        }

        if(data.Water < data.maxWater * 0.3f && GetDrinkItem() != null && Vector2.Distance(this.transform.position,GetDrinkItem().transform.position) < 3){
            actionType = ActionType.Drink;
            return;
        }

        if (data.Food < data.maxFood * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Eat;
                return;
            }
        }


        if (data.Water < data.maxWater * 0.1f)
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


        if(data.Energy < data.maxEnergy * 0.1f){
            actionType = ActionType.Tired;
            return;
        }

        if (data.Energy < data.maxEnergy * 0.3f)
        {
            actionType = ActionType.Rest;
            return;
        }

        if (data.curious > data.maxCurious * 0.9f)
        {
            actionType = ActionType.Discover;
            return;
        }


        actionType = ActionType.Patrol;
        //Other Action
    }


    protected override void CalculateDirection(){
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else 
            direction = Direction.R;
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
        else if (actionType == ActionType.Tired)
        {
            StartCoroutine(Tired());
        }
        else if (actionType == ActionType.Sad)
        {
            StartCoroutine(Sad());
        }
        else if (actionType == ActionType.Call)
        {
            StartCoroutine(Call());
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


    IEnumerator Table()
    {   
        anim.Play("Idle_" + this.direction.ToString(), 0);
        while (!isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Discover()
    {
        if (data.curious > data.maxCurious * 0.4f)
        {
           
            SetTarget(PointType.Door);
            yield return StartCoroutine(MoveToPoint());
            if (this.direction == Direction.RD || this.direction == Direction.RU)
                yield return StartCoroutine(DoAnim("Idle_R")) ;
            else
                yield return StartCoroutine(DoAnim("Idle_L")) ;
            data.curious -= 10;                
 

        }
        CheckAbort();
    }

    protected override IEnumerator Hold()
    {
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        GameManager.instance.SetCameraTarget(this.gameObject);
        anim.Play("Hold", 0);
        
        while (charInteract.interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > charScale.maxHeight)
                pos.y = charScale.maxHeight;
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
        CheckDrop(0);

        float fallSpeed = 0;
        float maxTime = 1;
        while (charInteract.interactType == InteractType.Drop && !isAbort)
        {
            if (agent.transform.position.y > dropPosition.y)
            {
                fallSpeed += 100f * Time.deltaTime;
                if (fallSpeed > 50)
                    fallSpeed = 50;
                Vector3 pos1 = agent.transform.position;
                pos1.y -= fallSpeed * Time.deltaTime;
                pos1.x = Mathf.Lerp(pos1.x,dropPosition.x,Time.deltaTime * 5);
                pos1.z = charScale.scalePosition.z;
                agent.transform.position = pos1;
            }
            else
            {
                this.transform.rotation = Quaternion.identity;
                Vector3 pos3 = agent.transform.position;
                pos3.y = dropPosition.y;
                pos3.x = dropPosition.x;
                agent.transform.position = pos3;
                charScale.height = dropPosition.y - charScale.scalePosition.y;

                anim.Play("Drop", 0);
                maxTime = 1;

                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.ResetCameraTarget();
        yield return StartCoroutine(Wait(maxTime));
        if (data.Health < data.maxHealth * 0.1f)
        {
            actionType = ActionType.None;
        }
        else
        {
            CheckEnviroment();            
        }
       
        CheckAbort();
    }


    IEnumerator Mouse()
    {
        agent.maxSpeed = data.speed * 1f;
        anim.speed = 1f;
        while(GetMouse() != null && GetMouse().state != MouseState.Idle && !isAbort){
            agent.SetDestination(GetMouse().transform.position);
            anim.Play("Run_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(0.1f));
        }
        agent.maxSpeed = data.speed;
        CheckAbort();
    }

    IEnumerator Call()
    {
        yield return StartCoroutine(DoAnim("Idle_"+direction.ToString()));
        yield return StartCoroutine(MoveToPoint());
        touchObject.SetActive(true);
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Call);
        anim.Play("Idle_" + direction.ToString(),0);
        yield return StartCoroutine(Wait(Random.Range(2f,5f)));
        CheckAbort();
    }

    IEnumerator Listening(){
        yield return StartCoroutine(DoAnim("Idle_" + direction.ToString()));
        CheckAbort();
    }

    
    IEnumerator Rest()
    {
        anim.Play("Idle_" + direction.ToString(),0);
        
        while(data.Food > 0 && data.Water > 0 && data.Sleep > 0 &&data.Energy < 0.5f * data.maxEnergy && !isAbort){
            data.Energy += 0.05f;
            data.Food -= 0.03f;
            data.Water -= 0.03f;
            data.Sleep -= 0.01f;
            yield return new WaitForEndOfFrame();
        }  
        yield return StartCoroutine(Wait(Random.Range(2f,5f)));
        CheckAbort();
    }

    

    IEnumerator Sick()
    {
        anim.Play("Sick", 0);
        Debug.Log("Sick");
        while (data.health < 0.1f * data.maxHealth && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Sick);
        CheckEnviroment();
        CheckAbort();
    }

    


    IEnumerator Tired()
    {
        anim.Play("Tired", 0);
        while (data.Food > 0 && data.Water > 0 && data.Sleep > 0 && data.energy < data.maxEnergy * 0.1f && !isAbort)
        {
            data.Energy += 0.1f;
            data.Food -= 0.05f;
            data.Water -= 0.05f;
            data.Sleep += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Sad()
    {
        anim.Play("Sad", 0);
        while (data.happy < data.maxHappy * 0.4f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }


    protected override IEnumerator LevelUp()
    {
        yield return new WaitForEndOfFrame();
        CheckAbort();
    }

    

    

    protected override IEnumerator Bath()
    {
        int ran = Random.Range(0,100);
        if(ran < 70 + data.GetSkillProgress(SkillType.Bath) * 3){
            anim.Play("Idle_" + direction.ToString(),0);
            while(!isAbort){   
                yield return new WaitForEndOfFrame();
            }
        }
        else{
            yield return StartCoroutine(JumpDown(-5,25,35));
            OnLearnSkill(SkillType.Bath);            
        }

        CheckAbort();
    }

    protected override IEnumerator Toilet()
    {
        if(data.shit > 0.7*data.maxShit){
            actionType = ActionType.Shit;
            isAbort = true;
        }else if(data.pee > 0.7f*data.maxPee){
            actionType = ActionType.Pee;
            isAbort = true;
        }
        else{
            yield return StartCoroutine(JumpDown(-7,10,30));           
        }

        CheckAbort();
    }

    protected override IEnumerator Pee()
    {
        if(enviromentType != EnviromentType.Toilet)
        {
            if (data.SkillLearned(SkillType.Toilet) )
            {
                SetTarget(PointType.Toilet);
                yield return StartCoroutine(MoveToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Toilet);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position + new Vector3(0,col.height,0),col.height));
                enviromentType = EnviromentType.Toilet;
            }else{
                OnLearnSkill(SkillType.Toilet);
            }
        }

        SetDirection(Direction.D);
        anim.Play("Pee", 0);
        Debug.Log("Pee");
        SpawnPee(peePosition.position + new Vector3(0, 0, 50));
        while (data.Pee > 1 && !isAbort)
        {
            data.Pee -= 0.5f;
            yield return new WaitForEndOfFrame();
        }

        if(enviromentType == EnviromentType.Toilet){
            GameManager.instance.AddExp(5,data.iD);
            GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnToilet);
            LevelUpSkill(SkillType.Toilet);
            yield return StartCoroutine(JumpDown(-7,10,30));     
        }
        
        CheckAbort();
    }

    protected override IEnumerator Shit()
    {
        if(enviromentType != EnviromentType.Toilet)
        {
            if (data.SkillLearned(SkillType.Toilet) )
            {
                SetTarget(PointType.Toilet);
                yield return StartCoroutine(MoveToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Toilet);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position + new Vector3(0,col.height,0),col.height));
                enviromentType = EnviromentType.Toilet;
            }else{
                OnLearnSkill(SkillType.Toilet);
            }
        }

        SetDirection(Direction.D);
        anim.Play("Shit", 0);
        SpawnShit(shitPosition.position);
        while (data.Shit > 1 && !isAbort)
        {
            data.Shit -= 0.5f;
            yield return new WaitForEndOfFrame();
        }

        if(enviromentType == EnviromentType.Toilet){
            GameManager.instance.AddExp(5,data.iD);
            GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnToilet);
            LevelUpSkill(SkillType.Toilet);
            yield return StartCoroutine(JumpDown(-7,10,30));     
        }
        CheckAbort();
    }

    protected override IEnumerator Eat()
    {
        if (GetFoodItem() != null)
        {
            SetTarget(PointType.Eat);
            yield return StartCoroutine(MoveToPoint());
            bool canEat = true;
            if (GetFoodItem().CanEat())
            {
                direction = Direction.LD;
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
                    yield return DoAnim("Bark_D");
                else if(ran < 40)
                {
                    SetTarget(PointType.Patrol);
                    yield return StartCoroutine(MoveToPoint());
                    yield return DoAnim("Bark_D");
                    SetTarget(PointType.Eat);
                    yield return StartCoroutine(MoveToPoint());
                }else if(ran < 50)
                    yield return DoAnim("Bark_Sit_D");
                else if(ran < 70) 
                    yield return DoAnim("Idle_D");
                else if(ran < 80){
                    yield return DoAnim("Eat_LD");
                }else if(ran < 90){
                    yield return DoAnim("Smell_LD");
                }else{
                    yield return DoAnim("Smell_Bark_LD");
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
            yield return StartCoroutine(MoveToPoint());
            

            bool canDrink = true;

            if (GetDrinkItem().CanEat())
            {
                direction = Direction.LD;
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
                yield return DoAnim("Idle_"+ direction.ToString());
            }
        }
        CheckAbort();
    }

    protected override IEnumerator Bed()
    {
        int ran = Random.Range(0,100);
        if(ran < 50 + data.GetSkillProgress(SkillType.Sleep) * 5){
            if(data.sleep < 0.3f*data.maxSleep){
                actionType = ActionType.Sleep;
                Abort();
            }else{                    
                anim.Play("Idle_" + direction.ToString(),0);
                yield return StartCoroutine(Wait(Random.Range(2,6)));
                yield return StartCoroutine(JumpDown(-7,10,30));
            }
        }
        else{
            yield return StartCoroutine(JumpDown(-7,10,30));
        }
        
        CheckAbort();
    }

    protected override IEnumerator Sleep()
    {
        if(enviromentType != EnviromentType.Bed)
        {
            if (data.SkillLearned(SkillType.Sleep) )
            {
                SetTarget(PointType.Sleep);
                yield return StartCoroutine(MoveToPoint());
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Bed);
                yield return StartCoroutine(JumpUp(10,5,col.transform.position,col.height));
                enviromentType = EnviromentType.Bed;
            }else{
                OnLearnSkill(SkillType.Sleep);
            }
        }

       
        direction = Direction.LD;
        anim.Play("Sleep", 0);

        while (data.Sleep < data.maxSleep && !isAbort)
        {
            data.Sleep += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        
 
        if(enviromentType == EnviromentType.Bed){
            GameManager.instance.AddExp(5,data.iD);
            GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.Sleep);
            LevelUpSkill(SkillType.Sleep);
            yield return StartCoroutine(JumpDown(-7,10,30));
        }

        CheckAbort();
    }

    
    #endregion

}
