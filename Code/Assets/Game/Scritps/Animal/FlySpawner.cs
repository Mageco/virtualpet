using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour
{
    public GameObject[] flyPrefabs;
    int maxCount = 0;
    public Transform[] spawnPoints;
    float time = 0;
    float maxTimeSpawn = 1;

    public CharController character;
    List<GameObject> flies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
       maxCount = Random.Range(1,maxCount);
       character = this.GetComponentInParent<CharController>();
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
                maxTimeSpawn = Random.Range(0.1f,1f);
            }    

            if(character != null){
                maxCount = (int)Mathf.Clamp((character.data.dirty - character.data.MaxDirty*0.7f)/6f,0f,5f);
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
        //go.transform.localScale = Vector3.one;
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
