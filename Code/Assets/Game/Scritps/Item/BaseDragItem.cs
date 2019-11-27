using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseDragItem : MonoBehaviour
{
	Vector3 dragOffset;
    protected Animator anim;
    public ItemDragState state = ItemDragState.None;
    protected Vector3 originalPosition;
    protected Quaternion originalRotation;
    protected Vector3 originalScale;
	protected bool isDragable = true;
	bool isBusy = false;
    protected bool isHighlight = false;
	
    protected float fallSpeed = 0;
    Vector3 dropPosition;

	public float initZ = -6;
	public float scaleFactor = 0.1f;
	Vector3 dragScale;
	Vector3 targetScale = Vector3.one;
	float offset = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		originalRotation = this.transform.rotation;
		originalScale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
     	if (state == ItemDragState.Drag ) {
            OnDrag();
		}else if(state == ItemDragState.Drop ){
			OnDrop();			
		}else if(state == ItemDragState.Fall){
			OnFall();
		}
		else if(state == ItemDragState.Hit){
			OnHit();
		}
        else if (state == ItemDragState.Highlight)
        {
            OnHighlight();
        }else if(state == ItemDragState.Active)
        {
            OnActive();
        }


    }

	void LateUpdate()
    {
		offset = initZ;
		if (state == ItemDragState.Drop || state == ItemDragState.Fall || state == ItemDragState.Hit)
		{
			//return;
			targetScale = dragScale;
		}
		else if (state == ItemDragState.Drag || state == ItemDragState.Highlight) {
			offset = initZ + 20;
			if (this.transform.position.y > 2)
			{
				if (this.transform.position.y < offset)
					targetScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
				else
					targetScale = originalScale;
			}else{
				targetScale = originalScale * (1 + (-2 + offset) * scaleFactor);
			}
			dragScale = targetScale;
		}else{
			if (this.transform.position.y < offset)
				targetScale = originalScale * (1 + (-this.transform.position.y + offset) * scaleFactor);
			else
				targetScale = originalScale;
		}

		//Debug.Log(gameObject.name + " "+ originalScale + "  " + targetScale);
		this.transform.localScale = Vector3.Lerp(targetScale,this.transform.localScale,Time.deltaTime *  3f);

    }

    protected virtual void OnDrag()
    {
        if (isDragable)
        {
            anim.Play("Drag");
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
            pos.z = this.transform.position.z;
            pos.z = -50;
            if (pos.y > 20)
                pos.y = 20;
            else if (pos.y < -20)
                pos.y = -20;

            if (pos.x > 52)
                pos.x = 52;
            else if (pos.x < -49)
                pos.x = -49;
            this.transform.position = pos;
            if (isHighlight)
                state = ItemDragState.Highlight;
        }
    }

    protected virtual void OnHighlight()
    {
        if (isDragable)
        {

            anim.Play("Highlight");
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
            pos.z = this.transform.position.z;
            pos.z = -50;
            if (pos.y > 20)
                pos.y = 20;
            else if (pos.y < -20)
                pos.y = -20;

            if (pos.x > 52)
                pos.x = 52;
            else if (pos.x < -49)
                pos.x = -49;
            this.transform.position = pos;
            if (!isHighlight)
                state = ItemDragState.Drag;
        }
    }

    protected virtual void OnHit()
	{
       
            
	}

    protected virtual void OnDrop()
	{
		RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position + new Vector3(0, -2, 0), -Vector2.up, 100);
		Vector3 pos2 = this.transform.position;
		pos2.y = pos2.y - 22;
		if (pos2.y < -20)
			pos2.y = -20;
		dropPosition = pos2;

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i].collider.tag == "Table")
			{
				pos2.y = hit[i].collider.transform.position.y;
				dropPosition = pos2 + new Vector3(0, 0.5f, 0);
				break;
			}
		}
        state = ItemDragState.Fall;
    }

    protected virtual void OnActive()
    {

    }

    protected virtual void OnFall()
	{
		fallSpeed += 100f * Time.deltaTime;
		if (fallSpeed > 50)
			fallSpeed = 50;
		Vector3 pos = this.transform.position;
		pos.y -= fallSpeed * Time.deltaTime;
		pos.z = dropPosition.y;
		this.transform.position = pos;
		if (Vector2.Distance(this.transform.position, dropPosition) < 1f)
		{
			state = ItemDragState.Hit;
		}
	}

	protected IEnumerator DoAnim(string a)
    {
        anim.Play(a, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    protected IEnumerator ReturnPosition(Vector3 pos)
    {
        isBusy = true;
        while (Vector2.Distance(this.transform.position, pos) > 0.1f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }
        isBusy = false;

    }

    protected IEnumerator ReturnRotation(Quaternion rot)
    {
        isBusy = true;
        while (Quaternion.Angle(this.transform.rotation, rot) > 1f)
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rot, Time.deltaTime * 40);
            yield return new WaitForEndOfFrame();
        }
        isBusy = false;
    }

    public virtual void StartDrag(){
        if (IsPointerOverUIObject())
            return;

        if (isBusy)
            return;

        if (state == ItemDragState.None)
        {
            dragOffset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
            state = ItemDragState.Drag;
            GameManager.instance.SetCameraTarget(this.gameObject);
        }

    }

    public virtual void EndDrag()
    {
        
    }



	void OnMouseDown()
	{
        StartDrag();
	}

    

	void OnMouseUp()
	{
		dragOffset = Vector3.zero;
		if(state == ItemDragState.Drag)
			state = ItemDragState.Drop;
        else if(state == ItemDragState.Highlight)
        {
            state = ItemDragState.Active;
        }
		
	}


	private bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

