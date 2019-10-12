using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Minigame1 : MonoBehaviour
{ 
    public static Minigame1 instance;
    public ObjectLevel[] levels;

    RunObject[] bg;

    public Transform anchor;
    float time;
    float timeSpawn = 20;
    float maxTimeSpawn = 10;
    int id = 0;
    GameObject item;
    // Start is called before the first frame update

    void Awake(){
        if(instance == null){
            instance = this;
        }

       
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime; 
        if(timeSpawn > maxTimeSpawn){
            SpawnItem();
        }else
        {
            timeSpawn += Time.deltaTime;
        }   
    }

    void SpawnItem(){
        for(int i=0;i<levels.Length;i++)
        {                
            if(time >= levels[i].startTime )
            {
                id = i;
            }
        }       

        item = GameObject.Instantiate(levels[id].itemPrefabs[Random.Range(0,levels[id].itemPrefabs.Length)]);
        //item.transform.position = this.transform.position;
        //maxTimeSpawn += Random.Range (2f,4f);
        timeSpawn = 0;
    }

    public void OnFail(){
       StartCoroutine(OnFailCoroutine());
    }

    IEnumerator OnFailCoroutine(){
        bg = GameObject.FindObjectsOfType<RunObject>();
        for(int i=0;i<bg.Length;i++)
        {
            bg[i].isMove = false;
        }
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public class ObjectLevel{
        public float startTime;
        public GameObject[] itemPrefabs;
} ;
