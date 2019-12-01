using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class CharMinigame1 : CharController
{
    float timeCheck = 0;
    float maxTimeCheck = 0.1f;
    bool isStart = false;
    protected override void CalculateData()
    {
        data.Sleep -= data.sleepConsume;
        data.actionEnergyConsume = 0;
        if (actionType == ActionType.Patrol)
            data.actionEnergyConsume = 1f;

        data.Energy -= data.actionEnergyConsume;
        if(data.Food > 0 && actionType == ActionType.Tired){
            data.Food -= 1f;
            data.Energy += 2f;
        }

    }

    public AnimalController[] animals;
    AnimalController animalTarget;

    #region Thinking
    protected override void Think()
    {
        if(data.energy < data.maxEnergy * 0.1f){
            actionType = ActionType.Tired;
            return;
        }

        CheckAnimal();
        if(animalTarget != null)
        {
            actionType = ActionType.Patrol;
        }else 
            actionType = ActionType.Rest;
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

        if(timeCheck > maxTimeCheck){
            CheckAnimal();
            timeCheck = 0;
        }else
            timeCheck += Time.deltaTime;
    }

    protected override void Load(){
        LoadAnimal();
        //this.data.speed = 20;
        this.agent.maxSpeed = data.speed;
    }

    public override void OnHold(){

    }

    protected override void DoAction()
    {
        //Debug.Log("DoAction " + actionType);
        isAbort = false;
        if (agent == null)
            return;
        //agent.Stop();
        if (actionType == ActionType.Rest)
        {
            StartCoroutine(Rest());
        }
        else if (actionType == ActionType.Patrol)
        {
            StartCoroutine(Patrol());
        }else if (actionType == ActionType.Tired)
        {
            StartCoroutine(Tired());
        }

    }
    #endregion


    IEnumerator Patrol()
    {
        isArrived = false;
        float time = 1;
        while (!isArrived && !isAbort && animalTarget != null)
        {
            //if(time > 0.1f)
            //{
                agent.SetDestination(animalTarget.transform.position);
            //    time = 0;
            //}else
            //{
            //    time += Time.deltaTime;
            //}
            
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);

            if(Vector2.Distance(this.transform.position,animalTarget.transform.position) < 5f){
                isArrived = true;
                agent.Stop();
                animalTarget.OnFlee();
                GameManager.instance.AddCoin(1);
                GameManager.instance.AddExp(5);
                yield return StartCoroutine(DoAnim("Bark_Angry_" +direction.ToString())); 
                animalTarget = null;
            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Rest()
    {
        if(isStart)
        {
            target = GetChiken().transform.position;  
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            yield return StartCoroutine(MoveToPoint());    
            anim.Play("Idle_Angry_" + direction.ToString());
            yield return StartCoroutine(Wait(Random.Range(0.5f,1f)));
        }else
        {
            int ran = Random.Range(0,100);

            if(ran < 30){
                anim.Play("Idle_"+direction.ToString(), 0);
                 yield return StartCoroutine(Wait(Random.Range(0.4f,0.6f)));
            }
            else if(ran < 60){
                anim.Play("BathStart_D", 0);
                 yield return StartCoroutine(Wait(Random.Range(0.4f,0.6f)));
            }else
            {
                target = GetChiken().transform.position;
                yield return StartCoroutine(MoveToPoint());
            }
           
        } 
        CheckAbort();
    }    


    void CheckAnimal(){
        LoadAnimal();
        if(animalTarget != null && Minigame.instance.IsInBound(animalTarget.transform.position))
        {
            isStart = true;
            return;
        }
        animalTarget = null;
        for(int i=0;i<animals.Length;i++){
            if(Minigame.instance.IsInBound(animals[i].transform.position) && (animals[i].tag == "Animal") && animals[i].state != AnimalState.Flee){
                
                animalTarget = animals[i];
                break;
            }
        }
        if(animalTarget != null && actionType == ActionType.Rest){
            isAbort = true;
            actionType = ActionType.Patrol;
        }else if(animalTarget == null && actionType == ActionType.Patrol){
            isAbort = true;
            actionType = ActionType.Rest;
        }
    }

    void LoadAnimal(){
        animals = GameObject.FindObjectsOfType<AnimalController>();
    }

    GameObject GetChiken(){
        ChickenController[] chickens = GameObject.FindObjectsOfType<ChickenController>();
        if(chickens != null && chickens.Length > 0)
            return chickens[Random.Range(0,chickens.Length)].gameObject;
        else return null;
    }

    IEnumerator Tired()
    {
        if(UIManager.instance != null)
            UIManager.instance.OnQuestNotificationPopup("Chó của bạn cần nghỉ ngơi để lấy lại sức");
        anim.Play("Idle_Tired_D",0);
        while (data.energy < data.maxEnergy * 0.2f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }
}
