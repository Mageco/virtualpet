using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseController : MonoBehaviour
{
	Vector3[] paths;
	float time = 0;
	public float maxTimeSpawn = 3;
	public float speed;
	public GameObject body;
	Vector3 originalPosition;
	public CircleCollider2D col;
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
		if(GameManager.instance.GetActivePet().level > 5){
			state = MouseState.Seek;
			time = 0;
			Seek();
		}
	}

	void Seek(){
		
		lastPosition = this.transform.position;

		List<Transform> points = GetRandomPoints (PointType.MouseEat);
		List<Transform> pointRandoms = GetRandomPoints (PointType.Mouse);

		if(points == null || pointRandoms == null || points.Count == 0 || pointRandoms.Count == 0){
			return;
		}
		paths = new Vector3[3];
		paths [0] = originalPosition;
		paths [1] = pointRandoms[Random.Range(0,pointRandoms.Count)].position;
		paths [2] = points[0].position;

		iTween.MoveTo (this.gameObject, iTween.Hash ("path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteSeek"));
		maxTimeSpawn = Random.Range (60, 120);
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
		List<Transform> points = GetRandomPoints (PointType.Mouse);
		int max = Random.Range (3, points.Count);
		paths = new Vector3[max + 1];
		paths [max] = originalPosition;
		for (int i = 0; i < paths.Length - 1; i++) {
			paths [i] = points [i].position;
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
			pos.z = this.transform.position.y;
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

			if(state == MouseState.Seek && Vector2.Distance(this.body.transform.position,GameManager.instance.GetPetObject(0).transform.position) < 3){
				Run();
			}		

		}
		else if(state == MouseState.Eat){
			
			if(GetFoodItem().CanEat() && Vector2.Distance(this.transform.position,GetRandomPoint (PointType.MouseEat).position) < 1)
                GetFoodItem().Eat(0.3f);
			else 
				Run();

			if(Vector2.Distance(this.body.transform.position,GameManager.instance.GetPetObject(0).transform.position) < 3){
				Run();
			}	
		}
		else {
			if (time > maxTimeSpawn) {
				Spawn ();
				time = 0;
			} else
				time += Time.deltaTime;
		}



	}


	void OnMouseUp(){
		anim.Play("Hit",0);
		GameManager.instance.AddCoin(5);
		GameManager.instance.LogAchivement(AchivementType.Tap_Animal,ActionType.None,-1,AnimalType.Mouse);
		if(state == MouseState.Eat || state == MouseState.Seek)
			Run();
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

	#region  getpoint
    List<GizmoPoint> GetPoints(PointType type)
	{
		List<GizmoPoint> temp = new List<GizmoPoint>();
		GizmoPoint[] points = GameObject.FindObjectsOfType <GizmoPoint> ();
		for(int i=0;i<points.Length;i++)
		{
			if(points[i].type == type)
				temp.Add(points[i]);
		}
		return temp;
	}

	public Transform GetRandomPoint(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		if(points != null){
			int id = Random.Range (0, points.Count);
			return points [id].transform;
		}else
			return null;

	}

	public List<Transform> GetRandomPoints(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		List<Transform> randomPoints = new List<Transform> ();
		for (int i = 0; i < points.Count; i++) {
			randomPoints.Add (points [i].transform);
		}

		for (int i = 0; i < randomPoints.Count; i++) {
			if (i < randomPoints.Count - 1) {
				int j = Random.Range (i, randomPoints.Count);
				Transform temp = randomPoints [i];
				randomPoints [i] = randomPoints [j];
				randomPoints [j] = temp;
			}
		}
		return randomPoints;
	}

	
    protected void SpawnShit(Vector3 pos)
    {
        GameObject go = Instantiate(shitPrefab, pos, Quaternion.identity);
    }

    #endregion


}

public enum MouseState{Idle,Seek,Eat,Run}
