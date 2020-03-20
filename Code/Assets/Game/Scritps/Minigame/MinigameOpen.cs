using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinigameOpen : MonoBehaviour
{
    public int gameId = 0;
    float time = 0;
    bool isMouseDown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown)
            time += Time.deltaTime;
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.isGuest)
            return;

        isMouseDown = true;
    }

    void OnMouseUp()
	{
        if (GameManager.instance.isGuest)
            return;

        if (IsPointerOverUIObject())
            return;
        if(time < 0.3f)
            UIManager.instance.OnEventPanel(gameId);
        time = 0;
        isMouseDown = false;
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
