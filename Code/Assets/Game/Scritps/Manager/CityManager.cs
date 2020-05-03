using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    public static CityManager instance;
    public GameObject dayBGG;
    public GameObject nightBG;
    float time = 0;
    public float maxTimeCheck = 20.1f;
    System.DateTime today;
    public string music = "Forest";
    public GameObject coinPrefab;
    public GameObject starPrefab;
    //public int activePetId = 0;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            GameObject.Destroy(this.gameObject);


    }
    // Start is called before the first frame update
    void Start()
    {
        today = System.DateTime.Today;
        CheckDayNight();
        LoadMusic();
        //SpawnPet(activePetId);
    }


  



    // Update is called once per frame
    void Update()
    {
        if (time > maxTimeCheck)
        {
            time = 0;
            CheckDayNight();
        }
        else
        {
            time += Time.deltaTime;
        }

        
    }

    void CheckDayNight()
    {
        if (System.DateTime.Now.Hour < 6 || System.DateTime.Now.Hour > 18)
        {
            dayBGG.SetActive(false);
            nightBG.SetActive(true);
        }
        else
        {
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

    public void OnSpinWheelPanel()
    {
        UIManager.instance.OnSpinWheelPanel();
    }


    public void OnDailyQuest()
    {
        UIManager.instance.OnDailyQuestPanel();
    }

    public void OnAccessoryPanel(int petId)
    {
        UIManager.instance.OnAccessoryPanel(petId);
    }
}
