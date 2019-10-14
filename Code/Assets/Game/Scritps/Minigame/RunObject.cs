using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunObject : MonoBehaviour
{
    public float speed = 1;
    public bool isMove = true;
    public bool isLoop = true;
    public float maxLength = 20;
    float length = 0;
    Vector3 originalPos;
    // Start is called before the first frame update
    void Awake()
    {
        originalPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMove){
            this.transform.position += new Vector3(speed*Time.deltaTime,0,0);
            if(length > maxLength){
                if(isLoop)
                {
                    this.transform.position = originalPos;
                    length = 0;
                }else
                {
                    GameObject.Destroy(this.gameObject);
                }
            }else{
                length += Mathf.Abs(speed * Time.deltaTime);
            }
        }
    }
}
