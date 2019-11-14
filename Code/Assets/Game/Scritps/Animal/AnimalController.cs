using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    protected Vector3 originalPosition;
    protected Vector3 lastPosition;
    protected float speed = 5;
    public float maxSpeed = 10;
    public float initZ = -6;
    public float scaleFactor = 0.1f;
    protected Vector3 originalScale;
    public AnimalState state = AnimalState.None;
    protected Animator anim;    

    protected bool isAbort = false;
    protected bool isArrived = false;
    protected Direction direction = Direction.L;

   void Awake()
    {
        originalPosition = this.transform.position;
        lastPosition = this.transform.position;
        originalScale = this.transform.localScale;
        anim = this.GetComponent<Animator>();
        Load();
    }

    private void Update()
    {
        if(state == AnimalState.None)
        {
            Think();
            DoAction();
        }
    }

    protected virtual void Load(){

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

    protected virtual void DoAction()
    {
 
    }

    protected void SetDirection(Direction d)
    {
        direction = d;            
    }

    protected IEnumerator DoAnim(string a)
    {
        float time = 0;
        anim.Play(a, 0);
        yield return new WaitForEndOfFrame();
        while (time < anim.GetCurrentAnimatorStateInfo(0).length && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveToPoint(Vector3 target)
    {
        while (!isArrived && !isAbort)
        {
            anim.speed = 1 + speed/maxSpeed;
            Vector3 d = Vector3.Normalize(target - this.transform.position);
            this.transform.position += d * speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
            if(Vector2.Distance(this.transform.position,target) < 1f){
                isArrived = true;
                anim.speed = 1;
            }
        }
       CheckAbort();
    }

    protected IEnumerator Wait(float maxT)
    {
        float time = 0;

        while (time < maxT && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected void CheckAbort()
    {
        if (!isAbort)
            state = AnimalState.None;
        else
        {
            DoAction();
        }
    }

}

public enum AnimalState {None,Idle,Seek,Eat,Run,Flee,Hit }
