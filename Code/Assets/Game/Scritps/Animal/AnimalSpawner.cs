using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{

    List<GizmoPoint> spawnPoints = new List<GizmoPoint>();
    public int minNumber;
    public int maxNumber;
    public GameObject[] prefabs;
    List<AnimalController> animals = new List<AnimalController>();
    // Start is called before the first frame update
    void Awake()
    {
        GizmoPoint[] temp = this.transform.GetComponentsInChildren<GizmoPoint>();
        for(int i=0;i<temp.Length;i++){
            spawnPoints.Add(temp[i]);
        }
        Spawn();

    }


    void Spawn(){
        int n = Random.Range(minNumber,maxNumber + 1 );
        for(int i=0;i<n;i++){
            int id = Random.Range(0,prefabs.Length);
            int id1 = Random.Range(0,spawnPoints.Count);
           
            GameObject go = GameObject.Instantiate(prefabs[id]);
            go.transform.position = spawnPoints[id1].transform.position;
            go.transform.parent = this.transform;
            spawnPoints.RemoveAt(id1);
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
