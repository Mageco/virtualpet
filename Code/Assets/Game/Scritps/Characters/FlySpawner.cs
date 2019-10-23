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

    public CharController character;
    List<GameObject> flies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
       maxCount = Random.Range(1,maxCount);
       
    }


    // Update is called once per frame
    void Update()
    {
        if(time > maxTimeSpawn){

            if(maxCount < flies.Count)
            {
                CleanLast();
            }else if(maxCount > flies.Count){
                SpawnFly();
                maxTimeSpawn = Random.Range(1,3);
            }    

            if(character != null){
                maxCount = (int)Mathf.Clamp((character.data.dirty - 50f)/6f,0f,10f);
            }  
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

    void Clean(){
        foreach(GameObject go in flies){
            GameObject.Destroy(go);
        }
        flies.Clear();
    }

    void CleanLast(){
        GameObject.Destroy(flies[0]);
        flies.RemoveAt(0);
    }


}
