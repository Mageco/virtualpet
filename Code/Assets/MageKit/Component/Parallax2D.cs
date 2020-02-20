using UnityEngine;
using System.Collections;


public class Parallax2D : MonoBehaviour
{

	/** The intensity of the depth effect. Positive values will make the GameObject appear further away (i.e. in the background), negative values will make it appear closer to the camera (i.e. in the foreground). */
	public float depth;
	/** If True, then the GameObject will scroll in the X-direction */
	public bool xScroll;
	/** If True, then the GameObject will scroll in the Y-direction */
	public bool yScroll;
	/** An offset for the GameObject's initial position along the X-axis */
	public float xOffset;
	/** An offset for the GameObject's initial position along the Y-axis */
	public float yOffset;

	private float xStart;
	private float yStart;
	private float xDesired;
	private float yDesired;
	private Vector2 perspectiveOffset;

	private void Awake ()
	{
		xStart = transform.localPosition.x;
		yStart = transform.localPosition.y;

		xDesired = xStart;
		yDesired = yStart;
	}


	/**
	 * Updates the GameObject's position according to the camera.  This is called every frame by the StateHandler.
	 */
	public void Update()
	{
		perspectiveOffset = new Vector2 (Camera.main.transform.position.x, Camera.main.transform.position.y);

		xDesired = xStart;
		if (xScroll)
		{
			xDesired += perspectiveOffset.x * depth;
			xDesired += xOffset;
		}

		yDesired = yStart;
		if (yScroll)
		{
			yDesired += perspectiveOffset.y * depth;
			yDesired += yOffset;
		}

		if (xScroll && yScroll)
		{
			transform.localPosition = new Vector3 (xDesired, yDesired, transform.localPosition.z);
		}
		else if (xScroll)
		{
			transform.localPosition = new Vector3 (xDesired, transform.localPosition.y, transform.localPosition.z);
		}
		else if (yScroll)
		{
			transform.localPosition = new Vector3 (transform.localPosition.x, yDesired, transform.localPosition.z);
		}
	}

}

