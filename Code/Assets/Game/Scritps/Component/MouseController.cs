using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseController : MonoBehaviour
{
	Vector3[] paths;
	float time = 0;
	float maxTimeSpawn = 3;
	public float speed;
	bool isRun = false;
	public GameObject body;
	Vector3 originalPosition;

	void Awake()
	{
		originalPosition = this.transform.position;
		this.body.gameObject.SetActive (false);
		this.GetComponent <CircleCollider2D> ().enabled = false;
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
		List<Transform> points = InputController.instance.GetRandomPoints (PointType.Mouse);
		int max = Random.Range (3, points.Count);
		paths = new Vector3[max + 2];
		paths [0] = originalPosition;
		paths [max + 1] = originalPosition;
		for (int i = 1; i < paths.Length - 1; i++) {
			paths [i] = points [i-1].position;
		}
		iTween.MoveTo (this.gameObject, iTween.Hash ("path", paths, "speed", speed, "orienttopath", false, "easetype", "easeInOutSine", "oncomplete", "Complete"));

		time = 0;
		maxTimeSpawn = Random.Range (60, 120);
		isRun = true;
		this.body.gameObject.SetActive (true);
		this.GetComponent <CircleCollider2D> ().enabled = true;
	}

	void Update()
	{
		if (isRun) {
			Vector3 pos = this.transform.position;
			pos.z = this.transform.position.y;
			this.transform.position = pos;
		}
		else {
			if (time > maxTimeSpawn) {
				Spawn ();
			} else
				time += Time.deltaTime;
		}

	}

	void Complete()
	{
		Debug.Log ("Complete Run");
		isRun = false;
		this.body.gameObject.SetActive (false);
		this.GetComponent <CircleCollider2D> ().enabled = false;
		this.transform.position = originalPosition;
	}

}
