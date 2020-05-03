﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class CharCollider : MonoBehaviour
{
    CharController character;
    public List<CharController> pets = new List<CharController>();
    public List<BaseFloorItem> items = new List<BaseFloorItem>();

    void Awake()
    {
        character = this.transform.parent.GetComponent<CharController>();
    }
   
    void Update(){

        if(character != null && character.shadow != null)
        {
            this.transform.position = character.shadow.transform.position;
        }
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Mouse") {
           character.OnMouse();     
        }
        else if (other.tag == "Water") {
            if(character.isMoving){
                character.OnFall();
            }
        }else if(other.tag == "Pet"){
            CharController p = other.transform.parent.GetComponent<CharController>();
            pets.Add(p);
            if(p.actionType != ActionType.Hold && character.equipment == null && character.isMoving && character.equipment != null && character.actionType != ActionType.OnCall)
            {
                character.OnStop();
            }
        }else if(other.tag == "Car"){
            if(character.actionType == ActionType.Patrol)
                character.OnStop();
        }else if(other.tag == "Break"){
            Debug.Log("Break");
            character.OnSupprised();
        }else if(other.tag == "Equipment")
        {
            BaseFloorItem item = other.transform.parent.GetComponent<BaseFloorItem>();
            if(item != null)
            {
                items.Add(item);
                character.OnToy(item);
            }

        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Mouse" && character.actionType == ActionType.Mouse) {
            //character.OffMouse();
        }else if(other.tag == "Pet"){
            CharController pet = other.transform.parent.GetComponent<CharController>();
            if(pets.Contains(pet)){
                pets.Remove(pet);
            }
        }
        else if (other.tag == "Equipment")
        {
            BaseFloorItem item = other.transform.parent.GetComponent<BaseFloorItem>();
            if (item != null && items.Contains(item))
            {
                items.Remove(item); 
            }
        }
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
