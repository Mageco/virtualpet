using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public static Minigame instance;
    public int score;
    public int highScore;
    public int maxLive;
    public int live;
    public int level;

    public GameObject winPrefab;
    public GameObject losePrefab;
    public Vector2 boundX;
    public Vector2 boundY;

    void Awake(){
        if(instance == null){
            instance = this;
        }
        Load();
    }

    protected virtual void Load(){
        GetComponent<Camera> ();
        float ratio = (float)Screen.width / (float)Screen.height;
         Camera.main.orthographicSize = 54f/ratio;
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

    public virtual void AddLive(int n){
        live += n;
        if(live < 0)
            OnLose();
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
