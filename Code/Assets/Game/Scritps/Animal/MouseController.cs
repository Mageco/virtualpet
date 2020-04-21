using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseController : MonoBehaviour
{
	Vector3[] paths;
	float time = 0;
	public float maxTimeSpawn = 30;
	public float speed;
	public GameObject body;
	Vector3 originalPosition;
	BoxCollider2D col;
	Vector3 lastPosition;

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	Vector3 originalScale;

    FoodBowlItem foodItem;
    DrinkBowlItem drinkItem;

	public GameObject shitPrefab;

    public MouseState state = MouseState.Idle;

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
		state = MouseState.Seek;
		time = 0;
		Seek();
	}

	void Seek(){
		
		lastPosition = this.transform.position;
        
		if (ItemManager.instance.GetItem(ItemType.Food) == null)
			return;

		Vector3 foodPoint = ItemManager.instance.GetItem(ItemType.Food).transform.position;
		paths = new Vector3[3];
		paths [0] = originalPosition;
		paths [1] = ItemManager.instance.GetRandomPoint(AreaType.Room);
		paths [2] = foodPoint;

		iTween.MoveTo (this.gameObject, iTween.Hash ("name","Mouse","path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteSeek"));
		maxTimeSpawn = Random.Range (100, 200);
		this.body.gameObject.SetActive (true);
		col.enabled = true;
		anim.Play("Run",0);
	}

	void CompleteSeek()
	{
		
		body.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		body.transform.localScale = new Vector3 (body.transform.localScale.x, body.transform.localScale.y, 1);
		anim.Play("Eat",0);
		Debug.Log ("Complete Run");
		state = MouseState.Eat;
	}




	void Run()
	{
		anim.Play("Run",0);
		state = MouseState.Run;
		lastPosition = this.transform.position;
		int max = Random.Range (3, 10);
		List<Vector3> points = ItemManager.instance.GetRandomPoints(AreaType.Room, max);
		paths = new Vector3[max + 1];
		paths [max] = originalPosition;
		for (int i = 0; i < paths.Length - 1; i++) {
			paths [i] = points [i];
		}
		iTween.MoveTo (this.gameObject, iTween.Hash ("path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteRun"));

		time = 0;
		maxTimeSpawn = Random.Range (60, 120);
		this.body.gameObject.SetActive (true);
		col.enabled = true;
	}

	void CompleteRun()
	{
		Debug.Log ("Complete Run");
		state = MouseState.Idle;
		anim.Play("Run", 0);
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
		if (state == MouseState.Seek || state == MouseState.Run) {
			Vector3 pos = this.transform.position;
			pos.z = this.transform.position.y * 10;
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
		else if(state == MouseState.Eat){
			
			if(GetFoodItem().CanEat() && Vector2.Distance(this.transform.position,GetFoodItem().transform.position) < 1)
                GetFoodItem().Eat(0.3f);
			else 
				Run();
		}
		else {
			if (time > maxTimeSpawn) {
                if(GameManager.instance.myPlayer.questId > 7)
                    Spawn ();
				time = 0;
			} else
				time += Time.deltaTime;
		}



	}

    public void OnActive()
    {
		OnMouseUp();
    }


	void OnMouseUp(){
		

        if (state == MouseState.Eat || state == MouseState.Seek || state == MouseState.Run)
        {
            MageManager.instance.PlaySound("Punch1", false);
            MageManager.instance.PlaySound("collect_item_02", false);
            anim.Play("Hit", 0);
			int value = Random.Range(1, 3);
			ItemManager.instance.SpawnCoin(this.transform.position + new Vector3(0,2,-1), value,this.gameObject);
            GameManager.instance.AddCoin(value, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
            GameManager.instance.LogAchivement(AchivementType.Tap_Animal, ActionType.None, -1, AnimalType.Mouse);
            if (state == MouseState.Eat || state == MouseState.Seek)
            {
                Run();
            }
        }
	}


    public FoodBowlItem GetFoodItem()
    {
        if (foodItem == null)
            foodItem = FindObjectOfType<FoodBowlItem>();
        return foodItem;
    }

    public DrinkBowlItem GetDrinkItem()
    {
        if (drinkItem == null)
            drinkItem = FindObjectOfType<DrinkBowlItem>();
        return drinkItem;
    }

	
	
    protected void SpawnShit(Vector3 pos)
    {
        GameObject go = Instantiate(shitPrefab, pos, Quaternion.identity);
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
		
			if(state == MouseState.Eat || state == MouseState.Seek)
				Run();
		}
    }
}

public enum MouseState{Idle,Seek,Eat,Run};
