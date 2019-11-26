using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    Animator animator;
    float time;
    float maxTimeLight = 10;
    void Awake(){
        animator = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeLight){
            time = 0;
            maxTimeLight = Random.Range(10f,20f);
            animator.Play("Lightning");
        }else{
            time += Time.deltaTime;
        }
    }
}
