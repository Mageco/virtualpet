using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class ToyCarrier : BaseFloorItem
{
    //public CharController target;
    public PolyNavAgent agent;
    public bool isArrived = true;
    public bool isAbort = false;
    public float speed = 10;
    public Direction direction = Direction.R;
    public Vector3 target;
    ObstructItem obstructItem;

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
        obstructItem = this.GetComponentInChildren<ObstructItem>();
    }

    void OnArrived()
    {
        isArrived = true;
        Debug.Log("Arrived");
        if (state == EquipmentState.Active)
            OnRun();
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (state == EquipmentState.Active)
        {
            this.transform.position = agent.transform.position;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        //if (state == EquipmentState.Active)
        //{
            CalculateDirection();
        //}
    }


    void CalculateDirection()
    {
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else
            direction = Direction.R;

        
        if(direction == Direction.R)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, -1);
            foreach(Transform anchor in anchorPoints)
            {
                anchor.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                anchor.transform.localScale = new Vector3(anchor.transform.localScale.x, anchor.transform.localScale.y, -1);
            }
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, 1);
            foreach (Transform anchor in anchorPoints)
            {
                anchor.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                anchor.transform.localScale = new Vector3(anchor.transform.localScale.x, anchor.transform.localScale.y, 1);
            }
        }
    }

    public override void OnActive()
    {
        if (state == EquipmentState.Active)
            return;
        agent.transform.position = this.transform.position;
        obstructItem.gameObject.SetActive(false);
        state = EquipmentState.Active;
        animator.Play("Active", 0);
        OnRun();
    }


    public override void DeActive()
    {
        obstructItem.gameObject.SetActive(true);
        agent.transform.position = this.transform.position;
        animator.Play("Idle", 0);
        isAbort = true;
        state = EquipmentState.Idle;
        agent.Stop();
        isArrived = true;
    }

    void OnRun()
    {
        if (isArrived)
        {
            isArrived = false;
            target = ItemManager.instance.GetRandomPoint(AreaType.All);
            agent.SetDestination(target);
        }

    }
}
