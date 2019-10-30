using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidItem : MonoBehaviour
{
    bool isOpen = false;
    Animator animator;

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
        
    }

    void OnMouseUp()
    {
        if(isOpen){
            Close();
        }else
        {
            Open();
        }
    }

    void Open(){
        isOpen = true;
        animator.Play("Open",0);
    }

    void Close(){
        isOpen = false;
        animator.Play("Close",0);
    }
}
