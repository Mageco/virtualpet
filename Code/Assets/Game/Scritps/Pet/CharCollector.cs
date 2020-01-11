using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharCollector : MonoBehaviour
{
    public int petId = 0;
    public int quesId = 0;
    public GameObject petPrefab;
    GameObject petObject;
    Animator anim;
    bool isClick = false;
    bool isActive = false;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (!GameManager.instance.isLoad)
        {
            yield return new WaitForEndOfFrame();
        }
        Load();
    }



    public void Load()
    {
        if (!GameManager.instance.IsEquipPet(petId) && GameManager.instance.myPlayer.questId > quesId)
            Active();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Active()
    {
        if (!isActive)
        {
            isActive = true;
            Pet p = DataHolder.GetPet(petId);
            petObject = Instantiate(petPrefab) as GameObject;
            petObject.transform.parent = this.transform;
            petObject.transform.localPosition = Vector3.zero;
            anim = petObject.GetComponent<Animator>();
            anim.Play("Standby", 0);
        }

    }

    private void OnMouseUp()
    {
        if (IsPointerOverUIObject())
            return;

        if (!isClick && isActive)
        {
            StartCoroutine(Speak());
            UIManager.instance.OnPetRequirementPanel(DataHolder.GetPet(petId));
        }
    }

    IEnumerator Speak()
    {
        isClick = true;
        if(anim != null)
            anim.Play("Speak_L", 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        if (anim != null)
            anim.Play("Standby", 0);
        isClick = false;
    }


    public void DeActive()
    {
        Destroy(petObject);
        isActive = false;
    }

    protected bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
