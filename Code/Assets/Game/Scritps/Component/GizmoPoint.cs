using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoPoint : MonoBehaviour
{
	public PointType type = PointType.Favourite;
	void OnDrawGizmos()
	{
		
		if(type == PointType.Call)
			Gizmos.color = Color.red;
		else if(type == PointType.Eat)
			Gizmos.color = Color.yellow;
		else if(type == PointType.Sleep)
			Gizmos.color = Color.blue;
		else if(type == PointType.Favourite)
			Gizmos.color = Color.green;
		else if(type == PointType.Mouse)
			Gizmos.color = Color.cyan;
		else if(type == PointType.Patrol)
			Gizmos.color = Color.magenta;
		else if(type == PointType.Caress)
			Gizmos.color = Color.black;
		else if(type == PointType.Spawn)
			Gizmos.color = Color.black;
        else if (type == PointType.Garden)
            Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.5f);
	}
}

public enum PointType {Sleep,Eat,Drink,Favourite,Call,Mouse,Patrol,Caress,Bath,Table,Window,MouseGate,Door,Safe,Cleaner,Toilet,MouseEat,Spawn,Bee,Garden,Sunny,ChickenDefence,Fishing,Banana};
