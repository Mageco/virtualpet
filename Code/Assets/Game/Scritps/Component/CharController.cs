using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CharController : MonoBehaviour {


	//Data
	public CharData data;
	//[HideInInspector]
	public InteractType interactType;
	//[HideInInspector]
	public EnviromentType enviromentType = EnviromentType.Room;
	//[HideInInspector]
	public Direction direction;

	//Think
	float dataTime;
	float maxDataTime = 0.1f;

	//Movement
	public Transform target;
	PolyNavAgent agent;
	float agentTime;
	float maxAgentTime = 0.1f;
	Vector2 lastTargetPosition;
	public bool isArrived = true;
	public bool isAbort = false;

	//Action
	public ActionType actionType = ActionType.None;
	public bool isEndAction = false;


	//Anim
	string moveAnim = "Run";
	string idleAnim = "Idle";
	string dropAnim = "Drop_Light_D";
	string holdAnim = "Hold_D";
	string layAnim = "Lay_D";
	string eatAnim = "Eat_LD";
	string drinkAnim = "Eat_LD";
	string sheetAnim = "Poop_D";
	string sleepAnim = "Sleep_LD";
	string peeAnim = "Pee_D";

	Animator anim;

	//Interact
	bool isTouch = false;
	Vector3 dragOffset;
	Vector3 dropPosition;
	Rigidbody2D rigid;
	CircleCollider2D collider;
	float fallSpeed = 0;
	float interactTime = 0;
	float maxInteractTime = 3;

	//Pee,Sheet
	public Transform peePosition;
	public Transform shitPosition;
	public GameObject peePrefab;
	public GameObject shitPrefab;

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
		agent = GameObject.FindObjectOfType<PolyNavAgent> ();
		rigid = this.GetComponent <Rigidbody2D> ();
		collider = this.GetComponent <CircleCollider2D> ();
	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {

		if (interactType == InteractType.None || interactType == InteractType.Busy) {
			if (enviromentType == EnviromentType.Room) {
				if (isEndAction) {
					Think ();
				} 
			}
		} else if (interactType == InteractType.FollowTarget) {
			if (agentTime > maxAgentTime) {
				if (target != null) {
					if (Vector2.Distance (lastTargetPosition, target.position) > 1) {
						agent.SetDestination (target.position);
						lastTargetPosition = target.position;
					}
				}
				agentTime = 0;
			} else
				agentTime += Time.deltaTime;

			agent.speed = 40;
			anim.Play (moveAnim + "_" + direction.ToString (), 0);
			anim.speed = 1.3f;
		} else if (interactType == InteractType.Call) {
			if (isArrived) {
				anim.Play (idleAnim + "_" + direction.ToString (), 0);
				SetDirection (Direction.D);
				interactType = InteractType.Caress;
				maxInteractTime = Random.Range (5, 10);
			}
		} else if (interactType == InteractType.Drag) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition) - dragOffset;
			pos.z = 0;
			if (pos.y > 20)
				pos.y = 20;
			else if (pos.y < -20)
				pos.y = -20;

			if (pos.x > 35)
				pos.x = 35;
			else if (pos.x < -28)
				pos.x = -28;

			pos.z = -50;
			agent.transform.position = pos;
			anim.Play (holdAnim, 0);
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, Quaternion.identity, Time.deltaTime * 2);

		} else if (interactType == InteractType.Drop) {
			fallSpeed += 100f * Time.deltaTime;
			if (fallSpeed > 50)
				fallSpeed = 50;
			Vector3 pos = agent.transform.position;
			pos.y -= fallSpeed * Time.deltaTime;
			pos.z = dropPosition.z;
			agent.transform.position = pos;
			if (Vector2.Distance (agent.transform.position, dropPosition) < 1f) {

				if (fallSpeed < 50) {
					dropAnim = "Drop_Light_D";
					maxInteractTime = 2;
				} else {
					dropAnim = "Drop_Hard_D";
					maxInteractTime = 3;
				}

				interactTime = 0;
				maxInteractTime = 2;
				fallSpeed = 0;
				this.transform.rotation = Quaternion.identity;
				anim.Play (dropAnim, 0);
				interactType = InteractType.Fall;
			}
		} else if (interactType == InteractType.Fall) {
			if (interactTime > maxInteractTime) {
				if (enviromentType == EnviromentType.Bath)
					interactType = InteractType.Bath;
				else
					interactType = InteractType.None;
			} else {
				interactTime += Time.deltaTime;
			}
		}
		else if (interactType == InteractType.Bath) {
			anim.Play (idleAnim + "_" + direction.ToString (), 0);
		} else if (interactType == InteractType.Caress) {
			if (interactTime > maxInteractTime) {
				interactType = InteractType.None;
			} else {
				interactTime += Time.deltaTime;
			}
		}


		//Check Agent
