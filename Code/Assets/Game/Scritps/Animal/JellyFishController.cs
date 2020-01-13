using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishController : FishController
{
    Vector3 target = Vector3.zero;
    Rigidbody2D rigid;
    public BoxCollider2D seaBed;

    protected override void Start()
    {
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-15, 15)));
        rigid = this.GetComponent<Rigidbody2D>();
        rigid.gravityScale = 0.03f;
        
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
        while (Vector2.Distance(this.transform.position, target) > 1 && time < maxTime && this.transform.position.y < -1 && !isAbort)
        {
            rigid.velocity = transform.up * speed * Time.deltaTime;
            Vector3 direction = target - this.transform.position;
            float rotatingIndex = Vector3.Cross(direction, transform.up).z;
            rigid.angularVelocity = -1 * rotatingIndex * 50 * Time.deltaTime;
            time += Time.deltaTime;
            if (this.transform.rotation.eulerAngles.z > 30 )
            {

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(new Vector3(0, 0, 30)), Time.deltaTime);
            }
            else if (this.transform.rotation.eulerAngles.z < -30)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(new Vector3(0, 0, -30)), Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }
        if (rigid.IsTouching(seaBed))
        {
            this.transform.rotation = Quaternion.identity;
        }
        state = FishState.Idle;
        //anim.Play("Idle", 0);
        yield return new WaitForSeconds(Random.Range(1f,2f));
        Move();
    }

    private void LateUpdate()
    {
        Debug.Log(this.transform.rotation.z);

    }
}
