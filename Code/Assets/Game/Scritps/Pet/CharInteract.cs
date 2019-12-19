using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CharInteract : MonoBehaviour
{
    public InteractType interactType = InteractType.None;

    public Vector3 dragOffset;
    CharController character;


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
        
    }

    #region Interact
    void OnMouseDown()
    {
        if (IsPointerOverUIObject ()) {
            return;
        }
        dragOffset = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position ;
        //isTouch = true;
        if(interactType == InteractType.None){
            character.OnHold ();
        }

    }

    void OnMouseUp()
    {
        dragOffset = Vector3.zero;
        //isTouch = false;
        if (interactType == InteractType.Drag) {
            interactType = InteractType.Drop;
        }else{
            interactType = InteractType.None;
            character.isAbort = true;
            character.actionType = ActionType.None;
        }

 /*        if (isClick) {
            if (doubleClickTime > maxDoubleClickTime) {
                doubleClickTime = 0;
            } else {
                //character.OnListening ();
                doubleClickTime = 0;
                isClick = false;
                return;
            }
        } else {
            doubleClickTime = 0;
            isClick = true;
        } */
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

