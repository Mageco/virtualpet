using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
	public static InputController instance;
	public Transform target;
	[HideInInspector]
	public CharController character;
	public CameraController cameraController;


	float time;
	float maxDoubleClickTime = 0.4f;
	bool isClick = false;

	void Awake()
	{
		if (instance == null)
			instance = this;

		character = GameObject.FindObjectOfType<CharController> ();
		
	}

	List<GizmoPoint> GetPoints(PointType type)
	{
		List<GizmoPoint> temp = new List<GizmoPoint>();
		GizmoPoint[] points = GameObject.FindObjectsOfType <GizmoPoint> ();
		for(int i=0;i<points.Length;i++)
		{
			temp.Add(points[i]);
		}
		return temp;
	}

	public Transform GetRandomPoint(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		int id = Random.Range (0, points.Count);
		return points [id].transform;
	}

	public void SetTarget(PointType type)
	{
		Debug.Log (type);
		target.position = this.GetRandomPoint (type).position;
		character.target = this.target;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
	void Update()
	{
		if (isClick)
			time += Time.deltaTime;
	}


	public void OnCall()
	{
		character.OnCall ();
	}

	void OnMouseDown()
	{
		if (IsPointerOverUIObject ()) {
			return;
		}

	}

	void OnMouseUp()
	{
		if (isClick) {
			if (time > maxDoubleClickTime) {
				time = 0;
			} else {
				OnDoubleClick ();
				time = 0;
				isClick = false;
				return;
			}
		} else {
			time = 0;
			isClick = true;
		}
	}

	void OnDoubleClick()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos.z = 0;
		target.position = pos;
	}

	public void ResetCameraTarget()
	{
		cameraController.SetTarget (this.character.gameObject);
	}

	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

