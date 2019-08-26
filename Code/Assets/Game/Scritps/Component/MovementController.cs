using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementController : MonoBehaviour {

	public float maxSpeed;
	public float currentSpeed;
	Vector3 speed = Vector3.zero;
	public float accelerator;
	Vector2[] movePoint;
	Direction currentDirection = Direction.D;
	bool isMove = false;

	public void MoveTo(Vector2 target)
	{

	}

	void CalculatePoints()
	{

	}

	void Update()
	{

	}

}
