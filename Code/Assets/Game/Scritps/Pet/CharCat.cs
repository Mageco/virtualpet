using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCat : CharController
{
  

    protected override IEnumerator Patrol()
    {
       int ran = Random.Range(0, 100);
        if(ran < 30){
            SetTarget(PointType.Patrol);
            yield return StartCoroutine(RunToPoint());
        }else if (ran < 50)
        {
            anim.Play("Standby", 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }
        else if (ran < 70){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(1, 5)));
        }else{ 
            SetTarget(PointType.Table);
            yield return StartCoroutine(RunToPoint());
            if(Vector2.Distance(this.transform.position,target) < 1){
                ItemCollider col = ItemManager.instance.GetItemCollider(ItemType.Table);
                enviromentType = EnviromentType.Table;
                yield return StartCoroutine(JumpUp(35,0,col.transform.position + new Vector3(0,col.height-0.5f,0),col.height));
                enviromentType = EnviromentType.Table;
                CheckEnviroment();
            }
        }
        CheckAbort();
    }

    protected override IEnumerator Table()
    {   
        int ran = Random.Range(0,100);
        if(ran < 30){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(5,15)));
        }else if(ran < 60){
            anim.Play("Idle_" + this.direction.ToString(), 0);
            yield return StartCoroutine(Wait(Random.Range(5,15)));
        }else{
            anim.Play("Sleep", 0);
            while (data.Sleep < data.maxSleep && !isAbort)
            {
                data.Sleep += data.rateSleep * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameManager.instance.AddExp(5,data.iD);
        }
        yield return StartCoroutine(JumpDown(-2,10,30)); 
        CheckAbort();
    }

    public override void OnToy(ToyItem item){
        if(actionType != ActionType.Sick && actionType != ActionType.Injured
        && actionType != ActionType.Toy){
            actionType = ActionType.Toy;
            isAbort = true;
            toyItem = item;
            Debug.Log("Toy");
        }
        
    }

    protected override IEnumerator Toy()
    {
        if(toyItem != null){
            int n = Random.Range(3,10);
            int count = 0;
            Vector3 startPosition = agent.transform.position;
            dropPosition = toyItem.anchorPoint.position;
            agent.transform.position = dropPosition;
            
            yield return new WaitForEndOfFrame();
            while(!isAbort && count < n){
                toyItem.OnActive();
                anim.Play("Teased",0);  
                shadow.SetActive(false);   
                charInteract.interactType = InteractType.Jump;
                float ySpeed = 30;
                while (charInteract.interactType == InteractType.Jump && !isAbort)
                {
                    ySpeed -= 30 * Time.deltaTime;
                    if (ySpeed < -50)
                        ySpeed = -50;
                    Vector3 pos1 = agent.transform.position;
                    pos1.y += ySpeed * Time.deltaTime;
                    agent.transform.position = pos1;
                        
                    if (ySpeed < 0 && this.transform.position.y < dropPosition.y)
                    {
                        this.transform.rotation = Quaternion.identity;
                        charInteract.interactType = InteractType.None;
                        agent.transform.position = dropPosition;
                    }
                    yield return new WaitForEndOfFrame();
                }
                count ++;
            }
            agent.transform.position = startPosition;
        }
        target = GetRandomPoint(PointType.Patrol).position;
        yield return StartCoroutine(RunToPoint());
        CheckAbort();
    }

}
