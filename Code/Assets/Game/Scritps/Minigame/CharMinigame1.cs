using System.Collections;
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
        }else if(state == CharState.Fail)
        {
            anim.Play("Fail");
        }

    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Obstruct"){
            state = CharState.Fail;
            rigid.AddForce(new Vector2(0, 100));
            this.GetComponent<CircleCollider2D>().enabled = false;
            Minigame1.instance.OnFail();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Ground")
        {
            if(state != CharState.Fail)
                state = CharState.Run;
        }
    }
}

public enum CharState{Idle,Run,Jump,Fail}
