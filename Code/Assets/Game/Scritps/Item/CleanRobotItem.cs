﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class CleanRobotItem : BaseFloorItem
{
	public float clean = 1f;
    //public float initZ = -6;
	public Vector3 target;
    public PolyNavAgent agent;
    public bool isArrived = true;
	public bool isAbort = false;
	public float speed = 10;

	public AnimalState robotState = AnimalState.None;

	Direction direction = Direction.R;

	ItemDirty dirtyTarget;

	int count = 0;
	

    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();
		this.clean = DataHolder.GetItem(this.itemID).value;
		LoadPrefab();
    }

	public void LoadPrefab(){
        GameObject go1 = Instantiate(Resources.Load("Prefabs/Pets/Agent")) as GameObject;
        //go1.transform.parent = GameManager.instance.transform;
		go1.transform.position = this.transform.position;
		agent = go1.GetComponent<PolyNavAgent>();
        agent.OnDestinationReached += OnArrived;
		agent.maxSpeed = this.speed;
    }

	void OnArrived()
    {
        isArrived = true;
    }


    // Update is called once per frame
    protected override void Update()
    {
		base.Update();

		if (state == EquipmentState.Idle)
		{
			if (robotState == AnimalState.None)
			{
				Think();
				DoAction();
			}

			CalculateDirection();
			this.transform.position = agent.transform.position;
		}
		else
			agent.transform.position = this.transform.position;

    }

	void Think(){
		robotState = AnimalState.Idle;
	}

	void DoAction(){
		isAbort = false;
		isArrived = false;
		if (robotState == AnimalState.Idle)
		{
			StartCoroutine(Idle());
		}else if (robotState == AnimalState.Seek)
		{
			StartCoroutine(Seek());
		}else if (robotState == AnimalState.Run)
		{
			StartCoroutine(Run());
		}else if (robotState == AnimalState.Flee)
		{
			StartCoroutine(Flee());
		}else if (robotState == AnimalState.Hold)
		{
			StartCoroutine(Hold());
		}
	}

	void CalculateDirection(){
        if (agent.transform.eulerAngles.z < 180f && agent.transform.eulerAngles.z > 0f || (agent.transform.eulerAngles.z > -360f && agent.transform.eulerAngles.z < -180f))
            direction = Direction.L;
        else 
            direction = Direction.R;
    }

    /*
	void LateUpdate()
	{
		float offset = initZ;

		if (transform.position.y < offset)
			transform.localScale = originalScale * (1 + (-transform.position.y + offset) * scaleFactor);
		else
			transform.localScale = originalScale;

		Vector3 pos = this.transform.position;
		pos.z = this.transform.position.y * 10;
		this.transform.position = pos;
	}*/

	IEnumerator Idle(){
		agent.Stop();
		count = 0;
		animator.Play("Idle_" + direction.ToString(),0);
		while(!isAbort){
			yield return new WaitForEndOfFrame();
		}
		CheckAbort();
		
	}

	IEnumerator Run(){
		target = GetDirtyItem().transform.position;
		yield return StartCoroutine(MoveToPoint());
		if(!isAbort){
			robotState = AnimalState.Hold;
			isAbort = true;
		}
		CheckAbort();
	}

	IEnumerator Hold(){
        int soundId = MageManager.instance.PlaySound3D("Item_Robot_Clean", true,this.transform.position);
        animator.Play("Clean_" + direction.ToString(),0);
		float time = 0;
		while(dirtyTarget != null && time < 10 && !isAbort){
			time += Time.deltaTime;
			dirtyTarget.OnClean(clean * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		if(dirtyTarget == null){
			ItemManager.instance.SpawnStar(this.transform.position,1);
			GameManager.instance.LogAchivement(AchivementType.Clean);
		}
		if(!isAbort){
			robotState = AnimalState.Seek;
			isAbort = true;
		}
        MageManager.instance.StopSound(soundId);
		CheckAbort();
	}

	IEnumerator Flee(){
		target = ItemManager.instance.GetRandomPoint(AreaType.All);
		yield return StartCoroutine(MoveToPoint());
		if(!isAbort){
			robotState = AnimalState.Idle;
			isAbort = true;
		}
		CheckAbort();
	}

	IEnumerator Seek(){
		if(GetDirtyItem() != null){
			robotState = AnimalState.Run;
			isAbort = true;
		}else{
			int ran  = Random.Range(0,100);
			if(ran > 50 || count == 0){
				SetTarget(AreaType.All);
				count = 1;
				yield return StartCoroutine(MoveToPoint());
				if(!isAbort){
					robotState = AnimalState.Seek;
					isAbort = true;
				}

			}else{
				if(!isAbort){
					robotState = AnimalState.Flee;
					isAbort = true;
				}

			}
		}
		CheckAbort();
	}

	protected override void OnMouseDown()
	{
		if (IsPointerOverUIObject())
		{
			return;
		}

		if (robotState != AnimalState.Idle && (state == EquipmentState.Active || state == EquipmentState.Busy))
		{
			return;
		}

		clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		state = EquipmentState.Hold;
	}

	protected override void OnClick()
	{
		state = EquipmentState.Idle;
		if(robotState == AnimalState.Idle){
            MageManager.instance.PlaySound("Item_Robot_TurnOn", false);
            robotState = AnimalState.Seek;
			isAbort = true;
		}else{
            MageManager.instance.PlaySound("Item_Robot_TurnOff", false);
			robotState = AnimalState.Idle;
			isAbort = true;
		}
	}

	protected void SetDirection(Direction d)
    {
        direction = d;            
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
            robotState = AnimalState.None;
        else
        {
            DoAction();
        }
    }

 	ItemDirty GetDirtyItem()
    {
        if(dirtyTarget == null)
            dirtyTarget = FindObjectOfType<ItemDirty>();
        return dirtyTarget;
    }

	public void SetTarget(AreaType type)
	{
        int n = 0;
        Vector3 pos = ItemManager.instance.GetRandomPoint (type);
        while(pos == target && n<10)
        {
            pos = ItemManager.instance.GetRandomPoint (type);
            n++;
        }
        target = pos;
	}
}
