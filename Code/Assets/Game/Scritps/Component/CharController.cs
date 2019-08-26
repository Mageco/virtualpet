using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharController : MonoBehaviour {

	public bool isMove = false;
	public Transform target;
	public PolyNavAgent agent;
	float time;
	float maxTime = 0.1f;
	Vector2 lastPosition;
	Direction direction;
	string moveAnim = "run";
	string idleAnim = "idle";
	Animator anim;
	ActionType actionState;
	public Transform[] points;

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (time > maxTime) {
			if (target != null) {
				if (Vector2.Distance (lastPosition, target.position) > 1) {
					agent.SetDestination (target.position);
					lastPosition = target.position;
					actionState = ActionType.Move;
				}
			}
			time = 0;
		} else
			time += Time.deltaTime;

		this.transform.position = agent.transform.position;

		if (agent.transform.eulerAngles.z < 45 && agent.transform.eulerAngles.z > -45 || (agent.transform.eulerAngles.z > 315 && agent.transform.eulerAngles.z < 405) || (agent.transform.eulerAngles.z < -315 && agent.transform.eulerAngles.z > -405))
			direction = Direction.U;
		else if ((agent.transform.eulerAngles.z >= 45 && agent.transform.eulerAngles.z <= 135) || (agent.transform.eulerAngles.z <= -225 && agent.transform.eulerAngles.z >= -315))
			direction = Direction.L;
		else if ((agent.transform.eulerAngles.z <= -45 && agent.transform.eulerAngles.z >= -135) || (agent.transform.eulerAngles.z >= 225 && agent.transform.eulerAngles.z <= 315))
			direction = Direction.R;
		else
			direction = Direction.D;


		if (actionState == ActionType.Stop)
			anim.Play (idleAnim + "_" + direction.ToString (),0);
		else {
			anim.Play (moveAnim + "_" + direction.ToString (),0);
			anim.speed = agent.speed / agent.maxSpeed;
		}
	}

	public void OnArrived()
	{
		actionState = ActionType.Stop;
	}



}

public enum ActionType {Stop,Move};