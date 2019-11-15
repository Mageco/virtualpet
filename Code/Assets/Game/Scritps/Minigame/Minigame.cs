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
    public Vector2 boundX;
    public Vector2 boundY;

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

    public bool IsInBound(Vector3 pos){
        if(pos.x > boundX.x && pos.x < boundX.y && pos.y > boundY.x && pos.y < boundY.y){
            return true;
        }else
            return false;
    }

    public Vector3 GetPointInBound(){
        float x = Random.Range(Minigame.instance.boundX.x, Minigame.instance.boundX.y);
        float y = Random.Range(Minigame.instance.boundY.x, Minigame.instance.boundY.y);
        return new Vector3(x, y, 0);
    }
}
