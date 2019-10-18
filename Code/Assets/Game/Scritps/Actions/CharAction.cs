using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Touch;
public class CharAction : MonoBehaviour
{
    protected CharController character;
    protected Animator anim;
    protected Rigidbody2D rigid;
    protected PolyNavAgent agent;
    protected CircleCollider2D collid;
    protected  bool isAbort;
    protected float interactTime = 0;
    protected float maxInteractTime = 3;


    void Awake()
    {
        character = this.transform.GetComponent<CharController>();
        anim = this.transform.GetComponent<Animator>();
        rigid = this.transform.GetComponent<Rigidbody2D>();
        agent = GameObject.FindObjectOfType<PolyNavAgent> ();
        collid = this.GetComponent <CircleCollider2D> ();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator DoAnim(string a)
	{
		float time = 0;
		anim.Play (a, 0);
		yield return new WaitForEndOfFrame ();
		while (time < anim.GetCurrentAnimatorStateInfo (0).length && !isAbort) {
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}


	protected bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

    protected void EndAction(){
        character.actionType = ActionType.None;
    }

}
