using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCollector : MonoBehaviour
{
    public int petId = 0;
    public GameObject petPrefab;
    GameObject petObject;
    Animator anim;
    bool isClick = false;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Active()
    {
        isActive = true;
        Pet p = DataHolder.GetPet(petId);
        petObject = Instantiate(petPrefab) as GameObject;
        petObject.transform.parent = this.transform;
        petObject.transform.localPosition = Vector3.zero;
        anim = petObject.GetComponent<Animator>();
        anim.Play("Standby", 0);
    }

    private void OnMouseUp()
    {
        if (!isClick && isActive)
        {
            StartCoroutine(Speak());
            UIManager.instance.OnPetRequirementPanel(DataHolder.GetPet(petId));
        }
    }

    IEnumerator Speak()
    {
        isClick = true;
        anim.Play("Speak_L", 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.Play("Standby", 0);
        isClick = false;
    }


    public void DeActive()
    {
        Destroy(petObject);
        isActive = false;
    }
}
