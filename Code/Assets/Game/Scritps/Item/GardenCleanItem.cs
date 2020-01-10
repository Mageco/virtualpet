using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenCleanItem : MonoBehaviour
{
	public float clean = 1f;
	protected ItemDirty dirtyItem;




	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		if (dirtyItem != null)
		{
			dirtyItem.OnClean(clean * Time.deltaTime);
		}
	}



	void OnTriggerStay2D(Collider2D other)
	{
		if (other.GetComponent<ItemDirty>() != null)
		{
			dirtyItem = other.GetComponent<ItemDirty>();
		}
	}


	void OnTriggerExit2D(Collider2D other)
	{
		if (other.GetComponent<ItemDirty>() == dirtyItem)
		{
			dirtyItem = null;
		}
	}
}
