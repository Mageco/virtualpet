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
	public float scaleFactor = 0.02f;
	Vector3 originalScale;
    public BeeState state = BeeState.Idle;
	CharController target;
	public int minQuestId = -1;

	int hitCount = 0;
    int maxCount = 3;

	Animator anim;
	public GameObject item;

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
		item.SetActive(false);
		item.transform.localPosition = new Vector3(0, 0, -10);
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
        if (GameManager.instance.GetAchivement(18) == 0)
        {
            //UIManager.instance.OnQuestNotificationPopup("Mouse will eat your friends food, tap them to earn coin!");
        }
	}

	void Enter(){
		hitCount = 0;
        maxCount = Random.Range(3, 10);
		state = BeeState.Enter;
		paths = new Vector3[3];
		paths [0] = originalPosition;
		paths [1] = ItemManager.instance.GetRandomPoint(AreaType.Fly);
		paths [2] = ItemManager.instance.GetRandomPoint(AreaType.Fly);

		iTween.MoveTo (this.gameObject, iTween.Hash ("name","Bee_Enter","path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteEnter"));
		maxTimeSpawn = Random.Range (100, 200);
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
			paths [i] = ItemManager.instance.GetRandomPoint(AreaType.Fly);
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
			paths [1] = ItemManager.instance.GetRandomPoint(AreaType.Fly);
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
		paths [1] = ItemManager.instance.GetRandomPoint(AreaType.Fly);
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
            if(target != null)
            {
				if (Vector2.Distance(this.transform.position, target.transform.position) < 5)
				{
					anim.Play("Attack");
					target.OnSupprised();
					target.data.Damage += Random.Range(30,50);
				}
			}
			Patrol();
		}
		else {
			if (time > maxTimeSpawn) {
                if(GameManager.instance.myPlayer.questId > minQuestId)
                    Spawn ();
				time = 0;
			} else
				time += Time.deltaTime;
		}



	}


	void OnMouseUp(){
		MageManager.instance.PlaySound3D("Punch1",false, this.transform.position);
		MageManager.instance.PlaySound3D("collect_item_02",false, this.transform.position);
		anim.Play("Hit",0);
		GameManager.instance.LogAchivement(AchivementType.Tap_Animal,ActionType.None,-1,AnimalType.Bee);
		if(state == BeeState.Fight || state == BeeState.Seek || state == BeeState.Enter || state == BeeState.Patrol) {
			hitCount ++;
			int value = Random.Range(2, 5);
			ItemManager.instance.SpawnCoin(this.transform.position + new Vector3(0, 0, -1), value, this.gameObject);
			GameManager.instance.AddCoin(value, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
			GameManager.instance.LogAchivement(AchivementType.Tap_Animal, ActionType.None, -1, AnimalType.Mouse);
			if (hitCount == 3)
				StartCoroutine(SpawnItem());
		}
		if(hitCount >= 3)
			Run();
	}

	IEnumerator SpawnItem()
	{
		GameObject go = GameObject.Instantiate(item) as GameObject;
		go.SetActive(true);
		go.transform.position = this.body.transform.position;
		go.transform.parent = Camera.main.transform;
		Vector3 target = Camera.main.ScreenToWorldPoint(UIManager.instance.inventoryButton.transform.position) - Camera.main.transform.position;
		target.z = -100;
		while (Vector2.Distance(go.transform.localPosition, target) > 0.5)
		{
			go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, target, 5 * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		GameManager.instance.AddItem(232, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
		Destroy(go);
	}


	void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {

		}
    }
}

public enum BeeState{Idle,Enter,Seek,Patrol,Fight,Run};


