using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class ToyRobotItem : ToyItem
{
    public PetHappyItem item;
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

    protected override void OnMouseDown()
    {

        if (IsPointerOverUIObject())
        {
            return;
        }

        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(state == EquipmentState.Active)
        {
            isAbort = true;
            agent.Stop();
            Debug.Log("Turn off");
            state = EquipmentState.Idle;
            MageManager.instance.PlaySound3D("Item_Robot_TurnOff", false, this.transform.position);
            animator.Play("Idle_" + direction.ToString(), 0);
            return;
        }else
            state = EquipmentState.Hold;
    }

    protected override void OnClick()
    {
        Debug.Log("Click");
        if(state == EquipmentState.Drag || state == EquipmentState.Hold || state == EquipmentState.Idle)
        {
            PetHappyItem[] items = FindObjectsOfType<PetHappyItem>();
            if (items.Length > 0)
                item = items[Random.Range(0, items.Length)];
            if (item != null)
            {
                target = item.transform.position;
                isAbort = false;
                Debug.Log("Turn on");
                state = EquipmentState.Active;
                agent.transform.position = this.transform.position;

                StartCoroutine(MoveToPoint());
                MageManager.instance.PlaySound3D("Item_Robot_TurnOn", false, this.transform.position);
            }
            else
            {
                target = ItemManager.instance.GetRandomPoint(AreaType.All);
                isAbort = false;
                Debug.Log("Turn on");
                state = EquipmentState.Active;
                agent.transform.position = this.transform.position;

                StartCoroutine(MoveToPoint());
                MageManager.instance.PlaySound3D("Item_Robot_TurnOn", false, this.transform.position);
            }
        }
        else if(state == EquipmentState.Active)
        {
            isAbort = true;
            agent.Stop();
            Debug.Log("Turn off");
            state = EquipmentState.Idle;
            MageManager.instance.PlaySound3D("Item_Robot_TurnOff", false,this.transform.position);
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

    public override void OnActive()
    {
        state = EquipmentState.Active;
    }

    IEnumerator Hold()
    {
        int soundId = MageManager.instance.PlaySound3D("happy_collect_item_01", false,this.transform.position);
        agent.Stop();
        if (item != null)
            item.OnClick();
        
        yield return StartCoroutine(DoAnim("Dance_" + direction.ToString()));
        animator.Play("Idle_" + direction.ToString(), 0);
        PetHappyItem[] items = FindObjectsOfType<PetHappyItem>();
        if (items.Length > 0)
            item = items[Random.Range(0, items.Length)];
        if (item != null && state == EquipmentState.Active)
        {
            target = item.transform.position;
            isAbort = false;
            Debug.Log("Turn on");
            state = EquipmentState.Active;
            agent.transform.position = this.transform.position;

            StartCoroutine(MoveToPoint());
            MageManager.instance.PlaySound3D("Item_Robot_TurnOn", false, this.transform.position);
        }
        else
        {
            isAbort = true;
            agent.Stop();
            Debug.Log("Turn off");
            state = EquipmentState.Idle;
            MageManager.instance.PlaySound3D("Item_Robot_TurnOff", false, this.transform.position);
            animator.Play("Idle_" + direction.ToString(), 0);
        }
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


        while (target != null && !isArrived && !isAbort)
        {
            agent.SetDestination(target);
            animator.Play("Walk_" + this.direction.ToString(), 0);
            if (Vector2.Distance(this.transform.position, target) < 2)
            {
                StartCoroutine(Hold());
                isArrived = true;
                agent.Stop();
            }
                
            yield return new WaitForEndOfFrame();
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Happy")
        {
            other.transform.parent.GetComponent<PetHappyItem>().OnClick();
        }
    }

}
