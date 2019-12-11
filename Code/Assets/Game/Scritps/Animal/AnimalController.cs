using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

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

    public PolyNavAgent agent;
    public List<GizmoPoint> fleePoints = new List<GizmoPoint>();

    public AnimalType animalType = AnimalType.Mouse;

   void Awake()
    {
        LoadPrefab();
    }

    public void LoadPrefab(){
        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        go1.name = "Agent " + this.gameObject.name;
		agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
        
        Load();
    }


    protected virtual void Start(){
        originalPosition = this.transform.position;
        lastPosition = this.transform.position;
        originalScale = this.transform.localScale;
        anim = this.GetComponent<Animator>();
        agent.transform.position = this.transform.position;
        agent.maxSpeed = this.maxSpeed;
        Debug.Log(agent.maxSpeed);
        Load();
    }

    private void Update()
    {
        if(state == AnimalState.None)
        {
            Think();
            DoAction();
        }

        Debug.Log(agent.maxSpeed);

        CalculateDirection();
        if(state != AnimalState.Hold)
            this.transform.position = agent.transform.position;
    }

    protected virtual void CalculateDirection(){
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else 
            direction = Direction.R;
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

    public void OnArrived(){
        isArrived = true;
    }

    protected virtual void Think()
    {
        
    }

    protected virtual void DoAction()
    {
 
    }

    public virtual void InActive(){
        state = AnimalState.InActive;
        isAbort = true;
    }

    public virtual void OnFlee(){
        if(state != AnimalState.Flee)
        {
            isAbort = true;
            state = AnimalState.Flee;
        }
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
        isArrived = false;
        if (Vector2.Distance(target, agent.transform.position) > 0.5f)
        {
            if(!isAbort){
                agent.SetDestination(target);
            }
            anim.speed = 1 + speed/maxSpeed;


            while (!isArrived && !isAbort)
            {
                if(state == AnimalState.Flee)
                    anim.Play("Flee_" + direction.ToString(),0);
                else 
                    anim.Play("Run_" + direction.ToString(),0);
                yield return new WaitForEndOfFrame();
            }
            anim.speed = 1;
        }
        else
        {
            agent.transform.position = target;
            isArrived = true;
            agent.Stop();
        }
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

