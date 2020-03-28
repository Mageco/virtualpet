using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyNav;

public class CharCollider : MonoBehaviour
{
    CharController character;
    public List<CharController> pets = new List<CharController>();

    void Awake()
    {
        character = this.transform.parent.GetComponent<CharController>();
    }
   
    void Update(){
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Mouse") {
           character.OnMouse();     
        }else if (other.tag == "Food") {
            character.OnEat();
        }else if(other.tag == "Drink"){
            character.OnDrink();
        }
        else if (other.tag == "Water") {
            if(character.isMoving){
                character.OnFall();
            }
        }else if(other.tag == "Pet"){
            CharController p = other.transform.parent.GetComponent<CharController>();
            pets.Add(p);
            if(p.actionType != ActionType.Hold && p.actionType != ActionType.Toy && character.enviromentType == EnviromentType.Room && character.isMoving && character.actionType != ActionType.Toy && character.actionType != ActionType.OnCall)
            {
                character.OnStop();
            }
        }else if(other.tag == "Car"){
            if(other.transform.parent.GetComponent<ToyCarItem>().IsSupprised())
                character.OnSupprised();
            else if(character.actionType == ActionType.Patrol)
                character.OnStop();
        }else if(other.tag == "Toy"){
            ToyItem toy = other.transform.parent.GetComponent<ToyItem>();
            character.OnToy(toy);
        }else if(other.tag == "Break"){
            Debug.Log("Break");
            character.OnFear();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Mouse" && character.actionType == ActionType.Mouse) {
            //character.OffMouse();
        }else if (other.tag == "Food") {

        }else if(other.tag == "Pet"){
            CharController pet = other.transform.parent.GetComponent<CharController>();
            if(pets.Contains(pet)){
                pets.Remove(pet);
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
