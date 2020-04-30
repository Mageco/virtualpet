using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int realID = 0;
    public int itemID = 0;
	public ItemType itemType = ItemType.Coin;
    ItemCollider itemCollider;

    private void Start()
    {
        itemCollider = this.GetComponentInChildren<ItemCollider>();
    }
}

