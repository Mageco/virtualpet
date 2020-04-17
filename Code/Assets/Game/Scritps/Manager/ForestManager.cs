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
    public string music = "Forest";
    float timeCoin = 0;
    float maxTimeCoin = 3;
    float timeFish = 0;
    float maxTimeFish = 5;
    float timeAnimal = 0;
    float maxTimeAnimal = 10;
    GameObject collector;
    public bool isForest = true;
    List<CharCollectorTimeline> collectorTimelines = new List<CharCollectorTimeline>();
    


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
        LoadCollectors();
        
    }



    void LoadCollectors()
    {
        if (isForest)
        {
            int cave = Random.Range(0, 100);
            int tree = Random.Range(0, 100);
            int board = Random.Range(0, 100);
            for (int i = 0; i < charCollectors.Length / 5; i++)
            {
                int n = Random.Range(0, 5);
                if (cave > 50 && i == 2)
                    StartCoroutine(LoadCollector(i * 5 + n));
                else if (cave <= 50 && i == 3)
                    StartCoroutine(LoadCollector(i * 5 + n));
                else if (i == 0 && tree > 50)
                    StartCoroutine(LoadCollector(i * 5 + n));
                else if (i == 4 && tree <= 50)
                    StartCoroutine(LoadCollector(i * 5 + n));
                else if (i == 1 && board > 50)
                    StartCoroutine(LoadCollector(i * 5 + n));

            }
        }
        else
        {
            for (int i = 0; i < charCollectors.Length / 5; i++)
            {
                int r = Random.Range(0, 100);
                int n = Random.Range(0, 5);
                if (r > 50)
                {
                    StartCoroutine(LoadCollector(i * 5 + n));
                }
            }
        }

    }

    IEnumerator LoadCollector(int id)
    {
        yield return new WaitForSeconds(Random.Range(0, 15));
        collector = GameObject.Instantiate(charCollectors[id]) as GameObject;
        CharCollectorTimeline c = collector.GetComponentInChildren<CharCollectorTimeline>();
        collector.transform.parent = this.transform;
        if (GameManager.instance.IsHavePet(c.petId))
            GameObject.Destroy(collector);
        if (collector != null)
            collectorTimelines.Add(collector.GetComponentInChildren<CharCollectorTimeline>());
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in audios)
        {
            if (a.gameObject != MageManager.instance.gameObject)
            {
                if (MageManager.instance.GetSoundVolume() < 0.1f)
                    a.enabled = false;
                else
                    a.enabled = true;
            }
        }
    }

    public void CheckCollector(int id)
    {
        foreach(CharCollectorTimeline c in collectorTimelines)
        {
            if(c.petId == id)
            {
                GameObject.Destroy(c.transform.parent.gameObject);
                collectorTimelines.Remove(c);
                return;
            }
        }
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
            maxTimeCoin = Random.Range(10, 30);
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
            maxTimeAnimal = Random.Range(20, 60);
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
        MageManager.instance.PlayMusicName(music, true);
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
            go.transform.parent = this.transform;
        }
    }

    void SpawnAnimal()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        if (animals.Length < 2 && animals.Length > 0)
        {
            Transform point = GetRandomPoint(PointType.ChickenDefence);
            int n = Random.Range(0, animalPrefabs.Length);
            GameObject go = Instantiate(animalPrefabs[n], point.position,point.rotation);
        }
    }

    void SpawnFish()
    {
        if(fishPrefab != null)
        {
            Vector3 pos = GetRandomPoint(PointType.Fishing).position;
            GameObject go = Instantiate(fishPrefab, pos, Quaternion.identity);

        }
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
