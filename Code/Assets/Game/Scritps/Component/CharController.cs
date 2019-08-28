using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CharController : MonoBehaviour {

	//[HideInInspector]
	public InteractType interactType;
	//[HideInInspector]
	public EnviromentType enviromentType = EnviromentType.Room;
	//[HideInInspector]
	public Direction direction;

	//Movement
	public Transform target;
	PolyNavAgent agent;
	float agentTime;
	float maxAgentTime = 0.1f;
	Vector2 lastTargetPosition;
	bool isArrived = true;
	bool isAbort = false;

	//Action
	public ActionType actionType = ActionType.None;
	public float actionTime;
	public float maxActionTime = 1;


	//Anim
	string moveAnim = "Run";
	string idleAnim = "Idle";
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

		if (interactType == InteractType.None) {
			if (enviromentType == EnviromentType.Room) {
				if (actionTime > maxActionTime) {
					Abort ();
					Think ();
				} else
					actionTime += Time.deltaTime;
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

			anim.Play (moveAnim + "_" + direction.ToString (), 0);
			anim.speed = agent.speed / agent.maxSpeed;
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

			agent.transform.position = pos + new Vector3 (0, 0, -10);
			anim.Play ("Hold_D", 0);
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

				if (enviromentType == EnviromentType.Bath)
					interactType = InteractType.Bath;
				else
					interactType = InteractType.None;
				
				fallSpeed = 0;
				this.transform.rotation = Quaternion.identity;
				anim.Play ("Idle_D", 0);
			}
		} else if (interactType == InteractType.Bath) {
			anim.Play ("Idle_D", 0);
		} else if (interactType == InteractType.Caress) {
			if (interactTime > maxInteractTime) {
				Think ();
			} else {
				interactTime += Time.deltaTime;
			}
		}


		//Check Agent
		if (agent.transform.eulerAngles.z < 45 && agent.transform.eulerAngles.z > -45 || (agent.transform.eulerAngles.z > 315 && agent.transform.eulerAngles.z < 405) || (agent.transform.eulerAngles.z < -315 && agent.transform.eulerAngles.z > -405))
			direction = Direction.U;
		else if ((agent.transform.eulerAngles.z >= 45 && agent.transform.eulerAngles.z <= 135) || (agent.transform.eulerAngles.z <= -225 && agent.transform.eulerAngles.z >= -315))
			direction = Direction.L;
		else if ((agent.transform.eulerAngles.z <= -45 && agent.transform.eulerAngles.z >= -135) || (agent.transform.eulerAngles.z >= 225 && agent.transform.eulerAngles.z <= 315))
			direction = Direction.R;
		else
			direction = Direction.D;

		this.transform.position = agent.transform.position;

//		if (agent.transform.eulerAngles.z < 22.5f && agent.transform.eulerAngles.z > -22.5f || (agent.transform.eulerAngles.z > 337.5f && agent.transform.eulerAngles.z < 382.5f) || (agent.transform.eulerAngles.z < -337.5f && agent.transform.eulerAngles.z > -382.5f))
//			direction = Direction.U;
//		else if ((agent.transform.eulerAngles.z > 22.5f && agent.transform.eulerAngles.z < 67.5f) || (agent.transform.eulerAngles.z > -337.5f && agent.transform.eulerAngles.z < -292.5f))
//			direction = Direction.LU;
//		else if ((agent.transform.eulerAngles.z >= 67.5f && agent.transform.eulerAngles.z <= 112.5f) || (agent.transform.eulerAngles.z >= -292.5f && agent.transform.eulerAngles.z <= -247.5f))
//			direction = Direction.L;
//		else if ((agent.transform.eulerAngles.z >= 112.5f && agent.transform.eulerAngles.z <= 157.5f) || (agent.transform.eulerAngles.z >= -247.5f && agent.transform.eulerAngles.z <= -202.5f))
//			direction = Direction.LD;
//		else if ((agent.transform.eulerAngles.z <= -22.5f && agent.transform.eulerAngles.z >= -67.5) || (agent.transform.eulerAngles.z >= 247.5f && agent.transform.eulerAngles.z <= 292.5f))
//			direction = Direction.RU;
//		else if ((agent.transform.eulerAngles.z <= -67.5f && agent.transform.eulerAngles.z >= -112.5f) || (agent.transform.eulerAngles.z >= 202.5f && agent.transform.eulerAngles.z <= 257.5f))
//			direction = Direction.R;
//		else if ((agent.transform.eulerAngles.z <= -112.5 && agent.transform.eulerAngles.z >= -157.5f) || (agent.transform.eulerAngles.z >= 257.5f && agent.transform.eulerAngles.z <= 302.5f))
//			direction = Direction.RD;
//		else
//			direction = Direction.D;
	}


	#region Interact
	public void OnCall()
	{
		Abort ();
		if (enviromentType == EnviromentType.Room) {
			StartCoroutine (MoveToPoint ( InputController.instance.GetRandomPoint (PointType.Call).position));
			interactType = InteractType.Call;
		}
	}

	public void OnFollowTarget()
	{
		Abort ();
		interactType = InteractType.FollowTarget;
	}

	void OnDrag()
	{
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
		anim.Play ("Idle_D", 0);
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

	#region Action

	void Free()
	{
		interactType = InteractType.None;
		Think ();
	}

	void Think()
	{
		ResetInteract ();
		isAbort = false;
		int id = Random.Range (1, 100);
		if (id < 50) {
			actionType = ActionType.None;
			maxActionTime = Random.Range (2, 5);
		} else {
			actionType = ActionType.Discover;
			maxActionTime = Random.Range (20, 50);
		}

		DoAction ();
	}


	void DoAction()
	{
		if (actionType == ActionType.None) {
			anim.Play (idleAnim + "_D", 0);
		} else if (actionType == ActionType.Discover) {
			StartCoroutine (Discover ());
		}
	}

	public void Abort()
	{
		actionTime = 0;
		actionType = ActionType.None;
		isAbort = true;
	}



	IEnumerator Discover()
	{
		while(!isAbort) 
		{
			if(!isAbort)
				yield return StartCoroutine (MoveToPoint (InputController.instance.GetRandomPoint (PointType.Favourite).position));
			
			if (!isAbort) {
				anim.Play ("Idle_D");
				yield return StartCoroutine (Wait (Random.Range (1, 3)));
			}
		}
	}


	IEnumerator RandomDirection()
	{
		yield return null;
	}


	IEnumerator Wait(float maxT)
	{
		float time = 0;
		while (time < maxT && !isAbort) {
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}


	IEnumerator MoveToPoint(Vector3 pos)
	{
		isArrived = false;
		if (Vector2.Distance (pos, lastTargetPosition) > 0.5f) {
			lastTargetPosition = pos;
			agent.SetDestination (pos);
			while (!isArrived && !isAbort) {
				anim.Play (moveAnim + "_" + direction.ToString (), 0);
				if (isAbort)
					agent.Stop ();
				yield return new WaitForEndOfFrame ();
			}
		} else {
			isArrived = true;
			yield return new WaitForEndOfFrame ();
		}



	}

	void SetDirection(Direction d)
	{
		if (d == Direction.D) {
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
		}
	}
	#endregion

	#region Event
	public void OnArrived()
	{
		if (interactType == InteractType.FollowTarget) {
			Free ();
		}

		isArrived = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Mouse") {
			if (interactType == InteractType.None || interactType == InteractType.Caress) {
				interactType = InteractType.FollowTarget;
				target = other.transform;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Mouse" && interactType == InteractType.FollowTarget) {
			Free ();
		}
	}
	#endregion

}

public enum InteractType {None,FollowTarget,Drag,Drop,Caress,Call,Bath,Command};
public enum EnviromentType {Room,Table,Bath};
public enum ActionType {None,Sleepy,Hungry,Patrol,Discover,FollowTarget,Listening,Pee,Shit,Itchiness,Tired,Sickness}
public enum EmotionType {None,Sad,Fear,Happy,Supprise,mad}