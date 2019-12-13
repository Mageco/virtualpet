using UnityEngine;

namespace Lean.Touch
{
	public class LeanPinchZoom : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The camera that will be used to calculate the zoom.
		/// None = MainCamera.</summary>
		[Tooltip("The camera that will be used to calculate the zoom.\n\nNone = MainCamera.")]
		public Camera Camera;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		[Tooltip("If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.")]
		public float Dampening = -1.0f;
		public float minSize = 16;
		public float maxSize = 26;

		[HideInInspector]
		[SerializeField]
		private float remainingScale;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Update()
		{
			// Store
			var oldScale = Camera.orthographicSize;

			// Get the fingers we want to use
			var fingers = Use.GetFingers();

			// Calculate pinch scale, and make sure it's valid
			var pinchScale = LeanGesture.GetPinchScale(fingers);

			if (pinchScale != 1.0f)
			{
				// Perform the translation if this is a relative scale
				Camera.orthographicSize = Camera.orthographicSize * pinchScale;
				remainingScale += Camera.orthographicSize - oldScale;
			}

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Dampen remainingDelta
			var newRemainingScale = Mathf.Lerp(remainingScale, 0, factor);

			// Shift this transform by the change in delta
			//transform.localPosition = oldScale + remainingScale - newRemainingScale;
			Camera.orthographicSize = Mathf.Clamp(oldScale - remainingScale + newRemainingScale,minSize,maxSize);

			// Update remainingDelta with the dampened value
			remainingScale = newRemainingScale;
		}
	}
}