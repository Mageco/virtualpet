using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChickenController : AnimalController
{
    
    public GameObject[] bodies;
    public InteractType interactType;
    Vector3 dragOffset;

    CircleCollider2D col2D;

    protected override void Load(){
        speed = maxSpeed/2f;
        col2D = this.GetComponent<CircleCollider2D>();
        //this.originalScale = this.originalScale * Random.Range(0.9f,1.1f);
    }

    public void OnHold(){
        state = AnimalState.Hold;
        isAbort = true;
    }

    public override void OnFlee()
    {
        state = AnimalState.Flee;
        isAbort = true;
    }

    public void OnCached(){
        for(int i=0;i<bodies.Length;i++){
            bodies[i].SetActive(false);
        }
        col2D.enabled = false;
        state = AnimalState.Cached;
        isAbort = true;
        MageManager.instance.PlaySound3D("Chicken_Catched", false,this.transform.position);
    }

    public void OffCached(){
        for(int i=0;i<bodies.Length;i++){
            bodies[i].SetActive(true);
        }
        col2D.enabled = true;
        state = AnimalState.None;
        isAbort = true;
        MageManager.instance.PlaySound3D("Chicken_Idle", false, this.transform.position);
    }

    protected override void Think()
    {
        int ran = Random.Range(0, 100);
        if (ran > 45)
        {
            state = AnimalState.Run;
        }
        else if(ran > 35)
        {
            state = AnimalState.Eat;
        }
        else
        {
            state = AnimalState.Idle;
        }
    }

    protected override void DoAction()
    {
        isAbort = false;
        isArrived = false;
        if (state == AnimalState.Idle)
        {
            StartCoroutine(Idle());
        }
        else if (state == AnimalState.Eat)
        {
            StartCoroutine(Eat());
        }
        else if (state == AnimalState.Run)
        {
            StartCoroutine(Run());
        }
        else if (state == AnimalState.Hold)
        {
            StartCoroutine(Hold());
        }else if (state == AnimalState.InActive)
        {
            this.gameObject.SetActive(false);
        }else if(state == AnimalState.Flee)
        {
            StartCoroutine(Flee());
        }
    }

    IEnumerator Idle()
    {
        int n = Random.Range(1,4);
        for(int i=0;i<n;i++){
            int ran = Random.Range(0,100);
            if(ran > 50)
                direction = Direction.L;
            else
                direction = Direction.R;
            anim.Play("Idle_"+direction.ToString(),0);

            if(ran > 80)
                MageManager.instance.PlaySound3D("Chicken_Idle", false, this.transform.position);
            yield return StartCoroutine(Wait(Random.Range(1,2)));
        }
        CheckAbort();
    }

    IEnumerator Eat()
    {
        int n = Random.Range(1,4);
        for(int i=0;i<n;i++){
            int ran = Random.Range(0,100);
            if(ran > 50)
                direction = Direction.L;
            else
                direction = Direction.R;
            ran = Random.Range(0, 100);
            
            if (ran > 98 && !isMinigame)
                SpawnHappy();
            yield return StartCoroutine(DoAnim("Eat_" + direction.ToString()));
        }
        CheckAbort();
    }

    IEnumerator Run()
    {
        Vector3 target = Vector3.zero;
        if (isMinigame)
            target = Minigame.instance.GetPointInBound() / 3f;
        else
            target = originalPosition + new Vector3(Random.Range(-10f, 10f), Random.Range(-5, 5),0);
        speed = Random.Range(maxSpeed/2,maxSpeed/1.5f);
        if(target.x > this.transform.position.x){
            SetDirection(Direction.R);
        }else
            SetDirection(Direction.L);
        anim.Play("Run_" + direction.ToString(),0);

        yield return StartCoroutine(MoveToPoint(target));
        CheckAbort();
    }

    IEnumerator Flee()
    {
        MageManager.instance.PlaySound3D("Chicken_Flee", false, this.transform.position);
        Vector3 target = ItemManager.instance.GetRandomPoint(AreaType.Garden);
        speed = maxSpeed * 3;
        if (target.x > this.transform.position.x)
        {
            SetDirection(Direction.R);
        }
        else
            SetDirection(Direction.L);
        anim.Play("Run_" + direction.ToString(), 0);

        yield return StartCoroutine(MoveToPoint(target));
        CheckAbort();
    }

    IEnumerator Hold()
    {
        interactType = InteractType.Drag;
        Vector3 dropPosition = Vector3.zero;


        while (interactType == InteractType.Drag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
            pos.z = 0;
            if (pos.y > 10)
                pos.y = 10;
            else if (pos.y < -30)
                pos.y = -30;

            if (pos.x > 50)
                pos.x = 50;
            else if (pos.x < -50)
                pos.x = -50;

            pos.z = -50;
            agent.transform.position = pos;
            this.transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        //Start Drop

        CheckAbort();
    }

    void OnMouseDown()
    {

    }

    void OnMouseUp()
    {

    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void SpawnHappy()
    {
        if(ItemManager.instance != null)
            ItemManager.instance.SpawnHeart(1, this.transform.position);
    }

}
