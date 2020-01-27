using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    public bool isMinigame = true;
    public float speed = 10f;
    protected Vector3 originalPosition;
    protected PolygonCollider2D col;
    public FishState state = FishState.Move;
    protected Animator anim;
    protected Vector3 lastPosition = Vector3.zero;
    protected Quaternion targetRotation = Quaternion.identity;
    public int moveCount = 10;
    public float timeWait = 0;
    protected Vector3[] paths;
    public bool isAbort = false;
    public float weight = 1;
    public FishType fishType = FishType.Fish;


    void Awake()
    {
        col = this.transform.GetComponentInChildren<PolygonCollider2D>();
        originalPosition = this.transform.position;
        col.enabled = true;
        anim = this.GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        Spawn();

    }

    public void Spawn()
    {
        state = FishState.Move;
        Move();
    }

    protected virtual void Move()
    {
        state = FishState.Move;
        col.enabled = true;
        anim.Play("Move", 0);

        paths = new Vector3[moveCount];
        for (int i = 0; i < moveCount; i++)
        {
            paths[i] = Minigame.instance.GetPointInBound();
            paths[i].z = this.transform.position.z;
        }
        iTween.MoveTo(this.gameObject, iTween.Hash("path", paths, "speed", speed, "orienttopath", false, "easetype", "linear", "onupdate", "UpdateMove", "oncomplete", "CompleteMove"));
    }

    protected void CompleteMove()
    {
        StartCoroutine(CompleteMoveCoroutine());
    }

    protected virtual void UpdateMove()
    {
        Vector3 direction = (lastPosition - this.transform.position).normalized;
        direction.z = 0;
        if (state == FishState.Move)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angleY = 0;
            float scale = 1;
            if (angle > 90)
            {
                angle = 180 - angle;
                angleY = 180;
                scale = -1;
            }

            if (angle < -90)
            {
                angle = -180 - angle;
                angleY = 180;
                scale = -1;
            }

            this.transform.rotation = Quaternion.Euler(new Vector3(0, angleY, angle));
            this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, scale);
        }
    }



    protected virtual IEnumerator CompleteMoveCoroutine()
    {
        state = FishState.Idle;
        anim.Play("Idle", 0);
        yield return new WaitForSeconds(timeWait);
        Move();
    }



    private void LateUpdate()
    {
        lastPosition = this.transform.position;
    }


    public virtual void OnCached()
    {
        state = FishState.Cached;
        anim.Play("Hit", 0);
        //if(iTween.Check)
        iTween.Stop(this.gameObject);
        col.enabled = false;
        MageManager.instance.PlaySoundName("Collect_Item", false);
    }

    public void OnDeactive()
    {
        state = FishState.DeActive;
        this.gameObject.SetActive(false);
    }

    public virtual void OnActive()
    {
        col.enabled = true;
        this.transform.parent = null;
        Move();
    }
}
public enum FishState { Idle, Move, Cached, Active,DeActive };
public enum FishType {Fish,Squirt,JellyFish,YellowFish,SpecialFish,Rabbish};

