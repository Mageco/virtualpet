using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunObject : MonoBehaviour
{
    public float speed = 1;
    public bool isMove = true;
    public float duration = 3;
    float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isMove){
            this.transform.position += new Vector3(speed*Time.deltaTime,0,0);
            if(time > duration){
                GameObject.Destroy(this.gameObject);
            }else{
                time += Time.deltaTime;
            }

        }
    }
}