//		if (agent.transform.eulerAngles.z < 45 && agent.transform.eulerAngles.z > -45 || (agent.transform.eulerAngles.z > 315 && agent.transform.eulerAngles.z < 405) || (agent.transform.eulerAngles.z < -315 && agent.transform.eulerAngles.z > -405))
//			direction = Direction.U;
//		else if ((agent.transform.eulerAngles.z >= 45 && agent.transform.eulerAngles.z <= 135) || (agent.transform.eulerAngles.z <= -225 && agent.transform.eulerAngles.z >= -315))
//			direction = Direction.L;
//		else if ((agent.transform.eulerAngles.z <= -45 && agent.transform.eulerAngles.z >= -135) || (agent.transform.eulerAngles.z >= 225 && agent.transform.eulerAngles.z <= 315))
//			direction = Direction.R;
//		else
//			direction = Direction.D;



		if (agent.transform.eulerAngles.z < 30f && agent.transform.eulerAngles.z > -30f || (agent.transform.eulerAngles.z > 330f && agent.transform.eulerAngles.z < 390f) || (agent.transform.eulerAngles.z < -330f && agent.transform.eulerAngles.z > -390f))
			direction = Direction.U;
		else if ((agent.transform.eulerAngles.z > 30f && agent.transform.eulerAngles.z < 80f) || (agent.transform.eulerAngles.z > -330f && agent.transform.eulerAngles.z < -280f))
			direction = Direction.LU;
//		else if ((agent.transform.eulerAngles.z >= 67.5f && agent.transform.eulerAngles.z <= 112.5f) || (agent.transform.eulerAngles.z >= -292.5f && agent.transform.eulerAngles.z <= -247.5f))
//			direction = Direction.L;
		else if ((agent.transform.eulerAngles.z >= 80f && agent.transform.eulerAngles.z <= 150f) || (agent.transform.eulerAngles.z >= -280f && agent.transform.eulerAngles.z <= -210f))
			direction = Direction.LD;
		else if ((agent.transform.eulerAngles.z <= -30f && agent.transform.eulerAngles.z >= -80f) || (agent.transform.eulerAngles.z >= 280f && agent.transform.eulerAngles.z <= 330f))
			direction = Direction.RU;
