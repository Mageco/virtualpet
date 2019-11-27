using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDirty : MonoBehaviour
{

	public float maxDirty = 100;
	float dirty = 0;
	bool isClearning = false;
	Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
		originalScale = this.transform.localScale;
		dirty = maxDirty;
    }

    // Update is called once per frame
    void Update()
    {
		this.transform.localScale = dirty / maxDirty * originalScale;
		if(isClearning){
			OnClean(0.2f);
		}
    }

	public void OnClean(float clean)
	{
		dirty -= clean;
		if (dirty < 0){
			GameManager.instance.AddCoin(5);
			GameObject.Destroy (this.gameObject);
		}
			
	}

	void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
           other.GetComponent<CharController>().data.Dirty += this.dirty/10f;   
		}else if(other.tag == "Toilet"){
			isClearning = true;
		}        
    }
	
}
