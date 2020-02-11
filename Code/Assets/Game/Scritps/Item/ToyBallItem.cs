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


    protected override void Start()
    {
        
        base.Start();
        col = this.GetComponent<CircleCollider2D>();
        rigid = this.GetComponent<Rigidbody2D>();
        originalShadowScale = shadow.transform.localScale;
        Stop();
        
    }

    

    protected override void OnClick()
    {
        OnActive();
    }


    public override void OnActive()
    {
        if (isOnForce)
            return;
        time = 0;
        isOnForce = true;
        rigid.isKinematic = false;
        col.isTrigger = false;
        state = ToyState.Active;
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(Random.Range(-1000,1000),Random.Range(1000,5000)));
        rigid.AddTorque(Random.Range(-100, 100));
        lastPosition = this.transform.position;
    }

    private void Stop()
    {
        isOnForce = false;
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        rigid.isKinematic = true;
        col.isTrigger = true;
        state = ToyState.Idle;
    }

    protected override void Update()
    { 
        base.Update();
        if(state == ToyState.Active)
        {
            lastPosition.x = this.transform.position.x;
            float height = this.transform.position.y - lastPosition.y;
            shadow.transform.localScale = (1 - lastPosition.y * scaleFactor) * originalShadowScale * (1f - 0.5f * height / maxHeight);
            shadow.transform.position = lastPosition + new Vector3(0, -1 * this.transform.localScale.x, 100);
            Vector3 pos1 = wall.transform.position;
            pos1.y = lastPosition.y -1 * this.transform.localScale.x;
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
            rigid.angularVelocity = 0;
            rigid.velocity = Vector2.zero;
            rigid.isKinematic = true;

            shadow.transform.position = this.transform.position + new Vector3(0, -1*this.transform.localScale.x, 100);
            Vector3 pos1 = wall.transform.position;
            pos1.y = this.transform.position.y - 1 * this.transform.localScale.x;
            wall.transform.position = pos1;
            lastPosition = this.transform.position;
        }
    }

    protected override void LateUpdate()
    {
        if(state == ToyState.Active)
        {

        }
        else
        {
            base.LateUpdate();
        }

    }

}
