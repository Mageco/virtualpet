using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFall : MonoBehaviour
{
    float fallSpeed = 0;
    Vector3 dropPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFall(){
        Vector3 pos1 = this.transform.position;
		pos1.y = pos1.y - 22;
		if (pos1.y < -20)
			pos1.y = -20;
		dropPosition = pos1;
        fallSpeed += 100f * Time.deltaTime;            
        if (fallSpeed > 50)
            fallSpeed = 50;
        Vector3 pos = this.transform.position;
        pos.y -= fallSpeed * Time.deltaTime;
        pos.z = dropPosition.z;
        this.transform.position = pos;
        if (Vector2.Distance (this.transform.position, dropPosition) < fallSpeed * Time.deltaTime * 2) {
            fallSpeed = 0;
            this.transform.rotation = Quaternion.identity;
        }
    }
}
