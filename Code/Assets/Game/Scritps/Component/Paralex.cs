using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralex : MonoBehaviour
{
    public float speedX = 1;
    public float speedY = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position -= new Vector3(Camera.main.transform.position.x * speedX ,Camera.main.transform.position.y * speedY,0); 
        
    }
}
