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
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}

public enum PointType {Sleep,Eat,Favourite,Call,Mouse};
