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
        animalTarget =  animalTargets[0];
        isArrived = false;
        float time = 1;
        while (!isArrived && !isAbort)
        {
            if(time > 0.5f)
            {
                target = animalTarget.transform.position;
                agent.SetDestination(target);
                time = 0;
            }else
            {
                time += Time.deltaTime;
            }
            
            anim.Play("Run_" + this.direction.ToString(), 0);

            if(Vector2.Distance(this.transform.position,target) < 3f){
                isArrived = true;
                agent.Stop();
                animalTarget.OnFlee();
            }
            yield return new WaitForEndOfFrame();
        }
       
        yield return StartCoroutine(DoAnim("Bark_D"));
        CheckAbort();
    }

    IEnumerator Rest()
    {
        int ran = Random.Range(0,100);
        if(ran < 10){
            anim.Play("Lay_LD", 0);
        }    
        else if(ran < 20){
            anim.Play("Idle"+direction.ToString(), 0);
        }
        else if(ran < 50){
            anim.Play("Seat_D", 0);
        } else{
            anim.Play("BathStart_D", 0);
        }
            

        yield return StartCoroutine(Wait(Random.Range(1,2)));
        CheckAbort();
    }    


    void CheckAnimal(){
        for(int i=0;i<animals.Length;i++){
            if(Minigame.instance.IsInBound(animals[i].transform.position) && (animals[i].state == AnimalState.Seek || animals[i].state == AnimalState.Hit)){
                animalTargets.Add(animals[i]);
            }else
            {
                if(animalTargets.Contains(animals[i]))
                {
                    animalTargets.Remove(animals[i]);
                }
            }
        }
    }

    void LoadAnimal(){
        animals = GameObject.FindObjectsOfType<AnimalController>();
    }
}
