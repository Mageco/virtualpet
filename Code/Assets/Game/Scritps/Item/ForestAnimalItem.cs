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

        if(isActive && Vector3.Distance(clickPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1f)
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
        MageManager.instance.PlaySound("collect_item_02", false);
        yield return new WaitForEndOfFrame();
        SpriteRenderer[] sprites = this.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }
        if (shadow != null)
            shadow.SetActive(false);
        yield return new WaitForSeconds(Random.Range(10, 50));
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = true;
        }
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
