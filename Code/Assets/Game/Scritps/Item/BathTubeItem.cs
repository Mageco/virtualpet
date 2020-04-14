using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathTubeItem : MonoBehaviour
{

	Animator anim;
	bool isSoap = false;

	public int itemId = 0;
	public float clean = 10;


	void Awake()
	{
		anim = this.GetComponent <Animator> ();
	}
    // Start is called before the first frame update
    void Start()
    {
        itemId = this.transform.parent.GetComponentInParent<ItemObject>().itemID;
		clean = DataHolder.GetItem(itemId).value;
    }

    // Update is called once per frame
    void Update()
    {

    }

	public void OnSoap()
	{
		if (!isSoap) {
			anim.Play ("Bath_Start", 0);
			isSoap = true;
		}
	}

	public void OnShower()
	{
		if (isSoap) {
			anim.Play ("Bath_End", 0);
			isSoap = false;
			foreach(CharController pet in GameManager.instance.petObjects){
				if (pet.actionType == ActionType.OnBath && pet.data.dirty >= pet.data.MaxDirty * 0.3f) {
					pet.data.dirty -= clean;
					ItemManager.instance.SpawnHeart((pet.data.RateHappy + pet.data.level/5), this.transform.position);
					GameManager.instance.LogAchivement(AchivementType.Do_Action,ActionType.OnBath);
                }
			}
		}
	}
}
