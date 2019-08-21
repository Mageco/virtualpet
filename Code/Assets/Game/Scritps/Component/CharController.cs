using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharController : MonoBehaviour {

	Animator anim;
	bool isMove = false;
	public bool isRest = true;
	Vector3 originalPos;
	Vector3 targetPos;
	float timeAction;
	public float maxTimeAction;

	[HideInInspector]
	public Direction direction;
	Direction lastDirection;
	PolyNavAgent agent;
	[HideInInspector]
	string moveAnim = "run";
	string idleAnim = "idle";
	Transform[] targets;
	public GameObject targetParent;

	void Awake()
	{
		anim = this.GetComponent<Animator> ();
		this.originalPos = this.transform.position;
		targetPos = originalPos;
		agent = this.transform.GetComponent<PolyNavAgent> ();
		targets = targetParent.GetComponentsInChildren <Transform> ();
	}
	// Use this for initialization
	void Start () {
		isRest = true;
		isMove = true;
		agent.SetDestination (targetPos);
	}

	// Update is called once per frame
	void Update () {

		if (timeAction >= maxTimeAction) {
			Think ();
		}else
			timeAction += Time.deltaTime;


		if (isMove) {

			if (agent.rotation < 5 && agent.rotation > -5)
				direction = Direction.U;
			else if (agent.rotation >= 5 && agent.rotation <= 135)
				direction = Direction.L;
			else if (agent.rotation <= -5 && agent.rotation >= -135)
				direction = Direction.R;
			else
				direction = Direction.D;

			if (lastDirection != direction) {
				lastDirection = direction;
			}

			if (direction == Direction.D || direction == Direction.U)
				agent.speed = agent.maxSpeed * 0.5f;
			else
				agent.speed = agent.maxSpeed;

			anim.Play (moveAnim + "_" + direction.ToString(), 0);

		} else {
			if(isRest)
				anim.Play ("Lay_D", 0);
			else
				anim.Play (idleAnim + "_" + direction.ToString (),0);

		}
	}


	void Think()
	{
		timeAction = 0;
		int n = Random.Range (0, 10);
		if (n < 2) {
			isRest = true;
			maxTimeAction = Random.Range (3, 10);
		}
		else {
			isRest = false;
			if (n > 7) {
				maxTimeAction = 100;
				RandomMove ();
			} else if (n > 4) {
				int d = Random.Range (0, 4);
				direction = (Direction)d;
				maxTimeAction = Random.Range (1, 2);
			} else {
				maxTimeAction = Random.Range (1, 3);
			}
		}
	}

	void RandomMove()
	{
		int id = Random.Range (0, targets.Length);
		agent.SetDestination (targets[id].position);
		isMove = true;
	}

	public void OnArrived()
	{
		isMove = false;
		Think ();
	}


	public bool IsMove()
	{
		return isMove;
	}

	public void MoveTo(Vector3 target)
	{
		targetPos = target;
		agent.SetDestination (targetPos);
	}

}
