using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    Animator animator;
    int petId = 0;
    bool isPick = false;
    void Load(int id){
        petId = id;
    }

    void Awake(){
        animator = this.GetComponent<Animator>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = this.transform.position;
        pos.z = (this.transform.position.y - 2) * 10;
        this.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseUp(){
        if(!isPick){
            isPick = true;
            StartCoroutine(Pick());
        }
        
    }

    IEnumerator Pick(){
        animator.Play("Pick");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        GameManager.instance.AddCoin(Random.Range(5,10)*(GameManager.instance.GetPet(petId).level + 1) );
        GameObject.Destroy(this.gameObject);
    }
}
