﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMinigame1 : MonoBehaviour
{
    CharState state;
    Animator anim;
    Rigidbody2D rigid;
    int n = 0;

    void Awake(){
        anim = this.GetComponent<Animator>();
        rigid = this.GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    
    public void Jump(){
        if (state == CharState.Run)
        {
            rigid.AddForce(new Vector2(0, 200));
            Debug.Log("Jump");
            state = CharState.Jump;
            n = Random.Range(0, 100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(state == CharState.Jump)
        {
           
            //if (n > 50)
                anim.Play("Jump");
            //else
            //    anim.Play("Jump_Rotate");
        }else if(state == CharState.Run)
        {
            anim.Play("Run");
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            state = CharState.Run;
        }else if(collision.gameObject.tag == "Obstruct"){
            state = CharState.Fail;
            Minigame1.instance.OnFail();
        }
    }
}

public enum CharState{Idle,Run,Jump,Fail}
