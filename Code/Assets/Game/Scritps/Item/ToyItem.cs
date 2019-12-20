using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyItem : MonoBehaviour
{
    public ToyType toyType = ToyType.Ball;
    protected Animator animator;
    public Transform anchorPoint;

    protected ItemObject item;

    protected virtual void Awake(){

    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = this.GetComponent<Animator>();
        item = this.transform.parent.GetComponent<ItemObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnActive(){
        
    }
}
