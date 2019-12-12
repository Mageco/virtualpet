using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class CharMinigame1 : CharController
{
    float timeCheck = 0;
    float maxTimeCheck = 0.1f;
    protected override void CalculateData()
    {

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
    }

    protected override void Load(){
        this.agent.maxSpeed = data.speed;
    }


    public override void OnHold(){

    }

    public override void OnCall(Vector3 pos){

    }

    protected override void DoAction()
    {
        if(Minigame.instance != null && Minigame.instance.state == GameState.Run){
            if(isAction){
                Debug.Log("Action is doing " + actionType);
                StopAllCoroutines();
                isAction = false;
            }

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
    }
    #endregion


    IEnumerator Patrol()
    {
        isArrived = false;
        //float time = 1;
        while (!isArrived && !isAbort && animalTarget != null)
        {
            agent.SetDestination(animalTarget.transform.position);           
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);

            if(Vector2.Distance(this.transform.position,animalTarget.transform.position) < 10f * this.transform.localScale.x){
                isArrived = true;
                agent.Stop();
                animalTarget.OnFlee();
                GameManager.instance.AddCoin(1);
                GameManager.instance.AddExp(5,data.iD);
                yield return StartCoroutine(DoAnim("Bark_Angry_" +direction.ToString())); 
                animalTarget = null;
            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Rest()
    {
        if(GetChiken() != null){
            target = GetChiken().transform.position;  
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);
            yield return StartCoroutine(RunToPoint());    
            anim.Play("Idle_Angry_" + direction.ToString());
            yield return StartCoroutine(Wait(Random.Range(0.5f,1f)));
        }else
        {
            //anim.Play("BathStart_D", 0);
            yield return StartCoroutine(DoAnim("BathStart_D"));
        }

        CheckAbort();
    }    


    void CheckAnimal(){
       
        animals = GameObject.FindObjectsOfType<AnimalController>();
        animalTarget = null;
        for(int i=0;i<animals.Length;i++){
            if(Minigame.instance.IsInBound(animals[i].transform.position) && (animals[i].tag == "Animal")){
                
                animalTarget = animals[i];
                //Debug.Log("Check Animal " + animals[i].name);
                break;
            }
        }
        if(animalTarget != null){
            //isAbort = true;
            actionType = ActionType.Patrol;
        }else if(animalTarget == null){
            //isAbort = true;
            actionType = ActionType.Rest;
        }
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
        while (data.energy < data.maxEnergy * 0.5f && !isAbort)
        {
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }
}
