using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BeeController : MonoBehaviour
{
	Vector3[] paths;
	float time = 0;
	public float maxTimeSpawn = 30;
	float speed = 30;
	public float initSpeed = 30;
	public GameObject body;
	Vector3 originalPosition;
	BoxCollider2D col;
	Vector3 lastPosition;

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	Vector3 originalScale;
    public BeeState state = BeeState.Idle;
	CharController target;

	int hitCount = 0;
    int maxCount = 3;

	Animator anim;

	void Awake()
	{
		col = this.transform.GetComponentInChildren<BoxCollider2D>();
		originalPosition = this.transform.position;
		lastPosition = this.transform.position;
		this.body.gameObject.SetActive (false);
		col.enabled = false;
		originalScale = this.transform.localScale;
		anim = this.body.GetComponent<Animator>();
		speed = initSpeed;
	}

	void Start()
	{
		//Spawn ();
	}

	void OnDrawGizmos(){
		if(paths != null && paths.Length > 0)
			iTween.DrawPath(paths);
	}

	public void Spawn()
	{
		time = 0;
		Enter();
	}

	void Enter(){
		hitCount = 0;
        maxCount = Random.Range(3, 10);
		state = BeeState.Enter;
		paths = new Vector3[3];
		paths [0] = originalPosition;
		paths [1] = ItemManager.instance.GetRandomPoint(PointType.Bee).position;
		paths [2] = ItemManager.instance.GetRandomPoint(PointType.Bee).position;

		iTween.MoveTo (this.gameObject, iTween.Hash ("name","Bee_Enter","path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteEnter"));
		maxTimeSpawn = Random.Range (200, 600);
		this.body.gameObject.SetActive (true);
		col.enabled = true;
		anim.Play("Fly",0);
	}

	void CompleteEnter(){
		Patrol();
	}

	void Patrol(){
		speed = initSpeed;
		state = BeeState.Patrol;
		int n = Random.Range(3,4);
		paths = new Vector3[n];
		for(int i=0;i<n;i++){
			paths [i] = ItemManager.instance.GetRandomPoint(PointType.Bee).position;
		}

		iTween.MoveTo (this.gameObject, iTween.Hash ("name","Bee_Patrol","path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompletePatrol"));
	}

	void CompletePatrol(){
		Debug.Log ("Complete Patrol");
		Seek();
	}

	void Seek(){
		state = BeeState.Seek;
		speed = initSpeed * 5;
		target = GameManager.instance.GetRandomPetObject();
		if(target != null){
			paths = new Vector3[3];
			paths [0] = this.transform.position;
			paths [1] = ItemManager.instance.GetRandomPoint(PointType.Bee).position;
			paths [2] = target.transform.position;

			iTween.MoveTo (this.gameObject, iTween.Hash ("name","Bee_Seek","path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteSeek"));
			maxTimeSpawn = Random.Range (200, 600);
			this.body.gameObject.SetActive (true);
			col.enabled = true;
		}else{
			Run();
		}
	}

	void CompleteSeek()
	{
		body.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		body.transform.localScale = new Vector3 (body.transform.localScale.x, body.transform.localScale.y, 1);
		Debug.Log ("Complete Seek");
		Fight();
	}


	void Fight(){
		state = BeeState.Fight;
	}

	void Run()
	{
		speed = initSpeed * 10;
		state = BeeState.Run;
		paths = new Vector3[3];
		paths [0] = this.transform.position;
		paths [1] = ItemManager.instance.GetRandomPoint(PointType.Bee).position;
		paths [2] = originalPosition;
		iTween.MoveTo (this.gameObject, iTween.Hash ("path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteRun"));
	}

	void CompleteRun()
	{
		Debug.Log ("Complete Run");
		state = BeeState.Idle;
		this.body.gameObject.SetActive (false);
		col.enabled = false;
		this.transform.position = originalPosition;
	}

	void Update()
	{
		lastPosition = this.transform.position;
	}

	void LateUpdate()
	{
		if (state == BeeState.Seek || state == BeeState.Run || state == BeeState.Enter || state == BeeState.Patrol) {
			Vector3 pos = this.transform.position;
			pos.z = -100;
			this.transform.position = pos;
			if (pos.x > lastPosition.x) {
				body.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				body.transform.localScale = new Vector3 (body.transform.localScale.x, body.transform.localScale.y, -1);
			} else {
				body.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
				body.transform.localScale = new Vector3 (body.transform.localScale.x, body.transform.localScale.y, 1);
			}

			float offset = initZ;

			if (this.transform.position.y < offset)
				this.transform.localScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
			else
				this.transform.localScale = originalScale;	

		}
		else if(state == BeeState.Fight){
			CharController pet = GameManager.instance.GetRandomPetObject();
			if(Vector2.Distance(this.transform.position,pet.transform.position) < 5){
				anim.Play("Attack");
				pet.OnFear();
				pet.data.Health -= 5;
			}

			Patrol();
		}
		else {
			if (time > maxTimeSpawn) {
                if(GameManager.instance.myPlayer.questId > 16)
                    Spawn ();
				time = 0;
			} else
				time += Time.deltaTime;
		}



	}


	void OnMouseUp(){
		MageManager.instance.PlaySoundName("Punch1",false);
		MageManager.instance.PlaySoundName("collect_item_02",false);
		anim.Play("Hit",0);
		GameManager.instance.AddCoin(3);
		GameManager.instance.LogAchivement(AchivementType.Tap_Animal,ActionType.None,-1,AnimalType.Bee);
		if(state == BeeState.Fight || state == BeeState.Seek || state == BeeState.Enter || state == BeeState.Patrol) {
			hitCount ++;
		}
		if(hitCount >= 3)
			Run();
	}


    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {

		}
    }
}

public enum BeeState{Idle,Enter,Seek,Patrol,Fight,Run};


