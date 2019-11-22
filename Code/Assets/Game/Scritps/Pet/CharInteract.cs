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
    public bool isClick = false;
    //public bool isTouch = false;
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
        //isTouch = true;
        interactType = InteractType.Drag;
        character.OnHold ();
    }

    void OnMouseUp()
    {
        dragOffset = Vector3.zero;
        //isTouch = false;
        if (interactType == InteractType.Drag) {
            interactType = InteractType.Drop;
        }else if(interactType == InteractType.Drop){
            interactType = InteractType.None;
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

    

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Mouse") {
           character.OnMouse();     
           
                  
        } else if (other.tag == "Food") {

        }else if (other.tag == "Water") {
            if(Mathf.Abs(other.transform.position.y - this.transform.position.y) < 1f)
                character.OnFall();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Mouse" && character.actionType == ActionType.Mouse) {
            //character.OffMouse();
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

