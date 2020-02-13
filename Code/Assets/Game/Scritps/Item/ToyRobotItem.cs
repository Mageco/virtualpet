using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class ToyRobotItem : ToyItem
{

    public Vector3 target;
    public PolyNavAgent agent;
    public bool isArrived = true;
    public bool isAbort = false;
    public float speed = 10;
    Direction direction = Direction.R;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        LoadPrefab();
    }

    public void LoadPrefab()
    {
        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        go1.transform.position = this.transform.position;
        agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
        agent.maxSpeed = this.speed;

    }

    void OnArrived()
    {
        isArrived = true;
        StartCoroutine(Hold());
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(state == EquipmentState.Active)
        {
            CalculateDirection();
            this.transform.position = agent.transform.position;
        }

    }


    protected override void OnClick()
    {
        Debug.Log("Click");
        if(state == EquipmentState.Drag || state == EquipmentState.Hold)
        {
            isAbort = false;
            Debug.Log("Turn on");
            state = EquipmentState.Active;
            agent.transform.position = this.transform.position;
            target = GameManager.instance.GetRandomPetObject().transform.position + new Vector3(0,-2,0);
            StartCoroutine(MoveToPoint());
            MageManager.instance.PlaySoundName("Item_Robot_TurnOn", false);
        }
        else if(state == EquipmentState.Active)
        {
            isAbort = true;
            agent.Stop();
            Debug.Log("Turn off");
            state = EquipmentState.Idle;
            MageManager.instance.PlaySoundName("Item_Robot_TurnOff", false);
            animator.Play("Idle_" + direction.ToString(), 0);
        }
    }

    void CalculateDirection()
    {
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else
            direction = Direction.R;
    }


    IEnumerator Hold()
    {
        int soundId = MageManager.instance.PlaySoundName("Toy_Robot_Dance", false);
        agent.Stop();
        yield return StartCoroutine(DoAnim("Dance_" + direction.ToString()));
        MageManager.instance.StopSound(soundId);
        animator.Play("Idle_" + direction.ToString(), 0);
        state = EquipmentState.Idle;
    }


    protected IEnumerator DoAnim(string a)
    {
        float time = 0;
        animator.Play(a, 0);
        yield return new WaitForEndOfFrame();
        while (time < animator.GetCurrentAnimatorStateInfo(0).length && !isAbort)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator MoveToPoint()
    {
        isArrived = false;

        agent.SetDestination(target);
        while (!isArrived && !isAbort)
        {
            animator.Play("Walk_" + this.direction.ToString(), 0);
            yield return new WaitForEndOfFrame();
        }
    }

}
