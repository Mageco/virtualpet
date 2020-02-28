using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForestCoinItem : MonoBehaviour
{
    public int value;
    Animator animator;
    Vector3 clickPosition;
    bool isClick = false;
    public bool isOrder = true;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isOrder)
        {
            Vector3 pos = this.transform.position;
            pos.z = pos.y * 10;
            this.transform.position = pos;
            this.transform.localScale = this.transform.localScale * (1 - this.transform.position.y * 0.1f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnMouseDown()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {

        if (IsPointerOverUIObject())
            return;

        if(!isClick && Vector3.Distance(clickPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1f)
            StartCoroutine(Pick());

    }

    IEnumerator Pick()
    {
        isClick = true;
        animator.Play("Active", 0);
        ForestManager.instance.SpawnCoin(this.transform.position + new Vector3(0, 2, -1), value, this.gameObject);
        GameManager.instance.AddCoin(value);
        MageManager.instance.PlaySound("collect_item_02", false);
        GameManager.instance.LogAchivement(AchivementType.CollectFruit);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(this.gameObject);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
