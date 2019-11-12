using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : AnimalController
{
    bool isArrived = true;
    public float speed = 5;
    float maxSpeed = 20;
    float duration = 3;
    float time = 0;

    IEnumerator MoveToPoint(Vector3 target)
    {
        isArrived = false;
        while (!isArrived)
        {
            anim.Play("Walk");
            anim.speed = 1 + speed/maxSpeed;
            this.transform.position = Vector3.Lerp(this.transform.position, target, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
    }

    protected override void Think()
    {
        int ran = Random.Range(0, 100);
        if (ran > 30)
        {
            state = AnimalState.Run;
        }
        else if(ran > 60)
        {
            state = AnimalState.Eat;
        }
        else
        {
            state = AnimalState.Idle;
        }
    }

    
}
