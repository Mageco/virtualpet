using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedItem : MonoBehaviour
{

    Animator animator;
    bool isHighlight = false;
    // Start is called before the first frame update
    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isHighlight){
            animator.Play("Highlight",0);   
        }else
            animator.Play("Idle",0);   
    }

    void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
            isHighlight = true;  
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
            isHighlight = false;  
		}
	}
}
