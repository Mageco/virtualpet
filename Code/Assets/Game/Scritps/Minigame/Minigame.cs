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
    protected float time = 0;
    public float maxTime;

    public GameObject winPrefab;
    public GameObject losePrefab;
    public Vector2 boundX;
    public Vector2 boundY;

    WinPanel winPanel;
    LosePanel losePanel;

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
    protected virtual void Update()
    {
        time += Time.deltaTime;
        if(time >= maxTime){
            if(live == maxLive){
                OnWin(3);
            }else if(live == maxLive - 1)
            {
                OnWin(2);
            }else if(live == maxLive - 2){
                OnWin(1);
            }else {
                OnWin(0);
            }
            Debug.Log("Win");
        }
    }

    public virtual void AddCoin(){

    }

    public virtual void AddDiamon(){
        
    }

    public virtual void UpdateLive(){

    }


    public virtual void OnWin(int start){
        if (winPanel == null)
        {
            var popup = Instantiate(winPrefab) as GameObject;
            popup.transform.SetParent(this.transform);
            popup.transform.localScale = Vector3.one;
            popup.transform.localPosition = Vector3.zero;
            popup.SetActive(true);
            winPanel = popup.GetComponent<WinPanel>();
            winPanel.Load(3,4,5,3);
        }
    }

    public virtual void OnLose(){
        if (losePanel == null)
        {
            var popup = Instantiate(losePrefab) as GameObject;
            popup.transform.SetParent(this.transform);
            popup.transform.localScale = Vector3.one;
            popup.transform.localPosition = Vector3.zero;
            popup.SetActive(true);
            losePanel = popup.GetComponent<LosePanel>();
        }
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

    public void OnHome(){
        GameManager.instance.gameType = GameType.House;
        GameManager.instance.GetPet(0).Load();
        MageManager.instance.LoadSceneWithLoading("House");
    }
}
