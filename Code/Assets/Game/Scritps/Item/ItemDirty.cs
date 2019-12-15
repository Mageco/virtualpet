using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDirty : MonoBehaviour
{

	public float maxDirty = 100;
	float dirty = 0;
	bool isClearning = false;
	Vector3 originalScale;
	public GameObject heartPrefab; 
    // Start is called before the first frame update
    void Start()
    {
		this.transform.localScale = maxDirty/100f * Vector3.one;
		originalScale = this.transform.localScale;
		dirty = maxDirty;
    }

    // Update is called once per frame
    void Update()
    {
		this.transform.localScale = (0.5f + dirty * 0.5f / maxDirty) * Vector3.one;
		if(isClearning){
			OnClean(0.2f);
		}
    }

	public void OnClean(float clean)
	{
		dirty -= clean;
		if (dirty < 0){
			if(heartPrefab != null){
				GameObject go = GameObject.Instantiate(heartPrefab,this.transform.position,Quaternion.identity);
				go.GetComponent<HappyItem>().Load(5);
			}
			GameObject.Destroy (this.gameObject);
		}
			
	}

	void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
           other.GetComponent<CharController>().data.Dirty += this.dirty/5f;   
		}else if(other.tag == "Toilet"){
			isClearning = true;
		}        
    }
	
}
