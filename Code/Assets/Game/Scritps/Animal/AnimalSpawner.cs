using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{

    List<GizmoPoint> spawnPoints = new List<GizmoPoint>();
    public int maxNumber;
    public float speed = 10;
    public GameObject[] prefabs;
    List<AnimalController> animals = new List<AnimalController>();
    // Start is called before the first frame update
    void Awake()
    {
  
 
    }


   public  void Spawn(){
        GizmoPoint[] temp = this.transform.GetComponentsInChildren<GizmoPoint>();
        for(int i=0;i<temp.Length;i++){
            spawnPoints.Add(temp[i]);
        }

        int n = Random.Range(maxNumber-1,maxNumber + 1 );
        for(int i=0;i<n;i++){
            int id = Random.Range(0,prefabs.Length);
            int id1 = Random.Range(0,spawnPoints.Count);
           
            GameObject go = GameObject.Instantiate(prefabs[id]);
            go.transform.position = spawnPoints[id1].transform.position;
            go.transform.parent = this.transform;
            spawnPoints.RemoveAt(id1);
            AnimalController a = go.GetComponent<AnimalController>();
            a.maxSpeed = this.speed;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
