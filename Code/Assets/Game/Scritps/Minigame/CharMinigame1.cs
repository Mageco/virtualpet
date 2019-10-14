using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMinigame1 : MonoBehaviour
{
    CharState state;
    Animator anim;
    Rigidbody2D rigid;
    int n = 0;
    float time = 0;

    void Awake(){
        anim = this.GetComponent<Animator>();
        rigid = this.GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnMouseDown(){
        if (state == CharState.Run)
        {
            state = CharState.Touch;
             rigid.AddForce(new Vector2(0, 150));
        }
    }

    public void OnMouseUp(){
        if (state == CharState.Touch)
        {
            Debug.Log("Jump");
            state = CharState.Jump;
            n = Random.Range(0, 100);
            time = 0;
        }       
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
    void FixedUpdate()
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
        }else if(state == CharState.Touch){
            anim.Play("Jump");
            time += Time.deltaTime;
            rigid.AddForce(new Vector2(0, 5));
            anim.speed = 1/(1+time);
            if(time > 1f){
                OnMouseUp();
            }
        }



    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Obstruct"){
            state = CharState.Fail;
            rigid.AddForce(new Vector2(0, 100));
            this.GetComponent<PolygonCollider2D>().enabled = false;
            Minigame1.instance.OnFail();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Ground")
        {
            if(state != CharState.Fail){
                state = CharState.Run;
                 anim.speed = 1;
            }
                
        }
    }
}

public enum CharState{Idle,Touch,Run,Jump,Fail}
