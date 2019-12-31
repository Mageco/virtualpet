using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public float speed = 10f;
    public Direction direction = Direction.L;
    Vector3 originalPosition;
    BoxCollider2D col; 
    public FishState state = FishState.Move;
    Animator anim;

    Vector3[] paths;

    void Awake()
    {
        col = this.transform.GetComponentInChildren<BoxCollider2D>();
        originalPosition = this.transform.position;
        col.enabled = true;
        anim = this.GetComponent<Animator>();
    }

    void Start()
    {
        //Spawn ();
    }

    public void Spawn()
    {
        state = FishState.Move;
        Move();
    }

    void Move()
    {
        col.enabled = true;
        anim.Play("Run", 0);
    }

    void CompleteMove()
    {

    }

    void Catched()
    {

    }

    void Active()
    {

    }

    void DeActive()
    {

    }

    void Update()
    {

    }

   


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hook")
        {

        }
    }
}

public enum FishState { Idle, Move, Cached, Active };

