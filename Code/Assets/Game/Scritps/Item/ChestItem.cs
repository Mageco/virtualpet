using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestItem : MonoBehaviour
{
    [HideInInspector]
    public ItemSaveDataType itemSaveDataType = ItemSaveDataType.Chest;
    public int id = 0;
    int value = 0;
    PriceType priceType = PriceType.Coin;

    void Awake()
    {
        int n = Random.Range(0, 3);
        priceType = (PriceType)n;
        if (priceType == PriceType.Coin)
            value = Random.Range(10, 20);
        else if (priceType == PriceType.Happy)
            value = Random.Range(20, 30);
        else if (priceType == PriceType.Diamond)
            value = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Load()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }

    void OnMouseUp()
    {

        if (IsPointerOverUIObject())
            return;


        Pick();
    }

    void Pick()
    {

    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
