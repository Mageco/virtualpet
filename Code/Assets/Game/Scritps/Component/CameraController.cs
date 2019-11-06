/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the this.target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

using UnityEngine;
using System.Collections;

[AddComponentMenu("IRobot Kit/Smooth Follow Camera")]
public class CameraController : MonoBehaviour
{
	public float damping;
	public Transform target;
	float width;
	float height;
	float original_orthographicSize = 10;
	float orthographicsize;
	public bool isBound = true;
	public Vector2 offset = Vector2.zero;
	public Vector2 boundX;
	public Vector2 boundY;

	public void SetTarget(GameObject t)
	{
		this.target = t.transform;
	}

	public void SetPosition(Vector3 t)
	{
		this.transform.position = new Vector3(t.x,t.y,this.transform.position.z);
	}

	void Awake()
	{
		
	}


	void Start()
	{
		orthographicsize = Camera.main.orthographicSize;
		if(GameObject.FindObjectOfType<CharController> () != null)
			target = GameObject.FindObjectOfType<CharController> ().transform;
	}

	void LateUpdate()
	{
		this.ExecuteCamera();
	}

	public void SetOriginalOrthographic(float o)
	{
		this.original_orthographicSize = o;
		this.orthographicsize = o;
		if (target == null)
			Camera.main.orthographicSize = o;
	}


	public void SetOrthographic(float o)
	{
		//this.original_orthographicSize = Camera.main.orthographicSize;
		this.orthographicsize = o;
		if (target == null)
			Camera.main.orthographicSize = o;
	}

	public void ResetOrthographic()
	{
		this.orthographicsize = original_orthographicSize;
	}

	void  ExecuteCamera()
	{
		if (target == null)
			return;

		height = Camera.main.orthographicSize;
		width = height * Screen.width / Screen.height;

		Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, orthographicsize, damping * Time.deltaTime);

		if (this.target) {

			if (isBound) {
				float x = Mathf.Lerp (this.transform.position.x, target.position.x + offset.x, damping * Time.deltaTime);
				float y = Mathf.Lerp (this.transform.position.y, target.position.y + offset.y, damping * Time.deltaTime);

				x = Mathf.Clamp (x, boundX.x + width,boundX.y - width);
				y = Mathf.Clamp (y, boundY.x + height, boundY.y - height);

				this.transform.position = new Vector3 (x, y, this.transform.position.z);

			}
		} 
	}
}