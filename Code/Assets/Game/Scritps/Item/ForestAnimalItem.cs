using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForestAnimalItem : MonoBehaviour
{
    public int value;
    bool isActive = true;
    Vector3 clickPosition;
    public GameObject shadow;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseUp()
    {

        if (IsPointerOverUIObject())
            return;

        if(isActive && Vector3.Distance(clickPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 0.3f)
            StartCoroutine(Pick());

    }

    private void OnMouseDown()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    IEnumerator Pick()
    {
        isActive = false;
        ForestManager.instance.SpawnCoin(this.transform.position + new Vector3(0, 2, -1), value);
        GameManager.instance.AddCoin(value);
        MageManager.instance.PlaySoundName("happy_collect_item_01", false);
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false);
        if (shadow != null)
            shadow.SetActive(false);
        yield return new WaitForSeconds(Random.Range(10, 20));
        this.gameObject.SetActive(true);
        isActive = true;
        if (shadow != null)
            shadow.SetActive(true);
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
