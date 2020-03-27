using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseItem : MonoBehaviour
{
	[HideInInspector]
	public ItemObject item;
    public Vector2 roomBoundX = new Vector2(-50,70);
    public Vector2 roomBoundY = new Vector2(-26,-4);
    public Vector2 gardenBoundX = new Vector2(-265,70);
    public Vector2 gardenBoundY = new Vector2(-26, -4);
    public Vector2 cameraBoundX = new Vector2(-50,70);
    public Vector2 cameraBoundY = new Vector2(-26,40);
    public int petNumber = 2;

	// Start is called before the first frame update
	void Start()
    {
        item = this.transform.parent.GetComponent<ItemObject>();
        petNumber = (int)DataHolder.GetItem(item.itemID).value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
