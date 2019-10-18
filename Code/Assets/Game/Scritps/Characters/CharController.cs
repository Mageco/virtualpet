using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CharController : MonoBehaviour {

	#region Declair
	//Data
	public CharData data;
	//[HideInInspector]
	public InteractType interactType;
	//[HideInInspector]
	public EnviromentType enviromentType = EnviromentType.Room;
	//[HideInInspector]
	public Direction direction = Direction.D;
	public AnimType animType = AnimType.Idle;
	public Direction touchDirection = Direction.D;

	//Think
	float dataTime = 0;
	float maxDataTime = 1f;

	//Movement
	public Transform target;
	
	[HideInInspector]
	PolyNavAgent agent;
	float agentTime = 0;
	float maxAgentTime = 0.1f;
	Vector2 lastTargetPosition;
	public bool isArrived = true;
	public bool isAbort = false;

	//Action
	public ActionType actionType = ActionType.None;
	public bool isEndAction = false;



	//Anim
	CharAnim charAnim;
	Animator anim;

	//Interact
	bool isTouch = false;

    //Pee,Sheet
    public Transform peePosition;
    public Transform shitPosition;
    public GameObject peePrefab;
    public GameObject shitPrefab;

	#endregion

	#region Load

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
		charAnim = this.GetComponent<CharAnim> ();
		agent = GameObject.FindObjectOfType<PolyNavAgent> ();
	}
	// Use this for initialization
	void Start () {
		charAnim.SetAnimType (AnimType.Idle);
	}
	#endregion

	#region Update

	// Update is called once per frame
	void FixedUpdate () {

		if (agent.transform.eulerAngles.z < 30f && agent.transform.eulerAngles.z > -30f || (agent.transform.eulerAngles.z > 330f && agent.transform.eulerAngles.z < 390f) || (agent.transform.eulerAngles.z < -330f && agent.transform.eulerAngles.z > -390f))
			direction = Direction.U;
		else if ((agent.transform.eulerAngles.z > 30f && agent.transform.eulerAngles.z < 80f) || (agent.transform.eulerAngles.z > -330f && agent.transform.eulerAngles.z < -280f))
			direction = Direction.LU;
		else if ((agent.transform.eulerAngles.z >= 80f && agent.transform.eulerAngles.z <= 150f) || (agent.transform.eulerAngles.z >= -280f && agent.transform.eulerAngles.z <= -210f))
			direction = Direction.LD;
		else if ((agent.transform.eulerAngles.z <= -30f && agent.transform.eulerAngles.z >= -80f) || (agent.transform.eulerAngles.z >= 280f && agent.transform.eulerAngles.z <= 330f))
			direction = Direction.RU;
		else if ((agent.transform.eulerAngles.z <= -80 && agent.transform.eulerAngles.z >= -150) || (agent.transform.eulerAngles.z >= 210f && agent.transform.eulerAngles.z <= 280f))
			direction = Direction.RD;
		else
			direction = Direction.D;

		this.transform.position = agent.transform.position;

		//Calculate Attribue Data
		if (dataTime > maxDataTime) {
			CalculateData ();
			dataTime = 0;
		} else
			dataTime += Time.deltaTime;
	}
	#endregion

	#region Data
	void CalculateData()
	{
		data.actionEnergyConsume = 0;


		if (interactType == InteractType.Call)
			data.actionEnergyConsume = 0.2f;
		else if (interactType == InteractType.Command)
			data.actionEnergyConsume = 0.3f;
		else if (interactType == InteractType.Caress)
			data.actionEnergyConsume = 0.1f;
		else if (interactType == InteractType.FollowTarget)
			data.actionEnergyConsume = 0.5f;
		else if (interactType == InteractType.None) {
			if (actionType == ActionType.Discover) {
				data.actionEnergyConsume = 0.5f;
			}else if(actionType == ActionType.Patrol) {
				data.actionEnergyConsume = 0.3f;
			}
		}

		data.Energy -= data.basicEnergyConsume + data.actionEnergyConsume;

		data.Happy -= data.happyConsume;
		if (interactType == InteractType.Call)
			data.Happy += 0.01f;
		if (interactType == InteractType.Caress)
			data.Happy += 0.05f;
		if (interactType == InteractType.Command)
			data.Happy += 0.02f;

		if (data.Food > 0 && data.Water > 0) {
			float delta = 0.1f + data.Health * 0.001f + data.Happy * 0.001f;
			data.Food -= delta;
			data.Water -= delta;
			data.Energy += delta;
			data.Shit += delta;
			data.Pee += delta * 2;
		}

		data.Dirty += data.dirtyFactor;

		data.Stamina -= data.staminaConsume;
		data.Stamina += data.actionEnergyConsume;

		data.Sleep -= data.sleepConsume;

		float deltaHealth = data.healthConsume;

		deltaHealth += (data.Happy - data.maxHappy * 0.3f) * 0.001f;

		if (data.Dirty > data.maxDirty * 0.8f)
			deltaHealth -= (data.Dirty - data.maxDirty * 0.8f) * 0.003f;

		if(data.Pee > data.maxPee * 0.9f)
			deltaHealth -= (data.Pee - data.maxPee * 0.9f) * 0.001f;

		if(data.Shit > data.maxShit * 0.9f)
			deltaHealth -= (data.Shit - data.maxShit * 0.9f) * 0.002f;

		if(data.Food < data.maxFood * 0.1f)
			deltaHealth -= (data.maxFood * 0.1f - data.Food) * 0.001f;

		if(data.Water < data.maxWater * 0.1f)
			deltaHealth -= (data.maxWater * 0.1f - data.Water) * 0.001f;
		
		if(data.Sleep < data.maxSleep * 0.05f)
			deltaHealth -= (data.maxSleep * 0.05f - data.Sleep) * 0.004f;

		data.Health += deltaHealth;



	}


	#endregion


	#region Interact
	public void OnCall()
	{
		if (interactType == InteractType.Busy)
			return;
		
		Abort ();
		if (enviromentType == EnviromentType.Room) {
			charAnim.SetAnimType (AnimType.Idle);
			InputController.instance.SetTarget (PointType.Call);
			StartCoroutine (MoveToPointFocus ());
			interactType = InteractType.Call;
		}
	}

	public void OnFollowTarget()
	{
		if (interactType == InteractType.Busy)
			return;
		
		Abort ();
		interactType = InteractType.FollowTarget;
	}

	void OnHold()
	{
		Abort ();
		actionType = ActionType.Hold;
		SetDirection (Direction.D);
		enviromentType = EnviromentType.Room;
	}

	public void OnBath(){
		actionType = ActionType.Bath;
	}

	public void OnFingerTouchUp(Vector2 delta)
	{
		float angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
		if (isTouch && angle > -45 && angle < 45 && actionType != ActionType.Hold) {
			touchDirection = Direction.U;
			OnHold ();
		}else if(isTouch && (angle > 115 || angle < -115)) {
			touchDirection = Direction.D;
		}else if(isTouch && (angle > 45 && angle < 115)) {
			touchDirection = Direction.L;
		}else if(isTouch && (angle > 115 && angle < 205)) {
			touchDirection = Direction.R;
		}
	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}
		isTouch = true;
	}

	void OnMouseUp()
	{
		isTouch = false;
	}


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}



	#endregion

	#region Thinking
	void Think()
	{
		isAbort = false;
		isEndAction = false;
		actionType = ActionType.None;

		if (data.Shit > data.maxShit * 0.9f) {
			actionType = ActionType.Shit;
			DoAction ();
			return;
		}

		if (data.Pee > data.maxPee * 0.9f) {
			actionType = ActionType.Pee;
			DoAction ();
			return;
		}

		if (data.Health < data.maxHealth * 0.1f) {
			actionType = ActionType.Sick;
			DoAction ();
			return;
		}

		if (data.Food < data.maxFood * 0.2f) {
			int ran = Random.Range (0, 100);
			if (ran > 30) {
				actionType = ActionType.Eat;
				DoAction ();
				return;
			}
		}

		if (data.Water < data.maxWater * 0.2f) {
			int ran = Random.Range (0, 100);
			if (ran > 30) {
				actionType = ActionType.Drink;
				DoAction ();
				return;
			}
		}

		if (data.Sleep < data.maxSleep * 0.1f) {
			int ran = Random.Range (0, 100);
			if (ran > 30) {
				actionType = ActionType.Sleep;
				DoAction ();
				return;
			}
		}

		if (data.Dirty > data.maxDirty * 0.7f) {
			int ran = Random.Range (0, 100);
			if (ran > 50) {
				actionType = ActionType.Itchi;
				DoAction ();
				return;
			}
		}

		if (data.Energy < data.maxEnergy * 0.1f) {
			actionType = ActionType.Rest;
			DoAction ();
			return;
		}

		if (data.Stamina < data.maxStamina * 0.3f) {
			actionType = ActionType.Discover;
			DoAction ();
			return;
		}


		//Other Action
		int id = Random.Range (0,100);
		if (id < 20) {
			actionType = ActionType.None;
		} else if (id < 40) {
			actionType = ActionType.Rest;
		} else if (id < 60) {
			actionType = ActionType.Patrol;
		} else {
			actionType = ActionType.Discover;
		}
		DoAction ();
	}


	void DoAction()
	{

	}
	#endregion


	#region Basic Action
	//Basic Action
	public void Abort()
	{
		//actionType = ActionType.None;
		isAbort = true;
		isEndAction = true;
	}

	void RandomDirection()
	{

	}

	void SetDirection(Direction d)
	{
		if (d == Direction.D) {
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
		}else if(d == Direction.U)
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -180));
		else if(d == Direction.RD)
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -140));
		else if(d == Direction.RU)
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -40));
		else if(d == Direction.LD)
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 140));
		else if(d == Direction.LU)
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 40));
	}

	IEnumerator DoAnim(string a)
	{
		float time = 0;
		anim.Play (a, 0);
		yield return new WaitForEndOfFrame ();
		while (time < anim.GetCurrentAnimatorStateInfo (0).length && !isAbort) {
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator MoveToPoint()
	{
		isArrived = false;
		if (Vector2.Distance (target.position, agent.transform.position) > 0.5f) {
			lastTargetPosition = target.position;
			agent.SetDestination (target.position);
			while (!isArrived && !isAbort) {
				charAnim.SetAnimType (AnimType.Run);
				yield return new WaitForEndOfFrame ();
				if (isAbort)
					agent.Stop ();
			}
		} else {
			isArrived = true;
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator MoveToPointCaress()
	{
		isArrived = false;
		if (Vector2.Distance (target.position, agent.transform.position) > 0.5f) {
			lastTargetPosition = target.position;
			agent.SetDestination (target.position);
			charAnim.SetAnimType (AnimType.Idle);
			while (!isArrived && interactType == InteractType.Caress) {
				SetDirection (Direction.D);
				charAnim.SetAnimType (AnimType.Run);
				yield return new WaitForEndOfFrame ();
				if (interactType != InteractType.Caress)
					agent.Stop ();
			}
		} else {
			isArrived = true;
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator MoveToPointFocus()
	{
		isArrived = false;
		Debug.Log ((Vector2.Distance (target.position, agent.transform.position) < 0.5f));
		if (Vector2.Distance (target.position, agent.transform.position) > 0.5f) {
			lastTargetPosition = target.position;
			agent.SetDestination (target.position);
			while (!isArrived) {
				charAnim.SetAnimType (AnimType.Run);
				yield return new WaitForEndOfFrame ();
			}
		} else {
			isArrived = true;
			yield return new WaitForEndOfFrame ();
		}
	}




	IEnumerator Wait(float maxT)
	{
		float time = 0;

		int ran = Random.Range (0, 100);
		if (ran < 30) {
			charAnim.SetAnimType (AnimType.Bath);
		} 
		else if(ran < 70)
			charAnim.SetAnimType (AnimType.Idle);
		else 
			charAnim.SetAnimType (AnimType.Sit);
		

		while (time < maxT && !isAbort) {
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}
	#endregion

	#region Main Action
	IEnumerator None(){
		yield return Wait (Random.Range (1, 3));
		isEndAction = true;
	}


	IEnumerator Patrol(){
		yield return new WaitForEndOfFrame ();
		isEndAction = true;
	}

	IEnumerator Discover()
	{
		int n = 0;
		int maxCount = Random.Range (4, 10);
		while(!isAbort && n < maxCount) 
		{
			if(!isAbort)
			{
				InputController.instance.SetTarget (PointType.Favourite);
				yield return StartCoroutine (MoveToPoint ());
			}
			if (!isAbort) {
				int ran = Random.Range (0, 100);
				if (ran < 30) {
					charAnim.SetAnimType (AnimType.Bath);
				} else 
					charAnim.SetAnimType (AnimType.Idle);
				yield return StartCoroutine (Wait (Random.Range (1, 3)));
			}

			n++;
		}
		isEndAction = true;
	}

	IEnumerator Pee()
	{
		interactType = InteractType.Busy;
		charAnim.SetAnimType (AnimType.Pee);
		Debug.Log ("Pee");
		SpawnPee ();
		while (data.Pee > 1 && !isAbort) {
			data.Pee -= 0.5f;
			yield return new WaitForEndOfFrame();
		}
		interactType = InteractType.None;
		isEndAction = true;
	}

	IEnumerator Shit()
	{
		interactType = InteractType.Busy;
		charAnim.SetAnimType (AnimType.Shit);
		SpawnShit ();
		while (data.Shit > 1 && !isAbort) {
			data.Shit -= 0.5f;
			yield return new WaitForEndOfFrame();
		}
		interactType = InteractType.None;
		isEndAction = true;
	}

	IEnumerator Eat()
	{
		Debug.Log ("Eat");
		if (!isAbort) {
			InputController.instance.SetTarget (PointType.Eat);
			yield return StartCoroutine (MoveToPoint ());
		}
		bool canEat = true;
		if (ItemController.instance.foodBowl.CanEat ()) {
			charAnim.SetAnimType (AnimType.Eat);
			yield return new WaitForSeconds (0.1f);
			while (data.Food < data.maxFood && !isAbort && canEat) {
				data.Food += 0.3f;
				ItemController.instance.foodBowl.Eat (0.3f);
				if (!ItemController.instance.foodBowl.CanEat ()) {
					canEat = false;
				}
				if (Vector2.Distance (this.transform.position, ItemController.instance.foodBowl.anchor.position) > 0.5f)
					canEat = false;
				yield return new WaitForEndOfFrame ();
			}
		}
		isEndAction = true;
	}

	IEnumerator Drink()
	{
		
		Debug.Log ("Drink");
		if (!isAbort) {
			InputController.instance.SetTarget (PointType.Drink);
			yield return StartCoroutine (MoveToPoint ());
		}

		bool canDrink = true;

		if (ItemController.instance.waterBowl.CanEat ()) {
			charAnim.SetAnimType (AnimType.Eat);
			yield return new WaitForSeconds (0.1f);
			while (data.Water < data.maxWater && !isAbort && canDrink) {
				data.Water += 0.5f;
				ItemController.instance.waterBowl.Eat (0.5f);
				if (!ItemController.instance.waterBowl.CanEat ()) {
					canDrink = false;
				}
				if (Vector2.Distance (this.transform.position, ItemController.instance.waterBowl.anchor.position) > 0.5f)
					canDrink = false;
				yield return new WaitForEndOfFrame ();
			}
		}
		isEndAction = true;
	}

	IEnumerator Sleep()
	{
		
		Debug.Log ("Sleep");
		InputController.instance.SetTarget (PointType.Sleep);
		yield return StartCoroutine (MoveToPoint ());
		charAnim.SetAnimType (AnimType.Sleep);
		while (data.Sleep < data.maxSleep && !isAbort) {
			data.Sleep += 0.01f;
			yield return new WaitForEndOfFrame();
		}
		isEndAction = true;
	}

	IEnumerator Rest()
	{
		float time = 0;
		float maxTime = Random.Range (2, 5);
		charAnim.SetAnimType (AnimType.Sit);
		Debug.Log ("Rest");
		while (!isAbort && time < maxTime) {
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		isEndAction = true;
	}

	IEnumerator Itchi(){
		charAnim.SetAnimType (AnimType.Itchi);
		Debug.Log ("Itchi");
		yield return new WaitForEndOfFrame ();
		isEndAction = true;
	}

	IEnumerator Sick(){
		interactType = InteractType.Busy;
		charAnim.SetAnimType (AnimType.Sick);
		Debug.Log ("Sick");
		while (data.health < 0.1f*data.maxHealth  && !isAbort) {
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame ();
		isEndAction = true;
	}

	#endregion

	#region Event
	public void OnArrived()
	{
		Debug.Log ("Arrived");

		if (interactType == InteractType.FollowTarget) {
			interactType = InteractType.None;
			agent.speed = 30;
			anim.speed = 1;
		}

		if(interactType != InteractType.Caress)
			charAnim.SetAnimType (AnimType.Idle);
		isArrived = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Mouse") {
			if ((interactType == InteractType.None || interactType == InteractType.Caress) && interactType != InteractType.Busy) {
				interactType = InteractType.FollowTarget;
				target = other.transform;
				Abort ();
			}
		} else if (other.tag == "Food") {

		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Mouse" && interactType == InteractType.FollowTarget) {
			interactType = InteractType.None;
		}else if (other.tag == "Food") {

		}
	}
	#endregion

	#region Effect
	void SpawnPee()
	{
		GameObject go = GameObject.Instantiate (peePrefab, peePosition.position + new Vector3 (0, 0, 50), Quaternion.identity);
	}

	void SpawnShit(){
		GameObject go = GameObject.Instantiate (shitPrefab, shitPosition.position, Quaternion.identity);
	}


	#endregion

}

public enum InteractType {None,FollowTarget,Drag,Drop,Caress,Call,Bath,Command,Busy,Listening,Fall,Touch};
public enum EnviromentType {Room,Table,Bath};
public enum ActionType {None,Rest,Sleep,Eat,Drink,Patrol,Discover,Pee,Shit,Itchi,Sick,Sad,Fear,Happy,Tired,Call,Hold,OnTable,Bath}