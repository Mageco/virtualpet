using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour
{
    public GameObject[] flyPrefabs;
    public int maxCount = 3;
    public Transform[] spawnPoints;
    float time = 0;
    float maxTimeSpawn = 1;
    List<GameObject> flies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        maxCount = Random.Range(1,maxCount);
    }

    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeSpawn && flies.Count < maxCount){
            SpawnFly();
            maxTimeSpawn = Random.Range(1,3);
            time = 0;
        }else{
            time += Time.deltaTime;
        }        
    }

    public void SpawnFly(){
        int n = Random.Range(0,spawnPoints.Length);
        int id = Random.Range(0,flyPrefabs.Length);
        GameObject go = GameObject.Instantiate(flyPrefabs[id],spawnPoints[n].position,Quaternion.identity);
        go.transform.parent = this.transform;
        flies.Add(go);
    }


}
