using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{

    public static AnimalController instance;

    [HideInInspector]
    public MouseController mouse;

    void Awake(){
        if(instance == null)
            instance = this;
        mouse = GameObject.FindObjectOfType<MouseController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
