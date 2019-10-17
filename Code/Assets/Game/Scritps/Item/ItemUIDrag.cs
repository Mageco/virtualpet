using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;
    ItemDrag item;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Drag Drop
	public void OnBeginDrag(PointerEventData eventData)
	{
        Debug.Log("Begin Drag");
        GameObject go = GameObject.Instantiate(itemPrefab);
        Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        pos.z = 0;
        go.transform.position = pos;
        item = go.GetComponent<ItemDrag>();
        item.StartDrag();
	}

	public void OnDrag(PointerEventData eventData)
	{
		
	}



	public void OnEndDrag(PointerEventData eventData)
	{
        item.EndDrag();
	}
	#endregion
}
