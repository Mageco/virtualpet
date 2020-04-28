using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarFruitGame : MonoBehaviour
{
    public float originalSpeed = 10;
    public float speed = 0;
    float time = 0;
    AnimalState state = AnimalState.Idle;
    public Animator head;
    public Animator body;
    // Start is called before the first frame update
    void Start()
    {
        speed = originalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnIncreaseSpeed()
    {
        speed = originalSpeed * 1.5f;
    }

    public void ResetSpeed()
    {
        speed = originalSpeed;
    }

    public void OnLose()
    {

    }

    public void OnRunLeft()
    {

    }

    public void OnRunRight()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<FruitFallItem>() != null)
        {
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }
}

