using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructItem : MonoBehaviour
{
    public List<GameObject> itemCollides = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!itemCollides.Contains(other.gameObject))
            itemCollides.Add(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (itemCollides.Contains(other.gameObject))
        {
            itemCollides.Remove(other.gameObject);
        }
    }
}
