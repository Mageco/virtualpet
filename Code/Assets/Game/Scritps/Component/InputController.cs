using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
	public static InputController instance;
	public Transform target;
	public LeanScreenDepth ScreenDepth;
	public List<GizmoPoint> callPoints;
	public List<GizmoPoint> sleepPoints;
	public List<GizmoPoint> eatPoints;
	public List<GizmoPoint> drinkPoints;
	public List<GizmoPoint> favouritesPoints;
	public List<GizmoPoint> mousePoints;
	public List<GizmoPoint> patrolPoints;
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
		GizmoPoint[] points = GameObject.FindObjectsOfType <GizmoPoint> ();
		for (int i = 0; i < points.Length; i++) {
			if (points [i].type == PointType.Call)
				callPoints.Add (points [i]);
			else if (points [i].type == PointType.Eat)
				eatPoints.Add (points [i]);
			else if (points [i].type == PointType.Favourite)
				favouritesPoints.Add (points [i]);
			else if (points [i].type == PointType.Sleep)
				sleepPoints.Add (points [i]);
			else if (points [i].type == PointType.Mouse)
				mousePoints.Add (points [i]);
			else if (points [i].type == PointType.Drink)
				drinkPoints.Add (points [i]);
			else if (points [i].type == PointType.Patrol)
				drinkPoints.Add (points [i]);
		}

		Application.targetFrameRate = 50;
	}

	List<GizmoPoint> GetPoints(PointType type)
	{
		if (type == PointType.Call) {
			return callPoints;
		} else if (type == PointType.Eat) {
			return eatPoints;
		} else if (type == PointType.Favourite) {
			return favouritesPoints;
		} else if (type == PointType.Sleep) {
			return sleepPoints;
		}else if (type == PointType.Mouse) {
			return mousePoints;
		}else if (type == PointType.Drink) {
			return drinkPoints;
		}else if (type == PointType.Patrol) {
			return patrolPoints;
		}
		return null;
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

		if ((character.interactType == InteractType.None || character.interactType == InteractType.Caress) && character.enviromentType == EnviromentType.Room) {
			character.target = this.target;
			character.OnListening ();
			Debug.Log ("Double Click");
		}

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

