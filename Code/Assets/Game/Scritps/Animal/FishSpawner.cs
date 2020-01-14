using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public bool isCouroutine = true;
    List<GizmoPoint> spawnPoints = new List<GizmoPoint>();
    public int maxNumber;
    public GameObject[] prefabs;
    List<FishController> animals = new List<FishController>();
    // Start is called before the first frame update
    void Awake()
    {
  
 
    }

    void LoadSpawns(){
       GizmoPoint[] temp = this.transform.GetComponentsInChildren<GizmoPoint>();
        for(int i=0;i<temp.Length;i++){
            spawnPoints.Add(temp[i]);
        }
    }

   public  void Spawn(){
       
        LoadSpawns();
        if(isCouroutine)
            StartCoroutine(SpawnCoroutine());
        else{
            for(int i=0;i<maxNumber;i++){
                SpawnAnimal();
            }
        }

    }

    IEnumerator SpawnCoroutine(){
        int n = Random.Range(maxNumber-1,maxNumber + 1 );
        for(int i=0;i<n;i++){
            SpawnAnimal();
            yield return new WaitForSeconds(Random.Range(0,2f));
        }
    }

    void SpawnAnimal(){
        if(spawnPoints.Count == 0)
            LoadSpawns();
        int id = Random.Range(0,prefabs.Length);
        int id1 = Random.Range(0,spawnPoints.Count);
        GameObject go = GameObject.Instantiate(prefabs[id]);
        Vector3 pos = spawnPoints[id1].transform.position;
        pos.z = Minigame.instance.GetZindex();
        go.transform.position = pos;
        go.transform.parent = this.transform;
        go.transform.localScale *= Random.Range(0.9f,1.1f);

        spawnPoints.RemoveAt(id1);
        FishController a = go.GetComponent<FishController>();
        a.isMinigame = true;            
    }
}
