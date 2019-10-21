using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;

public class CharInteract : MonoBehaviour
{
    public InteractType interactType = InteractType.None;
    float doubleClickTime;
    float maxDoubleClickTime = 0.4f;
    bool isClick = false;
    public bool isTouch = false;
    public Direction touchDirection = Direction.D;

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
        isTouch = true;

    }

    void OnMouseUp()
    {
        dragOffset = Vector3.zero;
        isTouch = false;
        if (interactType == InteractType.Drag) {
            interactType = InteractType.Drop;
        } 

        if (isClick) {
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
        }
    }

    public void OnFingerTouchUp(Vector2 delta)
    {
        float angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
        if (isTouch && angle > -45 && angle < 45 && interactType == InteractType.None) {
            touchDirection = Direction.U;
            character.OnHold ();
        }else if(isTouch && (angle > 115 || angle < -115)) {
            touchDirection = Direction.D;
            //interactType = InteractType.SwipeDown;
        }else if(isTouch && (angle > 45 && angle < 115)) {
            touchDirection = Direction.L;
            //interactType = InteractType.SwipeLeft;
        }else if(isTouch && (angle > 115 && angle < 205)) {
            touchDirection = Direction.R;
            //interactType = InteractType.SwipeRight;
        }
    }

    public void OnFingerSwipe(LeanFinger finger)
    {
        if (interactType == InteractType.Drag) {
            float angle = finger.ScreenDelta.x;
            if (angle > 30)
                angle = 30;
            if (angle < -30)
                angle = -30;

            this.transform.rotation = Quaternion.Euler (0, 0, -angle);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Mouse") {
           character.OnMouse(other.transform);            
        } else if (other.tag == "Food") {

        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Mouse" && character.actionType == ActionType.Mouse) {
            character.OffMouse();
        }else if (other.tag == "Food") {

        }
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

public enum InteractType {None,Drag,Drop,Touch,SwipeUp,SwipeDown,SwipeLeft,SwipeRight,DoubleClick};
