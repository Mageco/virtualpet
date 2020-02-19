using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StarItem : MonoBehaviour
{
    Animator animator;
    public int value = 1;
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
        StartCoroutine(Pick());
    }

    IEnumerator Pick(){
        animator.enabled = false;
        //MageManager.instance.PlaySoundName("happy_collect_item_01",false);
        yield return new WaitForEndOfFrame();
        Vector3 target = this.body.transform.position + new Vector3(0, 10, 0);
        while (Vector2.Distance(this.body.transform.position,target) > 0.5)
        {
            this.body.transform.position = Vector3.Lerp(this.body.transform.position, target, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        GameManager.instance.myPlayer.exp += value;
        Destroy(this.gameObject);
    }
}
