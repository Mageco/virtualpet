using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FruitItem : MonoBehaviour
{
    public int id = 0;
    public GameObject[] steps;
    public int step = 0;
    public float timeGrow = 420;
    public float time = 0;
    float minScale = 0.1f;
    float maxScale = 1f;
    CircleCollider2D col;
    Vector3 clickPosition;
    public int fruidId = 1;
    Vector3 originalPosition;
    Transform parent;
    int lastStep = 0;
    

    void Awake(){
        col = this.GetComponent<CircleCollider2D>();
        time = Random.Range(timeGrow, timeGrow * 2);
        originalPosition = this.transform.localPosition;
        parent = this.transform.parent;
    }
    // Start is called before the first frame update
    void Start()
    {
        //if (id == 0)
        //    id = ItemManager.instance.GetFruitId();
    }

    public void Load(float t)
    {
        time = t;
        Grow();
    }



    // Update is called once per frame
    void Update()
    {
        lastStep = step;
        time += Time.deltaTime;

        if (time > timeGrow)
        {
            step = 2;
        }
        else if (time > 10)
        {
            step = 1;
        }
        else
            step = 0;

        if(step != lastStep)
        {
            Grow();
        }
 
        if (step == 1)
        {
            float s = (maxScale - minScale)/timeGrow*time;
            if (s > maxScale)
                s = maxScale;
            steps[1].transform.localScale = new Vector3(s, s, 1);
        }
  
    }

    private void OnMouseDown()
    {
        clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp(){

        if (IsPointerOverUIObject())
            return;

        if(step == steps.Length - 1 && Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), clickPosition) < 1f)
        {
            Pick();
        }
    }

    void Pick(){
        if (!GameManager.instance.isGuest)
            StartCoroutine(PickCouroutine());
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    void Grow()
    {
        for (int i = 0; i < steps.Length; i++)
        {
            if (i == step)
                steps[i].SetActive(true);
            else
                steps[i].SetActive(false);
        }

        if (step < steps.Length - 1)
        {
            col.enabled = false;
        }
        else
            col.enabled = true;
    }

    IEnumerator PickCouroutine()
    {
        ItemManager.instance.SpawnCoin(this.transform.position, Random.Range(1, 4));
        MageManager.instance.PlaySound("happy_collect_item_01", false);
        this.transform.parent = Camera.main.transform;
        Vector3 target = Camera.main.ScreenToWorldPoint(UIManager.instance.inventoryButton.transform.position) - Camera.main.transform.position;
        target.z = -100;
        while (Vector2.Distance(this.transform.localPosition, target) > 0.5)
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, target, 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        time = 0;
        this.transform.parent = parent;
        this.transform.localPosition = originalPosition;

        //Item item = DataHolder.GetItem(fruidId);

        GameManager.instance.AddItem(fruidId, Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013"));
        GameManager.instance.LogAchivement(AchivementType.CollectFruit);
    }

    string GetKey()
    {
        return Utils.instance.Md5Sum(GameManager.instance.count.ToString() + GameManager.instance.myPlayer.playTime.ToString() + GameManager.instance.myPlayer.Happy.ToString() + "M@ge2013");
    }
}
