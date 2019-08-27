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
	float time;
	float maxAgentTime = 0.1f;
	Vector2 lastTargetPosition;

	//Anim
	string moveAnim = "Run";
	string idleAnim = "Idle";
	Animator anim;

	//Drag Drop
	bool isTouch = false;
	Vector3 dragOffset;
	Vector3 dropPosition;
	Rigidbody2D rigid;
	CircleCollider2D collider;
	float fallSpeed = 0;

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
	void Update () {



		if (interactType == InteractType.None) {
			if (enviromentType == EnviromentType.Room)
				anim.Play (idleAnim + "_" + direction.ToString (), 0);
		}
		else if (interactType == InteractType.MoveToTarget) {
			if (time > maxAgentTime) {
				if (target != null) {
					if (Vector2.Distance (lastTargetPosition, target.position) > 1) {
						agent.SetDestination (target.position);
						lastTargetPosition = target.position;
						interactType = InteractType.MoveToTarget;
					}
				}
				time = 0;
			} else
				time += Time.deltaTime;

			anim.Play (moveAnim + "_" + direction.ToString (), 0);
			anim.speed = agent.speed / agent.maxSpeed;
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
			if (Vector2.Distance (agent.transform.position, dropPosition) < 0.5f) {

				if (enviromentType == EnviromentType.Bath)
					interactType = InteractType.Bath;
				else
					interactType = InteractType.None;
				
				fallSpeed = 0;
				this.transform.rotation = Quaternion.identity;
				anim.Play ("Idle_D", 0);
			}
		}else if (interactType == InteractType.Bath)
		{
			anim.Play ("Idle_D", 0);
		}



		if (agent.transform.eulerAngles.z < 45 && agent.transform.eulerAngles.z > -45 || (agent.transform.eulerAngles.z > 315 && agent.transform.eulerAngles.z < 405) || (agent.transform.eulerAngles.z < -315 && agent.transform.eulerAngles.z > -405))
			direction = Direction.U;
		else if ((agent.transform.eulerAngles.z >= 45 && agent.transform.eulerAngles.z <= 135) || (agent.transform.eulerAngles.z <= -225 && agent.transform.eulerAngles.z >= -315))
			direction = Direction.L;
		else if ((agent.transform.eulerAngles.z <= -45 && agent.transform.eulerAngles.z >= -135) || (agent.transform.eulerAngles.z >= 225 && agent.transform.eulerAngles.z <= 315))
			direction = Direction.R;
		else
			direction = Direction.D;
		this.transform.position = agent.transform.position;

	}




	#region Actionn
	public void OnArrived()
	{
		if (interactType == InteractType.MoveToTarget) {
			interactType = InteractType.None;
		} else if (interactType == InteractType.Call) {
			SetDirection (Direction.D);
		}
	}

	public void OnCall()
	{
		if (Vector2.Distance (lastTargetPosition, target.position) < 0.1f) {
			return;
		}
		interactType = InteractType.MoveToTarget;
	}

	public void OnMove()
	{
		interactType = InteractType.MoveToTarget;
	}


	#endregion


	#region Interact


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
	}

	public void OnFingerTouchUp(Vector2 delta)
	{
		float angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
		if (isTouch && angle > -45 && angle < 45 && interactType != InteractType.Drop) {
			interactType = InteractType.Drag;
			SetDirection (Direction.D);
			enviromentType = EnviromentType.Room;
		}else if(isTouch && (angle > 115 || angle < -115) && interactType == InteractType.Call) {
			anim.Play ("Lay_D", 0);
			interactType = InteractType.Caress;
			SetDirection (Direction.D);
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

			Debug.Log (angle);

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

	#region Direction

	#endregion

	#region Collider

	void SetDirection(Direction d)
	{
		if (d == Direction.D) {
			agent.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 180));
		}
	}
	#endregion

	#region Movement



	#endregion

}

public enum InteractType {None,MoveToTarget,Drag,Drop,Caress,Call,Bath,Command};
public enum EnviromentType {Room,Table,Bath};