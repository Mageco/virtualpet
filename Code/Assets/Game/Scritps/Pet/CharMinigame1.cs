using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMinigame1 : CharController
{
    protected override void CalculateData()
    {
        data.Sleep -= data.sleepConsume;
    }

    public AnimalController[] animals;
    List<AnimalController> animalTargets = new List<AnimalController>();
    AnimalController animalTarget;

    #region Thinking
    protected override void Think()
    {
        CheckAnimal();
        if(animalTargets.Count > 0)
        {
            actionType = ActionType.Patrol;
        }else 
            actionType = ActionType.Rest;
    }

    protected override void Load(){
        LoadAnimal();
        this.data.speed = 20;
        this.agent.speed = 20;
    }

    public override void OnHold(){

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

    }
    #endregion


    IEnumerator Patrol()
    {
        isArrived = false;
        float time = 1;
        animalTarget = GetTarget();
        while (!isArrived && !isAbort)
        {
            if(time > 0.1f)
            {
                animalTarget = GetTarget();
                agent.SetDestination(animalTarget.transform.position);
                time = 0;
            }else
            {
                time += Time.deltaTime;
            }
            
            anim.Play("Run_Angry_" + this.direction.ToString(), 0);

            if(Vector2.Distance(this.transform.position,animalTarget.transform.position) < 10f){
                isArrived = true;
                agent.Stop();
                anim.Play("Bark_Angry_" +direction.ToString(),0);
                animalTarget.OnFlee();
                GameManager.instance.AddCoin(1);
                yield return StartCoroutine(Wait(0.5f));
            }
            yield return new WaitForEndOfFrame();
        }
        CheckAbort();
    }

    IEnumerator Rest()
    {
        if(Minigame.instance != null && Minigame.instance.live < Minigame.instance.maxLive)
        {
            anim.Play("Idle_Angry_" + direction.ToString());
            yield return StartCoroutine(Wait(Random.Range(0.1f,0.5f)));
        }else
        {
            int ran = Random.Range(0,100);

            if(ran < 10){
                anim.Play("Lay_LD", 0);
                 yield return StartCoroutine(Wait(Random.Range(0.4f,0.6f)));
            }    
            else if(ran < 20){
                anim.Play("Idle_"+direction.ToString(), 0);
                 yield return StartCoroutine(Wait(Random.Range(0.4f,0.6f)));
            }
            else if(ran < 30){
                anim.Play("Idle_Sit_D", 0);
                 yield return StartCoroutine(Wait(Random.Range(0.4f,0.6f)));
            } else if(ran < 40){
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
        if(animals.Length ==0)
            LoadAnimal();
        animalTargets.Clear();
        for(int i=0;i<animals.Length;i++){
            if(Minigame.instance.IsInBound(animals[i].transform.position) && (animals[i].tag == "Animal")){
                animalTargets.Add(animals[i]);
            }
        }
    }

    AnimalController GetTarget(){
        if(animals.Length ==0){
            LoadAnimal();
            CheckAnimal();
        }
           
        float l = 1000;
        int id = 0;
        for(int i=0;i<animalTargets.Count;i++){
            if(l > Vector2.Distance(this.transform.position,animalTargets[i].transform.position)){
                id = i;
                l = Vector2.Distance(this.transform.position,animalTargets[i].transform.position);
            }
        }
        return animalTargets[id];
    }

    void LoadAnimal(){
        animals = GameObject.FindObjectsOfType<AnimalController>();
    }

    GameObject GetChiken(){
        ChickenController[] chickens = GameObject.FindObjectsOfType<ChickenController>();
        return chickens[Random.Range(0,chickens.Length)].gameObject;
    }
}
