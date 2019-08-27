using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This base class can be used to associate extra data with the specified LeanFinger instance.</summary>
	public abstract class LeanFingerData
	{
		// The finger associated with this link
		public LeanFinger Finger;

		public static void Remove<T>(List<T> fingerDatas, LeanFinger finger, Stack<T> pool = null)
			where T : LeanFingerData
		{
			if (fingerDatas != null)
			{
				for (var i = fingerDatas.Count - 1; i >= 0; i--)
				{
					var fingerData = fingerDatas[i];

					if (fingerData.Finger == finger)
					{
						fingerDatas.RemoveAt(i);

						if (pool != null)
						{
							pool.Push(fingerData);
						}
					}
				}
			}
		}

		public static T Find<T>(List<T> fingerDatas, LeanFinger finger)
			where T : LeanFingerData
		{
			if (fingerDatas != null)
			{
				// Find existing link?
				for (var i = fingerDatas.Count - 1; i >= 0; i--)
				{
					var fingerData = fingerDatas[i];

					if (fingerData.Finger == finger)
					{
						return fingerData;
					}
				}
			}

			return null;
		}

		public static T FindOrCreate<T>(ref List<T> fingerDatas, LeanFinger finger)
			where T : LeanFingerData, new()
		{
			if (fingerDatas == null)
			{
				fingerDatas = new List<T>();
			}

			// Find existing link?
			for (var i = fingerDatas.Count - 1; i >= 0; i--)
			{
				var fingerData = fingerDatas[i];

				if (fingerData.Finger == finger)
				{
					return fingerData;
				}
			}

			// Make new link?
			var newFingerData = new T();

			newFingerData.Finger = finger;

			fingerDatas.Add(newFingerData);

			return newFingerData;
		}
	}
}