using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Lean.Touch;

public class ToyBallItem : BaseDragItem
{
    CircleCollider2D col;
    Rigidbody2D rigid;
    public GameObject wall;
    Vector3 lastPos = Vector3.zero;
    float time = 0;
    bool isOnForce = false;


    protected override void Start()
    {
        
        base.Start();
        col = this.GetComponent<CircleCollider2D>();
        rigid = this.GetComponent<Rigidbody2D>();
        Stop();
        
    }

    public override void StartDrag()
    {
        rigid.isKinematic = true;
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        col.isTrigger = true;
        if (IsPointerOverUIObject())
            return;

        if (isBusy)
            return;


        dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        state = ItemDragState.Drag;
        ItemManager.instance.SetCameraTarget(this.gameObject);

    }


    public override void EndDrag()
    {
        rigid.isKinematic = false;
        col.isTrigger = false;
        state = ItemDragState.Drop;
        Vector2 vel = new Vector2(this.transform.position.x - lastPos.x, this.transform.position.y - lastPos.y).normalized;
        StartCoroutine(AddForce(vel*5000));
        ItemManager.instance.ResetCameraTarget();
    }

    public void OnForce()
    {
        if (state == ItemDragState.Drag)
            return;
        if (isOnForce)
            return;
        isOnForce = true;
        rigid.isKinematic = false;
        col.isTrigger = false;
        state = ItemDragState.Drop;
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(Random.Range(-1000,1000),Random.Range(1000,5000)));
        rigid.AddTorque(Random.Range(-100, 100));
        Invoke("EndForce", 1);
    }

    void EndForce()
    {
        isOnForce = false;
    }

    IEnumerator AddForce(Vector2 f)
    {
        yield return new WaitForEndOfFrame();
        rigid.AddForce(f);
    }

    protected override void OnDrop()
    {

    }

    private void Stop()
    {
        rigid.angularVelocity = 0;
        rigid.velocity = Vector2.zero;
        rigid.isKinematic = true;
        col.isTrigger = false;
        height = minHeight;
        this.scalePosition = this.transform.position + new Vector3(0,-minHeight,0);
        state = ItemDragState.None;
    }

    protected override void Update()
    { 
        if (time > 0.5f)
        {
            time = 0;
            lastPos = this.transform.position;
        }
        else
            time += Time.deltaTime;
        
        base.Update();
    }

    protected override void LateUpdate()
    {

        scalePosition.x = this.transform.position.x;
        if (state == ItemDragState.Drag || state == ItemDragState.Highlight)
        {



            float delta = this.transform.position.y - lastPosition.y;
            height += delta;
            if (height <= minHeight && this.transform.position.y <= scalePosition.y + minHeight)
            {
                scalePosition.y = this.transform.position.y - minHeight;
                height = minHeight;
            }
            else
            {
                if (delta >= 0 && height > maxHeight)
                {
                    scalePosition.y += height - maxHeight;
                    height = maxHeight;
                    if (scalePosition.y > depth)
                    {
                        scalePosition.y = depth;
                        Vector3 p = this.transform.position;
                        p.y = lastPosition.y;
                        this.transform.position = p;
                    }
                }
                else if (delta < 0 && height > minHeight)
                {
                    if (scalePosition.y > -20)
                    {
                        scalePosition.y += delta;
                        height -= delta;
                    }
                }
            }

            Vector3 pos1 = wall.transform.position;
            pos1.y = this.scalePosition.y;
            wall.transform.position = pos1;
            wall.transform.localScale = Vector3.one + this.transform.localScale * -pos1.y * 0.008f;
        }
        else if (state == ItemDragState.Drop || state == ItemDragState.Fall || state == ItemDragState.Hit)
        {
            height = this.transform.position.y - scalePosition.y + minHeight;
            if (height <= minHeight && this.transform.position.y <= scalePosition.y)
            {
                Vector3 p = this.transform.position;
                p.y = scalePosition.y + minHeight;
                this.transform.position = p;
                height = minHeight;
            }
        }



        dragScale = originalScale * (1 - scalePosition.y * scaleFactor);
        this.transform.localScale = Vector3.Lerp(dragScale, this.transform.localScale, Time.deltaTime * 3f);

        if (shadow != null)
        {

            shadow.transform.localScale = (1 - scalePosition.y * scaleFactor) * originalShadowScale * (1f - 0.5f * height / maxHeight);
            shadow.transform.position = scalePosition + new Vector3(0, 0, 100);
        }

        Vector3 pos = this.transform.position;
        pos.z = scalePosition.y * 10;
        this.transform.position = pos;

        lastPosition = this.transform.position;

    }

}
