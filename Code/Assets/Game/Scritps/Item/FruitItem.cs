using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FruitItem : MonoBehaviour
{
    [HideInInspector]
    public ItemSaveDataType itemSaveDataType = ItemSaveDataType.Fruit;
    public int id;
    public GameObject collectEffect;
    public GameObject[] steps;
    public int step = 0;
    public float[] maxTime;
    public float time = 0;
    public int scaleStepId = 1;
    public float minScale = 0.1f;
    public float maxScale = 1f;
    public float maxTimeCalculated = 1;
    float timeCaculated = 0;
    Animator animator;

    void Awake(){
        collectEffect.SetActive(false);
        OnStep();
        animator = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
       
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

    void OnMouseUp(){

        if (IsPointerOverUIObject())
            return;

        if(step == steps.Length - 1){
            StartCoroutine(Pick());
        }
        
    }

    IEnumerator Pick(){
        step = 0;
        time = 0;
        MageManager.instance.PlaySoundName("happy_collect_item_01",false);
        GameManager.instance.AddCoin(Random.Range(2, 5));
        GameManager.instance.LogAchivement(AchivementType.CollectFruit);
        collectEffect.SetActive(true);
        animator.Play("Active");
        yield return new WaitForSeconds(1);
        OnStep();
        collectEffect.SetActive(false);
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
