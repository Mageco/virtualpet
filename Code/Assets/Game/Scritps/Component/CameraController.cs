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
		//if(target.GetComponent<CharController>() != null){
		//	orthographicsize = target.GetComponent<CharController>().cameraSize;
		//}
	}

	public void SetPosition(Vector3 t)
	{
		this.transform.position = new Vector3(t.x,t.y,this.transform.position.z);
	}

	public void FindTarget(){
		if(GameObject.FindObjectOfType<CharController>() != null){
			target = GameObject.FindObjectOfType<CharController>().transform;
		}			
	}

	void Awake()
	{
		
	}


	void Start()
	{
		orthographicsize = Camera.main.orthographicSize;
		Vector3 pos = this.transform.position;


		if(GameObject.FindObjectOfType<CharController> () != null){
			pos.x = GameObject.FindObjectOfType<CharController> ().transform.position.x;
			pos.y = GameObject.FindObjectOfType<CharController> ().transform.position.y;
		}
		this.transform.position = pos;
	}

	void Update(){
		
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

		height = Camera.main.orthographicSize;
		width = height * Screen.width / Screen.height;

		//Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, orthographicsize, damping * Time.deltaTime);


			if (this.target) {
				float x = this.transform.position.x;
				float y = this.transform.position.y;
				if(this.target.position.x < this.transform.position.x - width * 0.7f || this.target.position.x > this.transform.position.x + width * 0.7f)
					x = Mathf.Lerp (this.transform.position.x, target.position.x + offset.x, damping * Time.deltaTime);

				if (this.target.position.y < this.transform.position.y - height * 0.7f || this.target.position.y > this.transform.position.y + height * 0.7f) 
					y = Mathf.Lerp (this.transform.position.y, target.position.y + offset.y, damping * Time.deltaTime);

				x = Mathf.Clamp (x, boundX.x + width,boundX.y - width);
				y = Mathf.Clamp (y, boundY.x + height, boundY.y - height);

				this.transform.position = new Vector3 (x, y, this.transform.position.z);

				
			}
		
			else {
				if (isBound) {
					float x = this.transform.position.x;
					float y = this.transform.position.y;

					x = Mathf.Clamp (x, boundX.x + width,boundX.y - width);
					y = Mathf.Clamp (y, boundY.x + height, boundY.y - height);

					this.transform.position = new Vector3 (x, y, this.transform.position.z);

				}
			}
	}
}