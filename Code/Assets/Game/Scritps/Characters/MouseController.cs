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
		state = MouseState.Seek;
		Seek();
	}

	void Seek(){
		anim.Play("Run",0);
		lastPosition = this.transform.position;
		List<Transform> points = InputController.instance.GetRandomPoints (PointType.MouseEat);
		List<Transform> pointRandoms = InputController.instance.GetRandomPoints (PointType.Mouse);
		paths = new Vector3[3];
		paths [0] = originalPosition;
		paths [1] = pointRandoms[Random.Range(0,pointRandoms.Count)].position;
		paths [2] = points[0].position;

		iTween.MoveTo (this.gameObject, iTween.Hash ("path", paths, "speed", speed, "orienttopath", false, "easetype", "linear","oncomplete", "CompleteSeek"));

		time = 0;
		maxTimeSpawn = Random.Range (60, 120);
		this.body.gameObject.SetActive (true);
		col.enabled = true;
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
		List<Transform> points = InputController.instance.GetRandomPoints (PointType.Mouse);
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

			if(state == MouseState.Seek && Vector2.Distance(this.body.transform.position,InputController.instance.character.transform.position) < 3){
				Run();
			}		

		}
		else if(state == MouseState.Eat){
			
			if(ItemController.instance.foodBowl.CanEat())
				ItemController.instance.foodBowl.Eat(0.3f);
			else 
				Run();

			if(Vector2.Distance(this.body.transform.position,InputController.instance.character.transform.position) < 3){
				Run();
			}	
		}
		else {
			if (time > maxTimeSpawn) {
				Spawn ();
			} else
				time += Time.deltaTime;
		}



	}


	void OnMouseUp(){
		anim.Play("Hit",0);
		if(state == MouseState.Eat || state == MouseState.Seek)
			Run();
	}


}

public enum MouseState{Idle,Seek,Eat,Run}
