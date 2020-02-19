using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HappyItem : MonoBehaviour
{
    public ItemSaveDataType itemSaveDataType = ItemSaveDataType.Happy;
    Animator animator;
    public int value = 5;
    bool isPick = false;
    bool isSound = true;
    public void Load(int e,bool isSound){
        value = e;
        this.isSound = isSound;
    }

    void Awake(){
        animator = this.GetComponent<Animator>();
        
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(isSound)
            MageManager.instance.PlaySoundName("Button",false);
        Vector3 pos = this.transform.position;
        pos.z = (this.transform.position.y - 2) * 10;
        this.transform.position = pos;
        yield return new WaitForSeconds(0.5f);
        //OnPick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseUp(){

        
        if (IsPointerOverUIObject())
            return;

        if(!isPick){
            isPick = true;
            GameManager.instance.LogAchivement(AchivementType.CollectHeart);
            StartCoroutine(Pick());
        }
        
    }

    public void OnPick()
    {
        isPick = true;
        GameManager.instance.LogAchivement(AchivementType.CollectHeart);
        StartCoroutine(Pick());
    }

    IEnumerator Pick(){
        MageManager.instance.PlaySoundName("happy_collect_item_01",false);
        animator.Play("Pick");
        this.GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        GameManager.instance.AddHappy(value);
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
