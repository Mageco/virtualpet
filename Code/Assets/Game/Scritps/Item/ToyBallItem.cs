using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Lean.Touch;

public class ToyBallItem : ToyItem
{
    CircleCollider2D col;
    Rigidbody2D rigid;
    public GameObject wall;
    float time = 0;
    bool isOnForce = false;
    public GameObject shadow;
    Vector3 originalShadowScale;
    float maxHeight = 20;
    float intHeight = 1;

    protected override void Start()
    {
        
        base.Start();
        col = this.GetComponent<CircleCollider2D>();
        rigid = this.GetComponent<Rigidbody2D>();
        originalShadowScale = shadow.transform.localScale;
        Stop();
        intHeight = col.radius;
        boundX = ItemManager.instance.houseItem.roomBoundX;
    }

    

    protected override void OnClick()
    {
        OnActive();
    }


    public override void OnActive()
    {
        if (isOnForce)
            return;

        if (state == EquipmentState.Active)
            return;
        time = 0;
        isOnForce = true;
        rigid.isKinematic = false;
        col.isTrigger = false;
        state = EquipmentState.Active;
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(Random.Range(-1000,1000),Random.Range(1000,5000)));
        rigid.AddTorque(Random.Range(-100, 100));
        lastPosition = this.transform.position;
        wall.SetActive(true);
        
    }

    private void Stop()
    {
        isOnForce = false;
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        rigid.isKinematic = true;
        col.isTrigger = true;
        state = EquipmentState.Idle;
        wall.SetActive(false);
    }

    protected override void Update()
    { 
        base.Update();
        if(state == EquipmentState.Active)
        {
            lastPosition.x = this.transform.position.x;
            float height = this.transform.position.y - lastPosition.y;
            shadow.transform.localScale = (1 - lastPosition.y * scaleFactor) * originalShadowScale * (1f - 0.5f * height / maxHeight);
            shadow.transform.position = lastPosition + new Vector3(0, -intHeight * this.transform.localScale.x, 100);
            Vector3 pos1 = wall.transform.position;
            pos1.y = lastPosition.y - intHeight * this.transform.localScale.x;
            wall.transform.position = pos1;
            time += Time.deltaTime;
            if (time > 1)
                isOnForce = false;
            if (time > 5 && Mathf.Abs(this.transform.position.y - lastPosition.y) < 0.3f)
            {
                Vector3 pos = this.transform.position;
                pos.y = lastPosition.y;
                this.transform.position = pos;
                Stop();
            }
                
        }
        else
        {
            if(this.transform.position.y > boundY.y)
            {
                Vector3 pos = this.transform.position;
                pos.y = boundY.y;
                this.transform.position = pos;

            }else if(this.transform.position.y < boundY.x)
            {
                Vector3 pos = this.transform.position;
                pos.y = boundY.x;
                this.transform.position = pos;
            }
            rigid.angularVelocity = 0;
            rigid.velocity = Vector2.zero;
            rigid.isKinematic = true;

            shadow.transform.position = this.transform.position + new Vector3(0, -intHeight * this.transform.localScale.x, 100);
            Vector3 pos1 = wall.transform.position;
            pos1.y = this.transform.position.y - 1 * this.transform.localScale.x;
            wall.transform.position = pos1;
            lastPosition = this.transform.position;
            Vector3 pos2 = this.transform.position;
            pos2.z = 10 * pos1.y;
            this.transform.position = pos2;
        }
    }

    protected override void LateUpdate()
    {
        if(state == EquipmentState.Active)
        {
            
            
        }
        else
        {
            transform.localScale = originalScale * (1 + (-transform.position.y) * scaleFactor);
            Vector3 pos = this.transform.position;
            pos.z = (this.transform.position.y - 1 * this.transform.localScale.x) * 10;
            this.transform.position = pos;
        }

    }

}
