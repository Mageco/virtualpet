using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CharInteract : MonoBehaviour
{
    public InteractType interactType = InteractType.None;

    public Vector3 dragOffset;
    CharController character;
    bool isMouseDown = false;
    bool isDrag = false;
    Vector3 holdPosition = Vector3.zero;
    float touchTime = 0;
    float maxClickTime = 0.3f;

    void Awake()
    {
        character = this.GetComponent<CharController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseDown)
        {
            if(!isDrag && Vector2.Distance(holdPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition)) > 0.1f)
            {
                OnDrag();
            }
            touchTime += Time.deltaTime;
        }
    }

    #region Interact
    void OnMouseDown()
    {
        if (IsPointerOverUIObject ()) {
            return;
        }
        isMouseDown = true;
        holdPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnDrag()
    {
        isDrag = true;
        dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        if (interactType == InteractType.None)
        {
            character.OnHold();
        }
    }

    public void OnHold()
    {
        character.OnHold();
    }

    void OnMouseUp()
    {
        
        dragOffset = Vector3.zero;
        //isTouch = false;
        if (interactType == InteractType.Drag) {
            interactType = InteractType.Drop;
        }else if(interactType == InteractType.Drop){
            interactType = InteractType.None;
            character.isAbort = true;
            character.actionType = ActionType.None;
        }
        else if(touchTime < maxClickTime)
        {
            OnClick();
        }

        touchTime = 0;
        isDrag = false;
        isMouseDown = false;
    }

    void OnClick()
    {
        MageManager.instance.PlaySoundName("Button", false);
        character.OnCall();
    }


    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }



    #endregion
}

