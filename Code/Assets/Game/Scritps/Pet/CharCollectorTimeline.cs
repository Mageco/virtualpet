using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharCollectorTimeline : MonoBehaviour
{
    public int petId = 0;
    bool isClick = false;
    bool isActive = false;
    Vector3 clickPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (IsPointerOverUIObject())
            return;

        if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) > 1f)
        {
            return;
        }

        UIManager.instance.OnPetRequirementPanel(DataHolder.GetPet(petId));
    }

    protected bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
