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
using Lean.Touch;

public class PanCamera : MonoBehaviour
{
    
    [Tooltip("The sensitivity of the movement, use -1 to invert")]
    public float Sensitivity = 1.0f;
    public LeanScreenDepth ScreenDepth;
    public float minx;
    public float maxx;
	public float miny;
	public float maxy;

    Vector3 target;

    private void Start()
    {
		float sizey = Camera.main.orthographicSize;
		float sizex = sizey * Screen.width / Screen.height;
        target = this.transform.position;
		target.x = minx + sizex;
		target.y = 0;
        this.transform.position = target;
    }

    protected virtual void LateUpdate()
    {
		
//		float sizey = Camera.main.orthographicSize;
//		float sizex = sizey * Screen.width / Screen.height;
//        // Get the fingers we want to use
//        var fingers = LeanTouch.GetFingers(true, true);
//
//        // Get the last and current screen point of all fingers
//        var lastScreenPoint = LeanGesture.GetLastScreenCenter(fingers);
//        var screenPoint = LeanGesture.GetScreenCenter(fingers);
//
//        // Get the world delta of them after conversion
//        var worldDelta = ScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, Camera.main, gameObject);
//
//        // Pan the camera based on the world delta
//        target -= worldDelta * Sensitivity;
//        //target.y = this.transform.position.y;
//		this.transform.position = target;// Vector3.Lerp(this.transform.position, target, Time.deltaTime * 2);
//
//		if ((target.x < minx + sizex || target.x > maxx - sizex) || target.y <  miny + sizey || target.y > maxy - sizey)
//        {
//             if(fingers.Count > 0){
//				target.x = Mathf.Clamp(target.x, minx + sizex - 2, maxx - sizex + 2);
//				target.y = Mathf.Clamp(target.y, miny + sizey - 1, maxy - sizey + 1);
//             }else{
//				target.x = Mathf.Clamp(target.x, minx + sizex, maxx - sizex);
//				target.y = Mathf.Clamp(target.y, miny + sizey, maxy - sizey);
//             }
//                
//             this.transform.position = Vector3.Lerp(this.transform.position, target, Time.deltaTime * 0.5f);
//        }


       
    }


}