using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDirty : MonoBehaviour
{
	public ItemSaveDataType itemType = ItemSaveDataType.None;
	public float dirty = 0;
	bool isClearning = false;
	Vector3 originalScale;
	public Vector3 position;
	Vector3 clickPosition;
    // Start is called before the first frame update
    void Start()
    {
		this.transform.localScale = (1f + dirty / 200) * Vector3.one;
		originalScale = this.transform.localScale;
		position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		this.transform.localScale = (1f + dirty / 200) * Vector3.one;
		if(isClearning){
			OnClean(0.2f);
		}
    }

	public void OnClean(float clean)
	{
		dirty -= clean;
		if (dirty <= 0){
			Destroy(this.gameObject);
		}
			
	}

    private void OnMouseDown()
    {
		clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

    private void OnMouseUp()
    {
        if(Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 1f)
        {
			MageManager.instance.PlaySound3D("happy_collect_item_01", false, this.transform.position);
			ItemManager.instance.SpawnStar(this.transform.position, 1);
			Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
           other.GetComponent<CharController>().data.Dirty += this.dirty/200f;   
		}else if(other.tag == "Toilet"){
			isClearning = true;
		}        
    }
	
}
