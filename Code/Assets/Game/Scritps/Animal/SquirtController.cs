using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirtController : FishController
{
    Vector3 target = Vector3.zero;
    Rigidbody2D rigid;
    public BoxCollider2D seaBed;
    public bool isRotate = true;

    protected override void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        rigid.gravityScale = 0.2f;
        Move();
    }

    protected override void Move()
    {
        StartCoroutine(CompleteMoveCoroutine());
    }

    protected override IEnumerator CompleteMoveCoroutine()
    {
        state = FishState.Idle;
        target = Minigame.instance.GetPointInBound();
        anim.Play("Move", 0);
        float time = 0;
        float maxTime = Random.Range(2, 10);
        while (Vector2.Distance(this.transform.position,target) > 1 && time < maxTime && this.transform.position.y < -1)
        {
            rigid.velocity = transform.up * speed * Time.deltaTime;
            Vector3 direction = target - this.transform.position;
            float rotatingIndex = Vector3.Cross(direction, transform.up).z;
            if(isRotate)
                rigid.angularVelocity = -1 * rotatingIndex * 50 * Time.deltaTime;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (rigid.IsTouching(seaBed))
        {
            this.transform.rotation = Quaternion.identity;
        }
        yield return new WaitForEndOfFrame();
        Move();
    }
}
