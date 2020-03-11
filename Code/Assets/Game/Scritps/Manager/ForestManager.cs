using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForestManager : MonoBehaviour
{
    public static ForestManager instance;
    public GameObject dayBGG;
    public GameObject nightBG;
    float time = 0;
    public float maxTimeCheck = 20.1f;
    System.DateTime today;
    public GameObject coinPrefab;
    public GameObject starPrefab;
    public GameObject[] forestCoinPrefab;
    public GameObject fishPrefab;
    public GameObject[] charCollectors;
    public GameObject[] animalPrefabs;
    float timeCoin = 0;
    float maxTimeCoin = 3;
    float timeFish = 0;
    float maxTimeFish = 5;
    float timeAnimal = 0;
    float maxTimeAnimal = 10;
    GameObject collector;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            GameObject.Destroy(this.gameObject);

        maxTimeAnimal = Random.Range(20, 100);

    }
    // Start is called before the first frame update
    void Start()
    {
        today = System.DateTime.Today;
        CheckDayNight();
        LoadMusic();
        Invoke("LoadCollector", Random.Range(5, 20));
    }

    void LoadCollector()
    {
        int n = Random.Range(0, charCollectors.Length);
        collector = GameObject.Instantiate(charCollectors[n]) as GameObject;
        CharCollectorTimeline c = collector.GetComponentInChildren<CharCollectorTimeline>();
        if (GameManager.instance.IsHavePet(c.petId))
            GameObject.Destroy(collector);
    }

    public void CheckCollector()
    {
        if (collector != null)
            GameObject.Destroy(collector);
    }
   

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeCheck){
            time = 0;
            CheckDayNight();
        }else
        {
            time += Time.deltaTime;
        }

        if (timeCoin > maxTimeCoin)
        {
            timeCoin = 0;
            SpawnForestCoin();
            maxTimeCoin = Random.Range(10, 50);
        }
        else
            timeCoin += Time.deltaTime;

        if (timeFish > maxTimeFish)
        {
            timeFish = 0;
            SpawnFish();
            maxTimeFish = Random.Range(3, 10);
        }
        else
            timeFish += Time.deltaTime;

        if (timeAnimal > maxTimeAnimal)
        {
            timeAnimal = 0;
            SpawnAnimal();
            maxTimeAnimal = Random.Range(20, 100);
        }
        else
            timeAnimal += Time.deltaTime;
    }

    void CheckDayNight(){
        if(System.DateTime.Now.Hour < 6 || System.DateTime.Now.Hour > 18)
        {
            dayBGG.SetActive(false);
            nightBG.SetActive(true);
        } else{
            nightBG.SetActive(false);
            dayBGG.SetActive(true);
        }
    }

    public void LoadMusic()
    {
        MageManager.instance.PlayMusicName("Forest", true);
    }

    public void SpawnStar(Vector3 pos, int value)
    {
        GameObject go = Instantiate(starPrefab, pos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
        go.GetComponent<StarItem>().Load(value);
    }

    public void SpawnCoin(Vector3 pos, int value, GameObject item = null)
    {
        GameObject go = Instantiate(coinPrefab, pos, Quaternion.identity);
        go.GetComponent<CoinItem>().Load(value);
        if (item != null)
        {
            go.transform.parent = item.transform;
        }
        SpawnStar(pos, 1);
    }

    void SpawnForestCoin()
    {
        ForestCoinItem[] coins = FindObjectsOfType<ForestCoinItem>();
        if(coins.Length < 6)
        {
            Vector3 pos = GetRandomPoint(PointType.Banana).position;
            int n = Random.Range(0, 100);
            GameObject go = Instantiate(forestCoinPrefab[n / 10], pos, Quaternion.identity);
        }
    }

    void SpawnAnimal()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        if (animals.Length < 2)
        {
            Transform point = GetRandomPoint(PointType.ChickenDefence);
            int n = Random.Range(0, animalPrefabs.Length);
            GameObject go = Instantiate(animalPrefabs[n], point.position,point.rotation);
        }
    }

    void SpawnFish()
    {
        Vector3 pos = GetRandomPoint(PointType.Fishing).position;
        GameObject go = Instantiate(fishPrefab, pos, Quaternion.identity);
    }

    List<GizmoPoint> GetPoints(PointType type)
    {
        List<GizmoPoint> temp = new List<GizmoPoint>();
        GizmoPoint[] points = FindObjectsOfType<GizmoPoint>();
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].type == type)
                temp.Add(points[i]);
        }
        return temp;
    }

    public Transform GetRandomPoint(PointType type)
    {
        List<GizmoPoint> points = GetPoints(type);
        if (points != null && points.Count > 0)
        {
            int id = Random.Range(0, points.Count);
            return points[id].transform;
        }
        else
            return null;

    }

    public List<Transform> GetRandomPoints(PointType type)
    {
        List<GizmoPoint> points = GetPoints(type);
        List<Transform> randomPoints = new List<Transform>();
        for (int i = 0; i < points.Count; i++)
        {
            randomPoints.Add(points[i].transform);
        }

        for (int i = 0; i < randomPoints.Count; i++)
        {
            if (i < randomPoints.Count - 1)
            {
                int j = Random.Range(i, randomPoints.Count);
                Transform temp = randomPoints[i];
                randomPoints[i] = randomPoints[j];
                randomPoints[j] = temp;
            }
        }
        return randomPoints;
    }
}
