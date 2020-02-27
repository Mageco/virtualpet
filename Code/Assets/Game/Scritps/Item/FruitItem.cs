using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FruitItem : MonoBehaviour
{
    [HideInInspector]
    public ItemSaveDataType itemSaveDataType = ItemSaveDataType.Fruit;
    public int id = 0;
    public GameObject[] steps;
    public int step = 0;
    public float[] maxTime;
    public float time = 0;
    public int scaleStepId = 1;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public float maxTimeCalculated = 1;
    float timeCaculated = 0;
    CircleCollider2D collider;
    Vector3 clickPosition;

    void Awake(){
        collider = this.GetComponent<CircleCollider2D>();
        step = Random.Range(0, steps.Length);
        time = Random.Range(0, maxTime[step]);
        OnStep();
       
    }
    // Start is called before the first frame update
    void Start()
    {
        if (id == 0)
            id = ItemManager.instance.GetFruitId();
    }

    public void Load()
    {
        OnStep();
    }

    // Update is called once per frame
    void Update()
    {
        if(time < maxTime[step])
        {
            time += Time.deltaTime;
        }
        else
        {
            if(step < steps.Length - 1)
                Grow();
        }

        if(timeCaculated > maxTimeCalculated)
        {
            if (step == scaleStepId)
            {
                float s = steps[step].transform.localScale.x;
                s += (maxScale - minScale)/maxTime[scaleStepId]*Time.deltaTime;
                if (s > maxScale)
                    s = maxScale;
                steps[step].transform.localScale = new Vector3(s, s, 1);
            }
        }
        else
        {
            timeCaculated += Time.deltaTime;
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
        step = 0;
        time = 0;
        OnStep();
        int value = Random.Range(1, 3);
        ItemManager.instance.SpawnCoin(this.transform.position + new Vector3(0, 2, -1), value, this.gameObject);
        GameManager.instance.AddCoin(value); 
        MageManager.instance.PlaySoundName("happy_collect_item_01",false);
        GameManager.instance.LogAchivement(AchivementType.CollectFruit);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void OnStep()
    {
        for(int i = 0; i < steps.Length; i++)
        {
            if (i != step)
                steps[i].SetActive(false);
            else
                steps[i].SetActive(true);
        }

        if (step == scaleStepId)
        {
            steps[step].transform.localScale = new Vector3(minScale, minScale, 1);
        }

        if (step < steps.Length - 1)
        {
            collider.enabled = false;
        }
        else
            collider.enabled = true;
    }

    void Grow()
    {
        step++;
        time = 0;
        OnStep();
        if(step == scaleStepId)
        {
            steps[step].transform.localScale = new Vector3(minScale, minScale, 1);
        }
    }
}
