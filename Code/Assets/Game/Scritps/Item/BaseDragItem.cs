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
	public float maxHeight = 10;
    public float depth = -1;
	public float scaleFactor = 0.05f;
	Vector3 dragScale;
	public float height = 0;
    protected float originalHeight = 0;
	public Vector3 scalePosition = Vector3.zero;
	Vector3 lastPosition = Vector3.zero;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = this.GetComponent<Animator> ();
		originalPosition = this.transform.position;
		originalRotation = this.transform.rotation;
		originalScale = this.transform.localScale;
        scalePosition = this.transform.position + new Vector3(0,-height,0);
        originalHeight = height;
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

        scalePosition.x = this.transform.position.x;
		if(state == ItemDragState.Drag || state == ItemDragState.Highlight){
			float delta = this.transform.position.y - lastPosition.y;
			height += delta;
			if(height <= 0 && this.transform.position.y <= scalePosition.y ){
				Vector3 p = this.transform.position;
				p.y = lastPosition.y;
				this.transform.position = p;
				height = 0;
			}else{
				if(delta >= 0 && height > maxHeight){
					scalePosition.y += height - maxHeight;	
					height = maxHeight;
					if(scalePosition.y > depth){
						scalePosition.y = depth;
						Vector3 p = this.transform.position;
						p.y = lastPosition.y;
						this.transform.position = p;
					}
				}else if(delta < 0 && height > 0){
					if(scalePosition.y > -20){
						scalePosition.y += delta;
						height -= delta;
					}
				}		
			}
		}else if(state == ItemDragState.Drop || state == ItemDragState.Fall || state == ItemDragState.Hit){
			height = this.transform.position.y - scalePosition.y;
			if(height <= 0 && this.transform.position.y <= scalePosition.y ){
				Vector3 p = this.transform.position;
				p.y = scalePosition.y;
				this.transform.position = p;
				height = 0;
			}
		}

		dragScale = originalScale * (1 - scalePosition.y * scaleFactor);
		this.transform.localScale = Vector3.Lerp(dragScale,this.transform.localScale,Time.deltaTime *  3f);

		Vector3 pos = this.transform.position;
		pos.z = scalePosition.y * 10;
		this.transform.position = pos;

		lastPosition = this.transform.position;

    }

    protected virtual void OnDrag()
    {
        if (isDragable)
        {
            anim.Play("Drag");
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragOffset;
            pos.z = this.transform.position.z;
            pos.z = -100;
            if (pos.y > maxHeight)
                pos.y = maxHeight;
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
		pos.z = scalePosition.y;
		this.transform.position = pos;
		if (this.transform.position.y < scalePosition.y)
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
        GameManager.instance.ResetCameraTarget();
    }



	void OnMouseDown()
	{
        StartDrag();
	}

    

	void OnMouseUp()
	{
        EndDrag();
		dragOffset = Vector3.zero;
		if(state == ItemDragState.Drag){
            state = ItemDragState.Drop;
        }
			
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

