using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("Touch Down");
        InputController.instance.character.OnTouch();
    }

    void OnMouseUp(){
        Debug.Log("Touch Up");
        InputController.instance.character.OffTouch();
    }
}
