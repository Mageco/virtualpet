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
    public int itemId = 0;
    public GameObject item;

    private void Awake()
    {
        item.GetComponent<SpriteRenderer>().enabled = false;
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
        //ForestManager.instance.SpawnCoin(this.transform.position + new Vector3(0, 2, -1), value);
        //GameManager.instance.AddCoin(value, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        MageManager.instance.PlaySound("collect_item_02", false);

        StartCoroutine(SpawnItem());

        yield return new WaitForEndOfFrame();
        SpriteRenderer[] sprites = this.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = false;
        }
        if (shadow != null)
            shadow.SetActive(false);
        yield return new WaitForSeconds(Random.Range(30, 60));
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].enabled = true;
        }
        isActive = true;
        if (shadow != null)
            shadow.SetActive(true);

        item.GetComponent<SpriteRenderer>().enabled = false;
    }

    IEnumerator SpawnItem()
    {
        GameObject go = GameObject.Instantiate(item) as GameObject;
        go.SetActive(true);
        go.GetComponent<SpriteRenderer>().enabled = true;
        go.transform.position = this.transform.position;
        go.transform.parent = Camera.main.transform;
        Vector3 target = Camera.main.ScreenToWorldPoint(UIManager.instance.inventoryButton.transform.position) - Camera.main.transform.position;
        target.z = -100;
        while (Vector2.Distance(go.transform.localPosition, target) > 0.5)
        {
            go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, target, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        GameManager.instance.AddItem(itemId, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        Destroy(go);
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
