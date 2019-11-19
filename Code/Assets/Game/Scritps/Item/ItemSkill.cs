using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkill : MonoBehaviour
{
    public SkillType skillType;
    bool isEnter = false;
    bool isActive = false;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnActive(){
        isActive = true;
        animator.Play("Active",0);
    }

    public void DeActive(){
        isActive = false;
        animator.Play("Idle",0);
    }


    void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {
            //Debug.Log("Enter");
            isEnter = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            isEnter = false;
		}
	}
}
