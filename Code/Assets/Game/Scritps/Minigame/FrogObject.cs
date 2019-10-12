using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogObject : MonoBehaviour
{
    public enum FrogState{wait,idle,active}
    public float waitingTime;
    public float idleTime;
    public float activeTime;
    FrogState state = FrogState.wait;
    float time;
    Animator anim;

    void Awake()
    {
        anim = this.GetComponent<Animator>();
    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(state == FrogState.wait){
            if(time > waitingTime){
                state = FrogState.active;
                time = 0;
            }
            anim.Play("Idle");
        }else if(state == FrogState.active){
            if(time > activeTime){
                state = FrogState.idle;
                time = 0;
            }
            anim.Play("Active");
        }else if(state == FrogState.idle){
            if(time > idleTime){
                state = FrogState.active;
                time = 0;
            }
            anim.Play("Idle");
        }



        time += Time.deltaTime;
    }
}
