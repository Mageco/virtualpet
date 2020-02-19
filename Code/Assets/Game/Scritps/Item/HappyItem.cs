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
    public GameObject body;
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
        OnPick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPick()
    {
        isPick = true;
        GameManager.instance.LogAchivement(AchivementType.CollectHeart);
        StartCoroutine(Pick());
    }

    IEnumerator Pick(){
        animator.enabled = false;
        this.GetComponent<CircleCollider2D>().enabled = false;
        Vector3 target = this.body.transform.position + new Vector3(0,10,0);
        target.z = -100;
        while(Vector2.Distance(this.body.transform.position,target) > 0.5)
        {
            this.body.transform.position = Vector3.Lerp(this.body.transform.position, target, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
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
