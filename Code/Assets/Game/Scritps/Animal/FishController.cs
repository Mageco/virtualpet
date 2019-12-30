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
        if(state == FishState.Move)
        {
            Move();
        }else if(state == FishState.Cached)
        {
            Catched();
        }else if(state == FishState.Active)
        {
            Active();
        }else if(state == FishState.DeActive){
            DeActive();
        }
    }

   


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Hook")
        {

        }
    }
}

public enum FishState { DeActive, Move, Cached, Active };

