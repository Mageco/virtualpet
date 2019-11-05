using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSmall : CharController
{
    protected override void CalculateData()
    {
        data.Sleep -= data.sleepConsume;
    }

    #region Thinking
    protected override void Think()
    {

        if (data.Sleep < data.maxSleep * 0.1f)
        {
            int ran = Random.Range(0, 100);
            if (ran > 30)
            {
                actionType = ActionType.Sleep;
                return;
            }
        }

        //Other Action
        actionType = ActionType.Rest;

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
        else if(actionType == ActionType.SkillUp){
            StartCoroutine(SkillUp());
        }
    }
    #endregion


    public override void OnEat(){
        StartCoroutine(Eat());
    }


    #region Main Action

    

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

    IEnumerator Hold()
    {
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        Vector3 dropPosition = Vector3.zero;

        anim.Play("Hold_D", 0);
        while (charInteract.interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > 20)
                pos.y = 20;
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
        pos2.y = pos2.y - 22;
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

    IEnumerator Eat()
    {
        if(ItemController.instance.FoodItem() != null){
            if (!isAbort)
            {
                InputController.instance.SetTarget(PointType.Eat);
                yield return StartCoroutine(MoveToPoint());
            }
            bool canEat = true;
            if (ItemController.instance.FoodItem().CanEat() && !isAbort)
            {
                direction = Direction.LD;
                anim.Play("Eat_LD" , 0);
                yield return StartCoroutine(Wait(0.1f));
                while (data.Food < data.maxFood && !isAbort && canEat)
                {
                    data.Food += 0.3f;
                    ItemController.instance.FoodItem().Eat(0.3f);
                    if (!ItemController.instance.FoodItem().CanEat())
                    {
                        canEat = false;
                    }
                    if (Vector2.Distance(this.transform.position, ItemController.instance.FoodItem().anchor.position) > 0.5f)
                        canEat = false;
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

    

    
    #endregion
}
