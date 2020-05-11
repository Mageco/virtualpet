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
    Vector3 clickPosition;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        Vector3 pos = this.transform.position;
        pos.z = pos.y * 10;
        this.transform.position = pos;
        while (!ItemManager.instance.isLoad)
        {
            yield return new WaitForEndOfFrame();
        }
        Load();
    }

    public void Load()
    {
        //if (!GameManager.instance.IsHavedPet(petId) && GameManager.instance.myPlayer.questId > quesId)
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
            Debug.Log(p.GetName(0));
            petObject = Instantiate(petPrefab) as GameObject;
            petObject.transform.parent = this.transform;
            petObject.transform.localPosition = Vector3.zero;
            anim = petObject.GetComponent<Animator>();
            if(anim != null)
                anim.Play("Standby", 0);
        }

    }

    private void OnMouseDown()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (IsPointerOverUIObject())
            return;

        if (GameManager.instance.isGuest)
            return;

        if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) > 1f)
        {
            return;
        }

        if (!isClick && isActive)
        {
            UIManager.instance.OnPetRequirementPanel(DataHolder.GetPet(petId));
        }
    }

    public void DeActive()
    {
        if(petObject != null)
        {
            Destroy(petObject);
        }
        
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
