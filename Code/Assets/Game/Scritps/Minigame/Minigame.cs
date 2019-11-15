using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public static Minigame instance;
    public int score;
    public int highScore;
    public int live;
    public int maxLive;
    public int level;

    public GameObject winPrefab;
    public GameObject losePrefab;

    void Awake(){
        if(instance == null){
            instance = this;
        }
    }
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void AddCoin(){

    }

    public virtual void AddDiamon(){
        
    }

    public virtual void OnWin(){

    }

    public virtual void OnLose(){

    }
}
