using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSmall : CharController
{
    protected override void CalculateData()
    {
        data.Sleep -= data.sleepConsume;
        data.Food -= 0.05f;
        data.energy += 0.05f;
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

/*
    IEnumerator Hold()
    {
        GameManager.instance.SetCameraTarget(this.gameObject);
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;

        anim.Play("Hold_D", 0);
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
            fallSpeed += 100f * Time.deltaTime;
            if (fallSpeed > 50)
                fallSpeed = 50;
            Vector3 pos1 = agent.transform.position;
            pos1.y -= fallSpeed * Time.deltaTime;
            pos1.z = charScale.scalePosition.z;
            agent.transform.position = pos1;

            if (Vector2.Distance(agent.transform.position, charScale.scalePosition) < fallSpeed * Time.deltaTime * 2)
            {
                this.transform.rotation = Quaternion.identity;
                Vector3 pos3 = agent.transform.position;
                pos3.y = charScale.scalePosition.y;
                agent.transform.position = pos3;

                if (fallSpeed < 40)
                {
                    SetDirection(Direction.D);
                    anim.Play("Drop_Light_LD", 0);
                    maxTime = 2;
                }
                else
                {
                    SetDirection(Direction.D);
                    anim.Play("Drop_Hard_LD", 0);
                    maxTime = 2;
                }                

                charInteract.interactType = InteractType.None;
            }
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.ResetCameraTarget();
        yield return StartCoroutine(Wait(maxTime));

        
        if(!isAbort){
            CheckEnviroment();
        }        

        CheckAbort();
    }*/

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
