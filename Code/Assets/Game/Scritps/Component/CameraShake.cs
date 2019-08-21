using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	//Camera shake effect, calling when player hits the bombs;

	public float shakeForce = 0.2F;	//Shake Power;
	
	private Vector3 originPosition;
	private Quaternion originRotation;
	private float shake_decay, shake_intensity;

	void Start()
	{
		originPosition = transform.position;
		originRotation = transform.rotation;
		shake_decay = 0.007F;
	}
	
	void FixedUpdate () 
	{
		if(shake_intensity > 0){
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			transform.rotation = new Quaternion(
			originRotation.x + Random.Range(-shake_intensity,shake_intensity)*0.1F,
			originRotation.y + Random.Range(-shake_intensity,shake_intensity)*0.1F,
			originRotation.z + Random.Range(-shake_intensity,shake_intensity)*0.1F,
			originRotation.w + Random.Range(-shake_intensity,shake_intensity)*0.1F);
			shake_intensity -= shake_decay;
		}
	}

	public void Shake()
	{
		shake_intensity = shakeForce;		//shake time;
	}
}
