using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidItem : MonoBehaviour
{
    bool isOpen = false;
    Animator animator;
    public HealthItem[] healthItems;

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
        Debug.Log("Mouse Up");
        if(isOpen){
            Close();
        }else
        {
            Open();
        }
    }

    void Open(){
        Debug.Log("Open");
        isOpen = true;
        for(int i=0;i<healthItems.Length;i++){
            healthItems[i].Open();
        }
        animator.Play("Open",0);
    }

    void Close(){
        isOpen = false;
        for(int i=0;i<healthItems.Length;i++){
            healthItems[i].Close();
        }
        animator.Play("Idle",0);
    }
}
