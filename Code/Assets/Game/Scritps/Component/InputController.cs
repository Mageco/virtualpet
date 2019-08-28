using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class InputController : MonoBehaviour
{
	public static InputController instance;
	public Transform target;
	public LeanScreenDepth ScreenDepth;
	public List<GizmoPoint> callPoints;
	public List<GizmoPoint> sleepPoints;
	public List<GizmoPoint> eatPoints;
	public List<GizmoPoint> favouritesPoints;
	public List<GizmoPoint> mousePoints;
	CharController character;

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
		}
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
		}
		return null;
	}

	public Transform GetRandomPoint(PointType type)
	{
		List<GizmoPoint> points = GetPoints (type);
		int id = Random.Range (0, points.Count);
		return points [id].transform;
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
        
    }

	public void SetTarget(LeanFinger finger)
	{
		var worldPoint = ScreenDepth.Convert (finger.ScreenPosition, gameObject);
		worldPoint.z = target.position.z;
		target.position = worldPoint;

		if ((character.interactType == InteractType.None || character.interactType == InteractType.Caress) && character.enviromentType == EnviromentType.Room) {
			character.target = this.target;
			character.OnFollowTarget ();
		}
	}

	public void OnCall()
	{
		character.OnCall ();
	}
}
