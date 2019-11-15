using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Minigame1 : MonoBehaviour
{ 
    public static Minigame1 instance;
    public GameObject[] itemPrefabs;
    RunObject[] bg;
    float time = 0;
    float timeSpawn = 20;
    float maxTimeSpawn = 0.1f;
    float speed = 3;
    GameObject item;
    CharController petObjects;
    
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
        item = GameObject.Instantiate(itemPrefabs[Random.Range(0,itemPrefabs.Length)]);
        item.transform.position += new Vector3(0,0,-17);
        RunObject run = item.GetComponent<RunObject>();
        run.speed = -this.speed;
        timeSpawn = 0;
        maxTimeSpawn = Random.Range(2f,2.5f);
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