//		else if ((agent.transform.eulerAngles.z <= -67.5f && agent.transform.eulerAngles.z >= -112.5f) || (agent.transform.eulerAngles.z >= 202.5f && agent.transform.eulerAngles.z <= 257.5f))
//			direction = Direction.R;
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

		deltaHealth += (data.Happy - data.maxHappy * 0.3f) + 0.001f;

		if (data.Dirty > data.maxDirty * 0.8f)
			deltaHealth -= (data.Dirty - data.maxDirty * 0.8f) * 0.03f;

		if(data.Pee > data.maxPee * 0.9f)
			deltaHealth -= (data.Pee - data.maxPee * 0.9f) * 0.01f;

		if(data.Shit > data.maxShit * 0.9f)
			deltaHealth -= (data.Shit - data.maxShit * 0.9f) * 0.02f;

		if(data.Food < data.maxFood * 0.1f)
			deltaHealth -= (data.maxFood * 0.1f - data.Food) * 0.01f;

		if(data.Water < data.maxWater * 0.1f)
			deltaHealth -= (data.maxWater * 0.1f - data.Water) * 0.01f;
		
		if(data.Sleep < data.maxSleep * 0.05f)
			deltaHealth -= (data.maxSleep * 0.05f - data.Sleep) * 0.04f;

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
			InputController.instance.SetTarget (PointType.Call);
			StartCoroutine (MoveToPoint ());
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

	void OnDrag()
	{
		if (interactType == InteractType.FollowTarget)
			return;
		
		Abort ();
		interactType = InteractType.Drag;
		SetDirection (Direction.D);
		enviromentType = EnviromentType.Room;
	}

	void OnDrop()
	{
		Abort ();
		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0,-2,0), -Vector2.up,100);

		Vector3 pos = this.transform.position;
		pos.y = pos.y - 22;
		if (pos.y < -20)
			pos.y = -20;
		dropPosition = pos;
		enviromentType = EnviromentType.Room;

		for (int i = 0; i < hit.Length; i++) {
			if (hit[i].collider.tag == "Table") {
				pos.y = hit[i].collider.transform.position.y;
				dropPosition = pos;
				enviromentType = EnviromentType.Table;
				break;
			}else if(hit[i].collider.tag == "Bath") {
				pos.y = hit[i].collider.transform.position.y;
				pos.z = hit [i].collider.transform.position.z;
				dropPosition = pos;
				enviromentType = EnviromentType.Bath;
				break;
			}
		}

		interactType = InteractType.Drop;
	}

	void OnLay()
	{
		anim.Play (layAnim, 0);
		interactTime = 0;
	}

	public void ResetInteract()
	{
		interactTime = 0;
		interactType = InteractType.None;
	}


	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}
		dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
		isTouch = true;

	}

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		isTouch = false;
		if (interactType == InteractType.Drag) {
			OnDrop ();
		}
	}

	public void OnFingerTouchUp(Vector2 delta)
	{
		float angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
		if (isTouch && angle > -45 && angle < 45 && interactType != InteractType.Drop) {
			OnDrag ();
		}else if(isTouch && (angle > 115 || angle < -115) && interactType == InteractType.Caress) {
			OnLay ();
		}
	}

	public void OnFingerSwipe(LeanFinger finger)
	{
		if (interactType == InteractType.Drag) {
			float angle = finger.ScreenDelta.x;
			if (angle > 30)
				angle = 30;
			if (angle < -30)
				angle = -30;

			this.transform.rotation = Quaternion.Euler (0, 0, -angle);
		}
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
		ResetInteract ();
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
			actionType = ActionType.Eat;
			DoAction ();
			return;
		}

		if (data.Water < data.maxWater * 0.2f) {
			actionType = ActionType.Drink;
			DoAction ();
			return;
		}

		if (data.Sleep < data.maxSleep * 0.1f) {
			actionType = ActionType.Sleep;
			DoAction ();
			return;
		}

		if (data.Dirty > data.maxDirty * 0.7f) {
			actionType = ActionType.Itchi;
			DoAction ();
			return;
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
		} else if (id < 50) {
			actionType = ActionType.Rest;
		} else if (id < 80) {
			actionType = ActionType.Patrol;
		} else {
			actionType = ActionType.Discover;
		}
		DoAction ();
	}


	void DoAction()
	{
		if (actionType == ActionType.None) {
			StartCoroutine (None ());
		} else if (actionType == ActionType.Rest) {
			StartCoroutine (Rest ());
		} else if (actionType == ActionType.Patrol) {
			StartCoroutine (Patrol ());
		} else if (actionType == ActionType.Pee) {
			StartCoroutine (Pee ());
		} else if (actionType == ActionType.Shit) {
			StartCoroutine (Shit ());
		} else if (actionType == ActionType.Eat) {
			StartCoroutine (Eat ());
		} else if (actionType == ActionType.Drink) {
			StartCoroutine (Drink ());
		} else if (actionType == ActionType.Sleep) {
			StartCoroutine (Sleep ());
		} else if (actionType == ActionType.Itchi) {
			StartCoroutine (Itchi ());
		} else if (actionType == ActionType.Sick) {
			StartCoroutine (Sick ());
		}else if (actionType == ActionType.Discover) {
			StartCoroutine (Discover ());
		}
	}
	#endregion


	#region Basic Action
	//Basic Action
	public void Abort()
	{
		actionType = ActionType.None;
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
		}
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
		Debug.Log ((Vector2.Distance (target.position, agent.transform.position) < 0.5f));
		if (Vector2.Distance (target.position, agent.transform.position) > 0.5f) {
			lastTargetPosition = target.position;
			agent.SetDestination (target.position);
			while (!isArrived) {
				anim.Play (moveAnim + "_" + direction.ToString (), 0);
				yield return new WaitForEndOfFrame ();
			}
		} else {
			isArrived = true;
			yield return new WaitForEndOfFrame ();
		}
	}

	IEnumerator Turn()
	{
		yield return new WaitForEndOfFrame ();
	}


	IEnumerator Wait(float maxT)
	{
		float time = 0;
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


	IEnumerator Listening(){
		yield return new WaitForEndOfFrame ();
		isEndAction = true;
	}

	IEnumerator Patrol(){
		yield return new WaitForEndOfFrame ();
		isEndAction = true;
	}

	IEnumerator Discover()
	{
		int n = 0;
		int maxCount = Random.Range (2, 5);
		while(!isAbort && n < maxCount) 
		{
			if(!isAbort)
			{
				InputController.instance.SetTarget (PointType.Favourite);
				yield return StartCoroutine (MoveToPoint ());
			}
			if (!isAbort) {
				anim.Play (idleAnim + "_" + direction.ToString ());
				yield return StartCoroutine (Wait (Random.Range (1, 3)));
			}

			n++;
		}
		isEndAction = true;
	}

	IEnumerator Pee()
	{
		interactType = InteractType.Busy;
		anim.Play (peeAnim, 0);
		Debug.Log ("Pee");
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
		anim.Play (sheetAnim, 0);
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
		anim.Play (eatAnim, 0);
		while (data.Food < data.maxFood && !isAbort) {
			data.Food += 0.3f;
			yield return new WaitForEndOfFrame();
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

		anim.Play (drinkAnim, 0);
		while (data.Water < data.maxWater && !isAbort) {
			data.Water += 0.5f;
			yield return new WaitForEndOfFrame();
		}
		isEndAction = true;
	}

	IEnumerator Sleep()
	{
		
		Debug.Log ("Sleep");
		InputController.instance.SetTarget (PointType.Sleep);
		yield return StartCoroutine (MoveToPoint ());
		anim.Play (sleepAnim, 0);
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
		anim.Play (idleAnim + "_" + direction.ToString (), 0);
		Debug.Log ("Rest");
		while (!isAbort && time < maxTime) {
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		isEndAction = true;
	}

	IEnumerator Itchi(){
		anim.Play (idleAnim + "_" + direction.ToString (), 0);
		Debug.Log ("Itchi");
		yield return new WaitForEndOfFrame ();
		isEndAction = true;
	}

	IEnumerator Sick(){
		anim.Play (idleAnim + "_" + direction.ToString (), 0);
		Debug.Log ("Sick");
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

		isArrived = true;
		anim.Play (idleAnim + "_" + direction.ToString (), 0);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Mouse") {
			if ((interactType == InteractType.None || interactType == InteractType.Caress) && interactType != InteractType.Busy ) {
				interactType = InteractType.FollowTarget;
				target = other.transform;
				Abort ();
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Mouse" && interactType == InteractType.FollowTarget) {
			interactType = InteractType.None;
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

public enum InteractType {None,FollowTarget,Drag,Drop,Caress,Call,Bath,Command,Busy,Listening,Fall};
public enum EnviromentType {Room,Table,Bath};
public enum ActionType {None,Rest,Sleep,Eat,Drink,Patrol,Discover,Pee,Shit,Itchi,Sick,Sad,Fear,Happy,Supprise,Mad}