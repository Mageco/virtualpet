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
    public Vector2 boundx;
    public Vector2 boundy;
    Direction direction;

    IEnumerator MoveToPoint(Vector3 target)
    {
        isArrived = false;
        while (!isArrived)
        {
            
            anim.speed = 1 + speed/maxSpeed;
            this.transform.position = Vector3.Lerp(this.transform.position, target, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        state = AnimalState.None;
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

    protected override void Idle()
    {
        anim.Play("Idle_L", 0);
        if(time > duration)
        {
            time = 0;
            state = AnimalState.None;
        }
        else
        {
            time += Time.deltaTime;
        }

    }

    protected override void Eat()
    {
        anim.Play("Eat_L", 0);
        if (time > duration)
        {
            time = 0;
            state = AnimalState.None;
        }
        else
        {
            time += Time.deltaTime;
        }

    }

    protected override void Run()
    {
        float x = Random.Range(boundx.x, boundx.y);
        float y = Random.Range(boundy.x, boundy.y);
        anim.Play("Run_L",0);
        StartCoroutine(MoveToPoint(new Vector3(x, y, 0)));

    }


}
