﻿using System.Collections;
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

    IEnumerator Hold()
    {
        GameManager.instance.SetCameraTarget(this.gameObject);
        charInteract.interactType = InteractType.Drag;
        enviromentType = EnviromentType.Room;
        Vector3 dropPosition = Vector3.zero;

        anim.Play("Hold_D", 0);
        while (charInteract.interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - charInteract.dragOffset;
            pos.z = 0;
            if (pos.y > dragOffset-4)
                pos.y = dragOffset-4;
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
    }

    IEnumerator Eat()
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

   
    IEnumerator Sleep()
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

    IEnumerator Bed()
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
