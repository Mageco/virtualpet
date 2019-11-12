using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    protected Vector3 originalPosition;
    protected Vector3 lastPosition;

    public float initZ = -6;
    public float scaleFactor = 0.1f;
    protected Vector3 originalScale;
    public AnimalState state = AnimalState.None;
    protected Animator anim;    

    void Awake()
    {
        originalPosition = this.transform.position;
        lastPosition = this.transform.position;
        originalScale = this.transform.localScale;
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if(state == AnimalState.None)
        {
            Think();
            DoAction();
        }

    }


    void LateUpdate()
    {
        Vector3 pos = this.transform.position;
        pos.z = this.transform.position.y;
        this.transform.position = pos;

        float offset = initZ;

        if (this.transform.position.y < offset)
            this.transform.localScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
        else
            this.transform.localScale = originalScale;
    }

    protected virtual void Think()
    {


        
    }

    void DoAction()
    {
        if (state == AnimalState.Idle)
        {
            Idle();
        }
        else if (state == AnimalState.Seek)
        {
            Seek();
        }
        else if (state == AnimalState.Eat)
        {
            Eat();
        }
        else if (state == AnimalState.Run)
        {
            Run();
        }
        else if (state == AnimalState.Flee)
        {
            Flee();
        }
        else if (state == AnimalState.Hit)
        {
            Hit();
        }
    }

    protected virtual void Idle()
    {

    }

    protected virtual void Seek()
    {

    }

    protected virtual void Run()
    {

    }

    protected virtual void Eat()
    {

    }

    protected virtual void Hit()
    {

    }

    protected virtual void Flee()
    {

    }


}

public enum AnimalState {None,Idle,Seek,Eat,Run,Flee,Hit }
