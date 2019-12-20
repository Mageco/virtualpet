using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class CleanRobotItem : MonoBehaviour
{
	public float clean = 1f;
    public float initZ = -6;
	public float scaleFactor = 0.05f;
	Vector3 originalScale;
	Vector3 originalPosition;
	Vector3 lastPosition;
	public Vector3 target;
    public PolyNavAgent agent;
    public bool isArrived = true;
	public bool isAbort = false;
	public float speed = 10;

	public AnimalState state = AnimalState.None;

	Direction direction = Direction.R;

	ItemDirty dirtyTarget;


	protected Animator anim;

	protected ItemObject item;
	int count = 0;
	

	protected void Awake(){
		anim = this.GetComponent<Animator>();
		originalPosition = this.transform.position;
		originalScale = this.transform.localScale;
	}
    // Start is called before the first frame update
    protected void Start()
    {
		item = this.transform.parent.GetComponent<ItemObject>();
		this.clean = DataHolder.GetItem(item.itemID).value;
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
    protected void Update()
    {
        if(state == AnimalState.None)
        {
            Think();
            DoAction();
        }

        CalculateDirection();
		this.transform.position = agent.transform.position;
    }

	void Think(){
		state = AnimalState.Idle;
	}

	void DoAction(){
		isAbort = false;
		isArrived = false;
		if (state == AnimalState.Idle)
		{
			StartCoroutine(Idle());
		}else if (state == AnimalState.Seek)
		{
			StartCoroutine(Seek());
		}else if (state == AnimalState.Run)
		{
			StartCoroutine(Run());
		}else if (state == AnimalState.Flee)
		{
			StartCoroutine(Flee());
		}else if (state == AnimalState.Hold)
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
	}

	IEnumerator Idle(){
		agent.Stop();
		count = 0;
		anim.Play("Idle_" + direction.ToString(),0);
		while(!isAbort){
			yield return new WaitForEndOfFrame();
		}
		CheckAbort();
		
	}

	IEnumerator Run(){
		target = GetDirtyItem().transform.position;
		yield return StartCoroutine(MoveToPoint());
		if(!isAbort){
			state = AnimalState.Hold;
			isAbort = true;
		}
		CheckAbort();
	}

	IEnumerator Hold(){
		anim.Play("Clean_" + direction.ToString(),0);
		float time = 0;
		while(dirtyTarget != null && time < 10 && !isAbort){
			time += Time.deltaTime;
			dirtyTarget.OnClean(clean * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		if(dirtyTarget == null)
			GameManager.instance.LogAchivement(AchivementType.Clean);
		if(!isAbort){
			state = AnimalState.Seek;
			isAbort = true;
		}
		CheckAbort();
	}

	IEnumerator Flee(){
		target = originalPosition;
		yield return StartCoroutine(MoveToPoint());
		if(!isAbort){
			state = AnimalState.Idle;
			isAbort = true;
		}
		CheckAbort();
	}

	IEnumerator Seek(){
		if(GetDirtyItem() != null){
			state = AnimalState.Run;
			isAbort = true;
		}else{
			int ran  = Random.Range(0,100);
			if(ran > 50 || count == 0){
				SetTarget(PointType.Patrol);
				count = 1;
				yield return StartCoroutine(MoveToPoint());
				if(!isAbort){
					state = AnimalState.Seek;
					isAbort = true;
				}

			}else{
				if(!isAbort){
					state = AnimalState.Flee;
					isAbort = true;
				}

			}
		}
		CheckAbort();
	}

	protected virtual void OnMouseUp()
	{
        if(IsPointerOverUIObject()){
            return;
        }

		if(state == AnimalState.Idle){
			state = AnimalState.Seek;
			isAbort = true;
		}else{
			state = AnimalState.Idle;
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
        anim.Play(a, 0);
        yield return new WaitForEndOfFrame();
        while (time < anim.GetCurrentAnimatorStateInfo(0).length && !isAbort)
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
            anim.Play("Walk_" + this.direction.ToString(), 0);
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
            state = AnimalState.None;
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

	public void SetTarget(PointType type)
	{
        int n = 0;
        Vector3 pos = ItemManager.instance.GetRandomPoint (type).position;
        while(pos == target && n<10)
        {
            pos = ItemManager.instance.GetRandomPoint (type).position;
            n++;
        }
        target = pos;
	}

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}
